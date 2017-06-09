using DPRec_Lib.Logging;
using DPRec_Lib.Model;
using DPRec_Lib.Profiling;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.CSharpImplementation
{
    
    public static class CSharpModelBuilder
    {

        public static LoggerProvider logprovider { get; set; }

        private static bool isInitialized;

        public static void InitLogger(LoggerProvider pr)
        {
            if (isInitialized)
            {
                return;
            }
            logprovider = pr;
            isInitialized = true;
        }


        public static List<Compilation> GetRecognitionContextInputFromSolution(string solutionPath)
        {
            var solution = MSBuildWorkspace.Create().OpenSolutionAsync(solutionPath).Result;
            var compilations = solution.Projects.Select(p => p.GetCompilationAsync().Result).ToList();
            return compilations;
        }

        public static void BuildTypeList(List<NamedTypeBase> types, IEnumerable<INamedTypeSymbol> typesymbols)
        {
            types.AddRange(typesymbols.Select(s => s.ToCSharpNamedType(logprovider)));
            logprovider.WriteToLog(string.Format("Built {0} types",types.Count),"",0);
            for (int i=0; i<types.Count; i++)
            {
                using (ProfilerService.Current.Section(string.Format("Building base list for type {0} ({1}/{2})",types[i].ToString(),i.ToString(),types.Count.ToString())))
                {
                    types[i].BuildBaseList(types, typesymbols);
                }
            }
        }
        private static void BuildBaseList(NamedTypeBase item, List<NamedTypeBase> types, IEnumerable<INamedTypeSymbol> symbols)
        {
            //Roslyn szimbólum elkérése
			var originalSymbol = ((CSharpNamedType)item).SourceSymbol;           
            
			//Az összes interfész szimbólumának lekérdezése
			var baseSymbols = originalSymbol.AllInterfaces;
            
			
			//Szimbúlumfeldolgozás
			foreach (var baseSymbol in baseSymbols)
            {
				var interfacetype = types.SingleOrDefault( t => t == originalSymbol.BaseType.ToCSharpNamedType() );

				//Ha még korábban nem találkoztunk ezzel a típussal
				if (interfacetype == null)
                {
					//létrehozzuk a típust és elátroljuk a modellben
					var newtype = baseSymbol.ToCSharpNamedType();                    
					types.Add(newtype);
					//és hozzáadjuk az ősök listájához
                    item.Bases.Add(newtype);
                }
                else
                {
					//egyébként hozzáadjuk az ősök listájához
					item.Bases.Add(interfacetype);
                }
            }

            
            if (originalSymbol.BaseType != null)
            {
				var basetype = types.SingleOrDefault( t => t == originalSymbol.BaseType.ToCSharpNamedType() );
                //Ha még korábban nem találkoztunk ezzel a típussal
				if (basetype == null)
                {
                    //létrehozzuk a típust és elátroljuk a modellben
                    var newtype = originalSymbol.BaseType.ToCSharpNamedType();
                    types.Add(newtype);
                    //és hozzáadjuk az ősök listájához
					item.Bases.Add(newtype);
                }
                else
                {
                    //egyébként hozzáadjuk az ősök listájához
					item.Bases.Add(basetype);
                }
            }
        }

        public static void BuildArrayTypeList(List<NamedTypeBase> types, IEnumerable<IArrayTypeSymbol> arrayTypeSymbols)
        {
            foreach (var array in arrayTypeSymbols)
            {
                types.Add(array.ToCSharpNamedType(logprovider));
            }
        }

        public static void BuildMethodList(List<NamedTypeBase> types, IEnumerable<IMethodSymbol> methodSymbols, Dictionary<ISymbol, SemanticModel> models)
        {
            foreach (var methodSymbol in methodSymbols)
            {
                var parentType = types.Where(t => t == methodSymbol.ContainingType.ToCSharpNamedType(logprovider)).OrderByDescending(t => t.Bases.Count).FirstOrDefault();
                if (parentType == null)
                {
                    logprovider.WriteToLog(string.Format("Cannot find parent type {0} for method {1}, creating empty type manually", methodSymbol.ContainingType.ToString(), methodSymbol.ToString()));
                    parentType = methodSymbol.ContainingType.ToCSharpNamedType(logprovider);
                    types.Add(parentType);
                    parentType.Methods.Add(methodSymbol.ToCSharpMethod(types, models[methodSymbol], parentType, logprovider));
                }
                else
                {
                    parentType.Methods.Add(methodSymbol.ToCSharpMethod(types, models[methodSymbol], parentType, logprovider));
                }
            }
            BuildInterfaceImplementationList(types, methodSymbols, models);
        }
        public static List<IGenericTypeParameter> BuildGenericList(IEnumerable<ITypeSymbol> arguments)
        {
            List<IGenericTypeParameter> parameters = new List<IGenericTypeParameter>();
            for (int i = 0; i < arguments.Count(); i++)
            {
                INamedTypeSymbol type = arguments.ElementAt(i) as INamedTypeSymbol;
                parameters.Add(new GenericTypeParameterBase
                {
                    Position = i,
                    IsSubstituted = (type != null),
                    Type = type == null ? null : type.ToCSharpNamedType(logprovider)
                }
              );
            }
            return parameters;
        }

        public static List<INamedType> GetActualReturnTypes(this IMethodSymbol method, List<NamedTypeBase> types, SemanticModel model)
        {
            List<INamedType> returntypes = new List<INamedType>();
            foreach (var item in method.Locations)
            {
                if (item.IsInSource)
                {                    
                    var ms = model.SyntaxTree.GetRoot().FindNode(item.SourceSpan) as MethodDeclarationSyntax;
                    if (ms != null && ms.Body != null && ms.Body.Statements.Count != 0)
                    {
                        var cf = model.AnalyzeControlFlow(ms.Body.Statements.First(), ms.Body.Statements.Last());
                        foreach (var returnStatement in cf.ReturnStatements)
                        {

                            if (returnStatement is YieldStatementSyntax)
                            {
                                continue;
                            }

                            var ret = (ReturnStatementSyntax)returnStatement;
                            //void methods
                            if (ret.Expression != null && ret.Expression.CSharpKind() != SyntaxKind.NullLiteralExpression)
                            {
                                var type = model.GetTypeInfo(ret.Expression);
                                if (type.Type == null)
                                {
                                    logprovider.WriteToLog(string.Format("Cannot get semantinc type info for expression {0}", ret.Expression.ToString()));
                                    continue;
                                }
                                //hack
                                if (type.Type.TypeKind==TypeKind.TypeParameter)
                                {
                                    logprovider.WriteToLog(string.Format("Cannot get semantinc type info for expression {0}", ret.Expression.ToString()));
                                    continue;
                                }
                                var rtype = types.FirstOrDefault(t => t == ((ITypeSymbol)(type.Type)).ToCSharpNamedType(logprovider));
                                if (rtype != null)
                                {
                                    returntypes.Add(rtype);
                                }
                                else
                                {
                                    logprovider.WriteToLog(string.Format("Cannot find type for symbol {0}", type.Type.ToString()));
                                }
                            }
                        }
                    }

                }
            }
            return returntypes;
        }

        private static List<IMethod> GetDirectlyReturningMethods(ISymbol symbol, NamedTypeBase sType, Dictionary<ISymbol, SemanticModel> models, List<NamedTypeBase> types)
        {
            var type = (CSharpNamedType)sType;
            List<IMethod> methods = new List<IMethod>();

            if (type.SourceSymbol.DeclaringSyntaxReferences == null)
            {
                logprovider.WriteToLog(string.Format("Cannot find declaring syntax references for symbol {0}", type.SourceSymbol.ToString()));
                return methods;
            }

            if (type.SourceSymbol.DeclaringSyntaxReferences.Count() > 1)
            {
                logprovider.WriteToLog(string.Format("There are more than 1 declaring references for symbol {0}", type.SourceSymbol.ToString()));
                return methods;
            }

            if (type.SourceSymbol.DeclaringSyntaxReferences.Count() < 1)
            {
                logprovider.WriteToLog(string.Format("There are less than 1 declaring references for symbol {0}", type.SourceSymbol.ToString()));
                return methods;
            }

            if (type.SourceSymbol.DeclaringSyntaxReferences[0].GetSyntax() == null)
            {
                logprovider.WriteToLog(string.Format("Cannnot get syntax for declaring syntax reference {0}", type.SourceSymbol.DeclaringSyntaxReferences[0].ToString()));
                return methods;
            }

            var rets = type.SourceSymbol.DeclaringSyntaxReferences[0].GetSyntax().DescendantNodes().OfType<ReturnStatementSyntax>().Cast<ReturnStatementSyntax>();
            var candidaterets = rets.Where(ret => ret.Expression != null && ret.Expression.CSharpKind() != SyntaxKind.NullLiteralExpression && (models[type.SourceSymbol].GetTypeInfo(ret.Expression).Type).ToCSharpNamedType(logprovider) == type);
            foreach (var ret in rets)
            {
                var ids = ret.DescendantNodes().OfType<IdentifierNameSyntax>().ToList();
                foreach (var id in ids)
                {
                    var retsymbol = models[type.SourceSymbol].GetSymbolInfo(id);

                    if (retsymbol.Symbol == null)
                    {
                        logprovider.WriteToLog(string.Format("Could not get symbol info for syntax {0}", retsymbol.ToString()));
                    }

                    if (retsymbol.Symbol != null && retsymbol.Symbol.Name == symbol.Name)
                    {
                        SyntaxNode current = ret;
                        while (!(current.CSharpKind() == SyntaxKind.MethodDeclaration || current.CSharpKind() == SyntaxKind.GetAccessorDeclaration || current.CSharpKind()==SyntaxKind.OperatorDeclaration))
                        {
                            current = current.Parent;
                        }
                        var cSymbol = models[type.SourceSymbol].GetDeclaredSymbol(current);

                        if (cSymbol == null)
                        {
                            logprovider.WriteToLog(string.Format("Could not get declared symbol symbol info for syntax {0}", cSymbol.ToString()));
                        }

                        if (cSymbol != null)
                        {
                            methods.Add(((IMethodSymbol)cSymbol).ToCSharpMethod(types, models[type.SourceSymbol], type, logprovider));
                        }
                    }
                }
            }
            return methods;
        }

        private static void BuildInterfaceImplementationList(List<NamedTypeBase> types, IEnumerable<IMethodSymbol> methodSymbols, Dictionary<ISymbol, SemanticModel> model)
        {
            foreach (var methodSymbol in methodSymbols)
            {
                //If not present in out code, DeclaringSyntaxReferences returns zero elements!!
                if (methodSymbol.DeclaringSyntaxReferences.Length > 0 && methodSymbol.DeclaringSyntaxReferences[0].GetSyntax().Parent.CSharpKind() == SyntaxKind.InterfaceDeclaration)
                {

                    foreach (var sType in types.Where(t => t.Bases != null && t.Bases.Any(b => b.Name == methodSymbol.ContainingType.Name)))
                    {
                        var type = sType as CSharpNamedType;
                        if (type == null)
                        {
                            throw new ArgumentException();
                        }
                        var implementor = type.SourceSymbol.FindImplementationForInterfaceMember(methodSymbol);
                        if (implementor != null && type.Methods != null)
                        {
                            var parentType = types.Where(t => t == implementor.ContainingType.ToCSharpNamedType(logprovider)).OrderByDescending(t => t.Bases.Count).FirstOrDefault();
                            SemanticModel mo = null;
                            if (model.TryGetValue(implementor,out mo) || model.TryGetValue(implementor.ContainingSymbol,out mo))
                            {
                                var o = type.Methods.FirstOrDefault(m => (MethodBase)m == ((IMethodSymbol)implementor).ToCSharpMethod(types, mo, parentType, logprovider));
                                if (o != null)
                                {
                                    o.IsOverride = true;
                                }
                            }                            
                        }
                    }
                }
            }
        }     

        public static void BuildFieldList(List<NamedTypeBase> types, IEnumerable<IFieldSymbol> fieldSymbols, IEnumerable<IPropertySymbol> propSymbols, Dictionary<ISymbol, SemanticModel> models)
        {
            foreach (var fieldSymbol in fieldSymbols)
            {
                var parentType = types.Where(t => t.Name == fieldSymbol.ContainingType.Name).OrderByDescending(t => t.Bases.Count).FirstOrDefault();
                if (parentType == null)
                {
                    logprovider.WriteToLog(string.Format("Cannot find parent type {0} for field {1}, creating empty type manually", fieldSymbol.ContainingType.ToString(), fieldSymbol.ToString()));
                    parentType = fieldSymbol.ContainingType.ToCSharpNamedType(logprovider);
                }
                bool canLookforInit = true;
                if (fieldSymbol.DeclaringSyntaxReferences == null)
                {
                    logprovider.WriteToLog(string.Format("Cannot find DeclaringSyntaxReferences for field {0}", fieldSymbol.ToString()));
                    canLookforInit = false;
                }

                if (fieldSymbol.DeclaringSyntaxReferences.Count() > 1)
                {
                    logprovider.WriteToLog(string.Format("Fieldsymbol {0} has more than 1 declaring syntax reference", fieldSymbol.ToString()));
                    canLookforInit = false;
                }

                else if (fieldSymbol.DeclaringSyntaxReferences.Count() < 1)
                {
                    logprovider.WriteToLog(string.Format("Fieldsymbol {0} has less than 1 declaring syntax reference", fieldSymbol.ToString()));
                    canLookforInit = false;
                }

                else if (fieldSymbol.DeclaringSyntaxReferences[0].GetSyntax() == null)
                {
                    logprovider.WriteToLog(string.Format("Could not get syntax node for symbol {0}", fieldSymbol.ToString()));
                    canLookforInit = false;
                }

                //hack
                if (fieldSymbol.Type.TypeKind==TypeKind.TypeParameter)
                {
                    logprovider.WriteToLog(string.Format("{0} of type {1} is generic, information missing", fieldSymbol.ToString(), fieldSymbol.ContainingSymbol.ToString()));
                    continue;
                }
                var fieldType = types.Where(t => t == fieldSymbol.Type.ToCSharpNamedType(logprovider)).OrderByDescending(t => t.Bases.Count).FirstOrDefault();
                if (fieldType == null)
                {
                    logprovider.WriteToLog(string.Format("Could not find type {0} for symbol {1}, creating empty type manually", fieldSymbol.Type.ToString(), fieldSymbol.ToString()));
                    fieldType = fieldSymbol.Type.ToCSharpNamedType(logprovider);
                }

                parentType.Fields.Add(
                    new CSharpField
                {
                    AutoImplemented = true,
                    Name = fieldSymbol.Name,
                    Parent = parentType,
                    Type = fieldType,
                    Visibility = fieldSymbol.DeclaredAccessibility.ToVisibilityModifierBase(logprovider),
                    IsStatic = fieldSymbol.IsStatic,
                    IsInitializedOnCreation = canLookforInit ? fieldSymbol.DeclaringSyntaxReferences[0].GetSyntax().DescendantNodes().OfType<EqualsValueClauseSyntax>().Any() : false,
                    IsInitializedOnce = fieldSymbol.IsFieldInitializedOnce(parentType, models, logprovider),
                    DirectlyReturningMethods = GetDirectlyReturningMethods(fieldSymbol, parentType, models, types),
                    SourceSymbol = fieldSymbol
                }
              );
            }
            foreach (var propSymbol in propSymbols)
            {
                var parentType = types.Where(t => t.Name == propSymbol.ContainingType.Name).OrderByDescending(t => t.Bases.Count).FirstOrDefault();
                if (parentType == null)
                {
                    logprovider.WriteToLog(string.Format("Could not find parent type {0} for property {1}, creating empty type manually", propSymbol, propSymbol.ContainingType));
                    parentType=propSymbol.ContainingType.ToCSharpNamedType(logprovider);
                }
                var gm = propSymbol.GetMethod;
                var sm = propSymbol.SetMethod;
                var autoprop = true;
                if (gm != null && gm.DeclaringSyntaxReferences.Length < 1)
                {
                    autoprop = false;
                }
                if (sm != null && sm.DeclaringSyntaxReferences.Length < 1)
                {
                    autoprop = false;
                }
                if (gm != null && gm.DeclaringSyntaxReferences.Length > 0 && ((AccessorDeclarationSyntax)gm.DeclaringSyntaxReferences[0].GetSyntax()).Body != null)
                {
                    autoprop = false;
                }
                if (sm != null && sm.DeclaringSyntaxReferences.Length > 0 && ((AccessorDeclarationSyntax)sm.DeclaringSyntaxReferences[0].GetSyntax()).Body != null)
                {
                    autoprop = false;
                }
                if (autoprop)
                {
                    parentType.Fields.Add(
                    new CSharpField
                    {
                        Name = "<>_backingfield" + propSymbol.Name,
                        Parent = parentType,
                        Type = types.FirstOrDefault(t => t.Name == propSymbol.Type.Name),
                        Visibility = VisibilityModifierBase.Private,
                        IsStatic = (gm != null && gm.IsStatic) || (sm != null && sm.IsStatic),
                        IsInitializedOnCreation = false,
                        IsInitializedOnce = false,
                        AutoImplemented = true,
                        DirectlyReturningMethods = gm == null ? new List<IMethod>() : new List<IMethod> { gm.ToCSharpMethod(types, models[propSymbol], parentType, logprovider) }
                    });
                }
            }
        }
        public static CallGraphBase BuildCallGraph(Dictionary<SyntaxTree, SemanticModel> treemodels, List<NamedTypeBase> types, LoggerProvider logprovider)
        {
            CallGraphBase callgraph = new CallGraphBase();
            var methods = types.Where(t => t.Methods != null).SelectMany(t => t.Methods);
            var methodInvocations = new Dictionary<InvocationExpressionSyntax, SyntaxTree>();
            foreach (var tree in treemodels.Keys)
            {
                foreach (var invocation in tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>())
                {
                    methodInvocations.Add(invocation, tree);
                }
            }

            foreach (var invocation in methodInvocations.Keys)
            {
                SyntaxNode current = invocation;                
                while (current != null && current.CSharpKind() != SyntaxKind.MethodDeclaration)
                {
                    current = current.Parent;
                }
                if (current==null)
                {
                    continue;
                }
                var parentDeclaration = treemodels[methodInvocations[invocation]].GetDeclaredSymbol(current);
                var methodSymbol = treemodels[methodInvocations[invocation]].GetSymbolInfo(invocation);

                if (parentDeclaration != null && methodSymbol.Symbol != null)
                {
                    var parentMethod = methods.FirstOrDefault(m => ((CSharpMethod)m).SourceSymbol == parentDeclaration);
                    var method = methods.FirstOrDefault(m => ((CSharpMethod)m).SourceSymbol == methodSymbol.Symbol);
                    CallGraphNodeBase node = new CallGraphNodeBase { Caller = parentMethod, Callee = method };
                    callgraph.Nodes.Add(node);
                }
            }
            return callgraph;
        }
    }
}
