using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SmartanAlyzers.ExceptionAnalyzer.Utils;

namespace SmartanAlyzers.ExceptionAnalyzer.Rules.ProvideInnerExceptionInCatch
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ProvideInnerExceptionInCatchAnalyzer : DiagnosticAnalyzer
    {
        private static class Rule1Container
        {
            private const string DiagnosticId = "EX003";
            private static readonly LocalizableString Title = "Always provide inner exception when throw from the catch clauses.";
            private static readonly LocalizableString MessageFormat = "{0}";

            public static DiagnosticDescriptor Create()
            {
                return new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, RuleCategories.ExceptionUsages, DiagnosticSeverity.Error, true);
            }
        }

        private static readonly DiagnosticDescriptor RuleEX003 = Rule1Container.Create();

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(RuleEX003);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(syntaxNodeContext =>
            {
                var catchClause = (CatchClauseSyntax) syntaxNodeContext.Node;
                var exceptionVariable = catchClause.Declaration?.Identifier.Text;
                var allExpressions = catchClause.Block.GetAllThrowExpressions();

                if (allExpressions.Count == 0)
                {
                    return;
                }

                foreach (var expression in allExpressions)
                {
                    if (expression is ObjectCreationExpressionSyntax objectCreationExpression)
                    {
                        TryToReportDiagnostic(objectCreationExpression.ArgumentList, exceptionVariable, syntaxNodeContext, objectCreationExpression);
                    }
                    else if (expression is InvocationExpressionSyntax invocationExpressionSyntax)
                    {
                        TryToReportDiagnostic(invocationExpressionSyntax.ArgumentList, exceptionVariable, syntaxNodeContext, invocationExpressionSyntax);
                    }
                }


            }, SyntaxKind.CatchClause);
        }

        private static void TryToReportDiagnostic(ArgumentListSyntax argumentListSyntax, string exceptionVariable, SyntaxNodeAnalysisContext syntaxNodeContext, ExpressionSyntax objectCreationExpression)
        {
            if (string.IsNullOrWhiteSpace(exceptionVariable))
            {
                var diagnostic = Diagnostic.Create(RuleEX003, objectCreationExpression.GetLocation(), "Complete Catch declaration with exception variable and provide it as a inner exception when throw from the catch clauses");
                syntaxNodeContext.ReportDiagnostic(diagnostic);
                return;
            }

            var isPassingInner = argumentListSyntax.Arguments.Any(x => x.Expression is IdentifierNameSyntax identifier && identifier.Identifier.Text == exceptionVariable);

            if (isPassingInner == false)
            {
                var diagnostic = Diagnostic.Create(RuleEX003, objectCreationExpression.GetLocation(),  $"Provide '{exceptionVariable}' as a inner exception when throw from the catch clauses");
                syntaxNodeContext.ReportDiagnostic(diagnostic);
            }
        }
    }
}
