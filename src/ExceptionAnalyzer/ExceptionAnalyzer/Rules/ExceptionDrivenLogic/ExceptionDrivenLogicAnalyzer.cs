using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ExceptionAnalyzer.Rules.UseMoreSpecificExceptionType;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExceptionAnalyzer.Rules.ExceptionDrivenLogic
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ExceptionDrivenLogicAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "EX006";
        internal static readonly LocalizableString Title = "Do not write logic driven by exceptions.";
        internal static readonly LocalizableString MessageFormat = "This exception is caught in this method. Try to convert try-catch statement into if-else statement.";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, RuleCategories.ExceptionUsages, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

        class ThrowExpressionInfo
        {
            public bool Reported { get; set; }
            public ExpressionSyntax Expression { get; set; }
            public ITypeSymbol Type { get; set; }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction((syntaxNodeContext) =>
            {
                var tryStatement = (TryStatementSyntax) syntaxNodeContext.Node;

                if (tryStatement.Catches.Count == 0)
                {
                    return;
                }

                var allThrowExpressions = GetAllThrowExpressionsInfo(tryStatement.Block, syntaxNodeContext);

                if (allThrowExpressions.Count == 0)
                {
                    return;
                }

                foreach (var catchClauseSyntax in tryStatement.Catches)
                {
                    if (catchClauseSyntax.Declaration == null)
                    {
                        ReportViolationForExpressions(allThrowExpressions, syntaxNodeContext);
                        break;
                    }

                    var catchTypeInfo = syntaxNodeContext.SemanticModel.GetTypeInfo(catchClauseSyntax.Declaration.Type);
                    var expressionsToReport = allThrowExpressions.Where(throwExpression => throwExpression.Reported == false && throwExpression.Type.IsAssignableToType(catchTypeInfo.Type));
                    ReportViolationForExpressions(expressionsToReport, syntaxNodeContext);
                }

            }, SyntaxKind.TryStatement);

        }

        private static IReadOnlyList<ThrowExpressionInfo> GetAllThrowExpressionsInfo(BlockSyntax tryStatementBlock, SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            return tryStatementBlock.GetAllThrowExpressions()
                .Select(x=> new ThrowExpressionInfo
                {
                    Expression = x,
                    Type = syntaxNodeContext.SemanticModel.GetTypeInfo(x).Type
                })
                .Where(x=>x.Type != null)
                .ToList();
        }

        private static void ReportViolationForExpressions(IEnumerable<ThrowExpressionInfo> expressionToReport,   SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            foreach (var expressionInfo in expressionToReport)
            {
                expressionInfo.Reported = true;
                syntaxNodeContext.ReportDiagnostic(Diagnostic.Create(Rule, expressionInfo.Expression.GetLocation()));
            }
        }
    }
}
