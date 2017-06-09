using DPRec_Lib.CSharpImplementation;
using DPRec_Lib.Helpers;
using DPRec_Lib.Logging;
using DPRec_Lib.Model;
using DPRec_Lib.Profiling;
using DPRec_Lib.Recognition.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.CSharpImplementation
{
    public class CSharpRecognitionContext : RecognitionContextBase
    {

        public CSharpRecognitionContext(string name) : base(name)
        {

        }      
        public List<Compilation> Compilations { get; set; }

        public Dictionary<ISymbol, SemanticModel> SemanticModels { get; private set; }

        public Dictionary<ISymbol, SyntaxTree> SyntaxTrees { get; private set; }

        public Dictionary<SyntaxTree, SemanticModel> TreeModels { get; private set; }

        private List<ISymbol> symbols;

        public override void Init(LoggerProvider p, bool reInit = false)
        {
            if (this.Initialized && !reInit)
            {
                return;
            }
            this.Logger = p;
            this.Types = new List<NamedTypeBase>();

            this.symbols = new List<ISymbol>();
            this.SemanticModels = new Dictionary<ISymbol, SemanticModel>();
            this.TreeModels = new Dictionary<SyntaxTree, SemanticModel>();
            this.SyntaxTrees = new Dictionary<ISymbol, SyntaxTree>();

            List<Tuple<ISymbol, SemanticModel>> tempSymbols = new List<Tuple<ISymbol, SemanticModel>>();

            using (ProfilerService.Current.Section("Getting symbols"))
            {
                for (int i = 0; i < Compilations.Count; i++)
                {
                    var comp = Compilations[i];
                    var compTrees = comp.SyntaxTrees.ToList();
                    for (int j = 0; j < compTrees.Count; j++)
                    {
                        tempSymbols.AddRange(GetSymbols(compTrees[j], comp));
                    }
                }
            }

            using (ProfilerService.Current.Section("Building symbol info"))
            {
                this.symbols = tempSymbols.Select(t => t.Item1).ToList();
                for (int i = 0; i < tempSymbols.Count; i++)
                {
                    if (tempSymbols[i].Item1.Locations.Length > 0 && tempSymbols[i].Item1.Locations[0].IsInSource)
                    {
                        var sTree = tempSymbols[i].Item1.Locations[0].SourceTree;

                        if (!this.SemanticModels.ContainsKey(tempSymbols[i].Item1))
                        {
                            SyntaxTrees.Add(tempSymbols[i].Item1, sTree);
                        }

                        var sComp = Compilations.SingleOrDefault(c => c.SyntaxTrees.Contains(sTree));

                        if (sComp == null)
                        {
                            Logger.WriteToLog(string.Format("Cannot find compilation for tree {0}", sTree.FilePath));
                        }
                        else
                        {
                            if (!this.SemanticModels.ContainsKey(tempSymbols[i].Item1))
                            {
                                this.SemanticModels.Add(tempSymbols[i].Item1, sComp.GetSemanticModel(sTree));
                            }
                            if (!this.TreeModels.ContainsKey(sTree))
                            {
                                this.TreeModels.Add(sTree, sComp.GetSemanticModel(sTree));
                            }
                        }

                    }
                    else
                    {
                        if (!this.SemanticModels.ContainsKey(tempSymbols[i].Item1))
                        {
                            this.SemanticModels.Add(tempSymbols[i].Item1, tempSymbols[i].Item2);
                        }                        
                    }
                }
            }


            CSharpModelBuilder.InitLogger(p);

            using (ProfilerService.Current.Method("BuildTypeList"))
            {
                CSharpModelBuilder.BuildTypeList(Types, symbols.Where(s => s.Kind == SymbolKind.NamedType).Distinct().Cast<INamedTypeSymbol>());
            }

            this.Types = this.Types.Distinct(new TypeComparer()).ToList();

            using (ProfilerService.Current.Method("BuildArrayTypeList"))
            {
                CSharpModelBuilder.BuildArrayTypeList(Types, symbols.Where(s => s.Kind == SymbolKind.ArrayType).Distinct().Cast<IArrayTypeSymbol>());
            }

            using (ProfilerService.Current.Method("BuildMethodList"))
            {
                CSharpModelBuilder.BuildMethodList(Types, symbols.Where(s => s.Kind == SymbolKind.Method).Distinct().Cast<IMethodSymbol>()
                                                                         .Where(s => s.MethodKind == MethodKind.Ordinary || s.MethodKind == MethodKind.Constructor || s.MethodKind == MethodKind.PropertyGet || s.MethodKind == MethodKind.PropertySet), this.SemanticModels);
            }

            using (ProfilerService.Current.Method("BuildFieldList"))
            {
                CSharpModelBuilder.BuildFieldList(Types, symbols.Where(s => s.Kind == SymbolKind.Field).Distinct().Cast<IFieldSymbol>(), symbols.Where(s => s.Kind == SymbolKind.Property).Cast<IPropertySymbol>(), this.SemanticModels);
            }

            using (ProfilerService.Current.Method("BuildCallGraph"))
            {
                this.CallGraph = CSharpModelBuilder.BuildCallGraph(this.TreeModels, this.Types, this.Logger);
            }

            this.Initialized = true;

            using (var fs = File.Create(string.Join(".",this.Name,"bin")))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, this.Types);
            }
        }


        private List<Tuple<ISymbol,SemanticModel>> GetSymbols(SyntaxTree tree, Compilation comp)
        {
            var root = tree.GetRoot();
            var nodes = root.DescendantNodes();
            var model = comp.GetSemanticModel(tree);

            List<ISymbol> s = new List<ISymbol>();
            foreach (var node in nodes)
            {
                var symbol = model.GetDeclaredSymbol(node);
                if (symbol != null)
                {
                    s.Add(symbol);
                }
                else
                {
                    var symbol2 = model.GetSymbolInfo(node);
                    if (symbol2.Symbol != null)
                    {
                        s.Add(symbol2.Symbol);
                    }
                }
            }

            var temp = s.Where(a => a.Kind != SymbolKind.Namespace).Distinct();
            return temp.Select(t => new Tuple<ISymbol, SemanticModel>(t, model)).ToList();
        }


    }
}
