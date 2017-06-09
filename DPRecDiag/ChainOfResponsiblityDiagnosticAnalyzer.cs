using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using DPRec_Lib.CSharpImplementation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using DPRec_Lib.Model;
using DPRec_Lib.Recognition.ChainOfResponbility;

namespace DPRecDiag
{    
    [DiagnosticAnalyzer]
    [ExportDiagnosticAnalyzer(DiagnosticId, LanguageNames.CSharp)]
    public class ChainOfResponsiblityDiagnosticAnalyzer : ISymbolAnalyzer
    {
        internal const string DiagnosticId = "ChainOfResponsibilityRec";
        internal const string Description = "This is probably a Chain of responsiblity base class";
        internal const string MessageFormat = "This is probably a Chain of responsiblity base class";
        internal const string Category = "CDPRec";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Info);

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public ImmutableArray<SymbolKind> SymbolKindsOfInterest { get { return ImmutableArray.Create(SymbolKind.NamedType); } }

        public void AnalyzeSymbol(ISymbol symbol, Compilation compilation, Action<Diagnostic> addDiagnostic, CancellationToken cancellationToken)
        {
            //var tree = compilation.SyntaxTrees.ToList()[2];            
            //var model = compilation.GetSemanticModel(tree);

            var typeSymbol = (INamedTypeSymbol)symbol;

            ((CSharpRecognitionContext)RecognitionContextManager.ctx).Compilations = new List<Compilation> { compilation };
            RecognitionContextManager.InitLogger();

            RecognitionContextManager.ctx.Init(RecognitionContextManager.GetLogger(), false);
            ChainOfResponsiblityRecognizer fmrec = new ChainOfResponsiblityRecognizer();
            fmrec.Context = RecognitionContextManager.ctx;

            foreach (var m in RecognitionContextManager.ctx.Types.Cast<CSharpNamedType>())
            {
                if (m.SourceSymbol == typeSymbol)
                {
                    if (fmrec.IsInstance(m))
                    {
                        var diagnostic = Diagnostic.Create(Rule, typeSymbol.Locations[0], typeSymbol.Name);
                        addDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}
