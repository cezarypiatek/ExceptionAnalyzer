using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SmartanAlyzers.ExceptionAnalyzer.Rules.UseContextAwareConstructor
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseContextAwareConstructorAnalyzer : ThrowExpressionBaseAnalyzer
    {
        private const string DiagnosticId = "EX002";
        private static readonly LocalizableString Title = "UseContextAwareConstructorAnalyzer Title";

        private static readonly LocalizableString MessageFormat = "Do not use standard exception constructor. Use one that accept context information";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, RuleCategories.ExceptionUsages, DiagnosticSeverity.Error, true);

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
                     return forbiddenSignatures.Any(t => t.IsSimilarTo(methodSymbol)) == false;
                 }
            }

            return true;
        }

        private static readonly MethodSignature[] forbiddenSignatures = new[]
        {
            new MethodSignature(new MethodParameter("String", "message")), 
            new MethodSignature(new MethodParameter("String", "message"), new MethodParameter("Exception", "innerException")), 
            new MethodSignature(new MethodParameter("SerializationInfo", "info"), new MethodParameter("StreamingContext", "context"))
        };
    }
}
