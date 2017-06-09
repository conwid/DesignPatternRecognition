using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DPRec_Lib.CSharpImplementation;
using DPRec_Lib.Logging;
using System.Text;

namespace DPRec_Lib.Model
{
    public static class BaseExtensions
    {

        public static bool CompareForArrayTypeCompatibility(NamedTypeBase t1, NamedTypeBase t2)
        {

            if (t1.Name != t2.Name)
            {
                return false;
            }
            if (t1.Namespace != t2.Namespace)
            {
                return false;
            }

            if (t1.IsGeneric != t2.IsGeneric)
            {
                return false;
            }
            if (t1.IsGeneric)
            {
                if (t1.GenericParameters.Count != t2.GenericParameters.Count)
                {
                    return false;
                }
                for (int i = 0; i < t1.GenericParameters.Count; i++)
                {
                    if ((GenericTypeParameterBase)t1.GenericParameters[i] != (GenericTypeParameterBase)t2.GenericParameters[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        public static VisibilityModifierBase ToVisibilityModifierBase(this Accessibility accessibility, LoggerProvider logprovider)
        {
            switch (accessibility)
            {
                case Microsoft.CodeAnalysis.Accessibility.Private:
                    return VisibilityModifierBase.Private;
                case Microsoft.CodeAnalysis.Accessibility.Protected:
                    return VisibilityModifierBase.Protected;
                case Microsoft.CodeAnalysis.Accessibility.Internal:
                    return VisibilityModifierBase.Internal;
                case Microsoft.CodeAnalysis.Accessibility.Public:
                    return VisibilityModifierBase.Public;
                default:
                    logprovider.WriteToLog(string.Format("Could not convert visiblity {0}", accessibility.ToString()));
                    return VisibilityModifierBase.Default;
            }
        }

        public static bool CheckForOverrideCompatiblity(IMethod caller, IMethod callee)
        {


            if (caller.Name != callee.Name)
            {
                return false;
            }
            if (caller.IsGeneric != callee.IsGeneric)
            {
                return false;
            }
            if (caller.Parameters.Count != callee.Parameters.Count)
            {
                return false;
            }
            if (caller.Parameters.Except(callee.Parameters).Count() != 0)
            {
                return false;
            }
            if (caller.IsGeneric)
            {
                if (caller.GenericParameters.Count != callee.GenericParameters.Count)
                {
                    return false;
                }
                for (int i = 0; i < caller.GenericParameters.Count; i++)
                {
                    if (caller.GenericParameters[i] != callee.GenericParameters[i])
                    {
                        return false;
                    }
                }
            }
            return true;

        }

        public static MethodBase ToCSharpMethod(this IMethodSymbol methodSymbol, List<NamedTypeBase> types, SemanticModel model, NamedTypeBase parentType, LoggerProvider logprovider)
        {
            if (methodSymbol == null)
            {
                return null;
            }

            INamedType drt = null;

            //hack
            if (methodSymbol.ReturnType.TypeKind != TypeKind.TypeParameter)
            {
                drt = types.FirstOrDefault(t => t == methodSymbol.ReturnType.ToCSharpNamedType(logprovider));
                if (drt == null && !methodSymbol.ReturnsVoid)
                {
                    logprovider.WriteToLog(string.Format("Cannot find type for {0} as the non-void declared return type of method {1}, creating empty type manually", methodSymbol.ReturnType.ToString(), methodSymbol.ToString()));
                    types.Add(methodSymbol.ReturnType.ToCSharpNamedType(logprovider));
                }
            }
            List<IMethodParameter> parameters = new List<IMethodParameter>();
            foreach (var paramSymbol in methodSymbol.Parameters)
            {
                parameters.Add(paramSymbol.ToCSharpMethodParameter(types, logprovider));
            }

            return new CSharpMethod
            {
                IsAbstract = methodSymbol.IsAbstract,
                IsOverride = methodSymbol.IsOverride,
                IsStatic = methodSymbol.IsStatic,
                IsVirtual = methodSymbol.IsVirtual,
                Parent = parentType,
                Name = methodSymbol.Name,
                IsCtor = (methodSymbol.MethodKind == MethodKind.Constructor),
                Visibility = methodSymbol.DeclaredAccessibility.ToVisibilityModifierBase(logprovider),
                DeclaredReturnType = drt,
                Parameters = parameters,
                ActualReturnTypes = methodSymbol.GetActualReturnTypes(types, model),
                IsGeneric = methodSymbol.IsGenericMethod,
                SourceSymbol = methodSymbol,
                GenericParameters = CSharpModelBuilder.BuildGenericList(methodSymbol.TypeArguments)
            };
        }


        public static CSharpMethodParameter ToCSharpMethodParameter(this IParameterSymbol s, List<NamedTypeBase> types, LoggerProvider logger)
        {
            if (s == null)
            {
                return null;
            }
            var p = new CSharpMethodParameter() { Ordinal = s.Ordinal, Name = s.Name };
            if (s.Type.TypeKind == TypeKind.TypeParameter)
            {
                p.IsTypeParameter = true;
                return p;
            }
            else
            {
                p.IsTypeParameter = false;
                p.Type = s.Type.ToCSharpNamedType(logger);
                return p;
            }
        }

        public static NamedTypeBase ToCSharpNamedType(this ITypeSymbol s, LoggerProvider logprovider)
        {

            if (s == null)
            {
                return null;
            }
            if (s is IArrayTypeSymbol)
            {
                var t = (ITypeSymbol)(((IArrayTypeSymbol)s).ElementType);
                return new CSharpNamedType
                {
                    IsArray = true,
                    Namespace = GetNamespace(t),
                    IsStatic = t.IsStatic,
                    IsAbstract = t.IsAbstract,
                    Visibility = t.DeclaredAccessibility.ToVisibilityModifierBase(logprovider),
                    Name = t.Name,
                    SourceSymbol = t
                };
            }
            else
            {
                var t = s as INamedTypeSymbol;
                if (t == null)
                {
                    logprovider.WriteToLog(string.Format("Symbol {0} has an unknown type", s.ToString(), s.TypeKind.ToString()));

                }
                return new CSharpNamedType
                {
                    IsArray = false,
                    IsStatic = t.IsStatic,
                    Namespace = GetNamespace(t),
                    IsAbstract = t.IsAbstract,
                    Visibility = t.DeclaredAccessibility.ToVisibilityModifierBase(logprovider),
                    IsGeneric = t.IsGenericType,
                    GenericParameters = CSharpModelBuilder.BuildGenericList(t.TypeArguments),
                    Name = t.Name,
                    SourceSymbol = t
                };
            }
        }

        private static string GetNamespace(ITypeSymbol t)
        {
            StringBuilder sb = new StringBuilder(t.ContainingNamespace.Name);
            var ns = t.ContainingNamespace.ContainingNamespace;
            while (ns != null)
            {
                sb.Insert(0, string.Format("{0}.",ns.Name));				
                ns = ns.ContainingNamespace;
            }
            return sb.ToString();
        }

        public static bool IsFieldInitializedOnce(this IFieldSymbol field, NamedTypeBase sType, Dictionary<ISymbol, SemanticModel> models, LoggerProvider logprovider)
        {
            var type = sType as CSharpNamedType;
            if (type == null)
            {
                throw new ArgumentException();
            }

            if (type.SourceSymbol.DeclaringSyntaxReferences == null)
            {
                logprovider.WriteToLog(string.Format("Cannot find declaring syntax references for symbol {0}", type.SourceSymbol.ToString()));
                return false;
            }

            if (type.SourceSymbol.DeclaringSyntaxReferences.Count() > 1)
            {
                logprovider.WriteToLog(string.Format("There are more than 1 declaring references for symbol {0}", type.SourceSymbol.ToString()));
                return false;
            }

            if (type.SourceSymbol.DeclaringSyntaxReferences.Count() < 1)
            {
                logprovider.WriteToLog(string.Format("There are less than 1 declaring references for symbol {0}", type.SourceSymbol.ToString()));
                return false;
            }

            if (type.SourceSymbol.DeclaringSyntaxReferences[0].GetSyntax() == null)
            {
                logprovider.WriteToLog(string.Format("Cannnot get syntax for declaring syntax reference {0}", type.SourceSymbol.DeclaringSyntaxReferences[0].ToString()));
                return false;
            }

            var ase = type.SourceSymbol.DeclaringSyntaxReferences[0].GetSyntax().DescendantNodes().Where(c => c.CSharpKind() == SyntaxKind.SimpleAssignmentExpression).ToList();
            foreach (var a in ase)
            {
                var id = a.DescendantNodes().OfType<IdentifierNameSyntax>().ToList();
                if (id.Count != 1)
                {
                    return false;
                }
                SemanticModel model = null;
                if (models.TryGetValue(field, out model) || models.TryGetValue(field.ContainingSymbol, out model))
                {
                    try
                    {
                        var s = model.GetSymbolInfo(id[0]);

                        if (s.Symbol == null)
                        {
                            logprovider.WriteToLog(string.Format("Cannot find symbol for IdentifierNameSyntax {0}", id[0].ToString()));
                            return false;
                        }

                        if (s.Symbol.Name == field.Name)
                        {
                            return true;
                        }
                    }
                    //hack
                    catch (Exception ex)
                    {
                        logprovider.WriteToLog(string.Format("Error when finding symbol for IdentifierNameSyntax {0}", id[0].ToString()));
                        return false;
                    }

                }

            }
            return false;
        }
    }
}
