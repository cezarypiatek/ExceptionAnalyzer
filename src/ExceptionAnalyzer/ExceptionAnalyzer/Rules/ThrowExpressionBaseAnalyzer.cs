using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExceptionAnalyzer.Rules
{
    public abstract class ThrowExpressionBaseAnalyzer : DiagnosticAnalyzer
    {
        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(syntaxNodeContext =>
            {
                var throwStatement = (ThrowStatementSyntax)syntaxNodeContext.Node;
                AnalyzeThrowSubExpression(syntaxNodeContext, throwStatement.Expression);
            }, SyntaxKind.ThrowStatement);

            context.RegisterSyntaxNodeAction(syntaxNodeContext =>
            {
                var throwExpression = (ThrowExpressionSyntax) syntaxNodeContext.Node;
                AnalyzeThrowSubExpression(syntaxNodeContext, throwExpression.Expression);
            }, SyntaxKind.ThrowExpression);
        }

        protected abstract void AnalyzeThrowSubExpression(SyntaxNodeAnalysisContext syntaxNodeContext, [NotNull] ExpressionSyntax expression);
    }
}
