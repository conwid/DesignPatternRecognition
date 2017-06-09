using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using DPRec_Lib.CSharpImplementation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using DPRec_Lib.Model;
using DPRec_Lib.Recognition.FactoryMethod;

namespace DPRecDiag
{    
    [DiagnosticAnalyzer]
    [ExportDiagnosticAnalyzer(DiagnosticId, LanguageNames.CSharp)]
    public class FactoryDiagnosticAnalyzer : ISymbolAnalyzer
    {
        internal const string DiagnosticId = "FactoryRec";
        internal const string Description = "This is probably a Factory method";
        internal const string MessageFormat = "This is probably a Factory method";
        internal const string Category = "FDPRec";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Info);

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public ImmutableArray<SymbolKind> SymbolKindsOfInterest { get { return ImmutableArray.Create(SymbolKind.Method); } }

        public void AnalyzeSymbol(ISymbol symbol, Compilation compilation, Action<Diagnostic> addDiagnostic, CancellationToken cancellationToken)
        {
            
            //var tree = compilation.SyntaxTrees.ToList()[2];
            //var model = compilation.GetSemanticModel(tree);

            var methodSymbol = (IMethodSymbol)symbol;

            ((CSharpRecognitionContext)RecognitionContextManager.ctx).Compilations = new List<Compilation> { compilation };
            RecognitionContextManager.InitLogger();
            
                        
            RecognitionContextManager.ctx.Init(RecognitionContextManager.GetLogger(), false);            
            FactoryMethodRecognizer fmrec = new FactoryMethodRecognizer();

            fmrec.Context = RecognitionContextManager.ctx;


            var methods = fmrec.Context.Types.Where(t => t.Methods != null).SelectMany(t => t.Methods);
            foreach (var m in methods.Cast<CSharpMethod>())
            {
                if (m.SourceSymbol==methodSymbol)
                {
                    if (fmrec.IsInstance(m))
                    {
                        var diagnostic = Diagnostic.Create(Rule, methodSymbol.Locations[0], methodSymbol.Name);
                        addDiagnostic(diagnostic);
                    }
                }
            }                                                                                                                      
        }
    }
}
