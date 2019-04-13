using System.Collections.Immutable;
using ExceptionAnalyzer.Rules.UseMoreSpecificExceptionType;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExceptionAnalyzer.Rules.UseContextAwareConstructor
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseContextAwareConstructorAnalyzer : ThrowExpressionBaseAnalyzer
    {
        private const string DiagnosticId = "EX002";
        private static readonly LocalizableString Title = "UseContextAwareConstructorAnalyzer Title";

        private static readonly LocalizableString MessageFormat = "Do not use standard exception constructor. Use one that accept context information";

        private const string Category = "Exceptions usages";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rule);

        protected override void AnalyzeThrowSubExpression(SyntaxNodeAnalysisContext syntaxNodeContext, ExpressionSyntax expression)
        {
            if (expression is ObjectCreationExpressionSyntax objectCreation)
            {
                if (IsContextAwareConstructor(syntaxNodeContext, objectCreation) == false)
                {
                    syntaxNodeContext.ReportDiagnostic(Diagnostic.Create(Rule, objectCreation.GetLocation()));
                }
            }
        }

        private static bool IsContextAwareConstructor(SyntaxNodeAnalysisContext syntaxNodeContext, [NotNull] ObjectCreationExpressionSyntax objectCreation)
        {
            var argumentsCount = objectCreation.ArgumentList.Arguments.Count;
            if (argumentsCount == 0)
            {
                return false;
            }

            if (argumentsCount <= 2)
            {
                 var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(objectCreation);
                 if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
                 {
                     for (var i = 0; i < forbiddenSignatures.Length; i++)
                     {
                         if (methodSymbol.HasSignature(forbiddenSignatures[i]))
                         {
                             return false;
                         }
                     }
                 }
            }

            return true;
        }

        private static readonly string[][] forbiddenSignatures = new[]
        {
            new[] {"String"},
            new[] {"String", "Exception"},
            new[] {"SerializationInfo", "StreamingContext"}
        };
    }
}
