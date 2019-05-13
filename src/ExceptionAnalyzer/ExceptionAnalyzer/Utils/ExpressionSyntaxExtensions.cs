using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SmartanAlyzers.ExceptionAnalyzer.Utils
{
    public static class ExpressionSyntaxExtensions
    {
        [Pure, NotNull]
        public static string GeTypeName([NotNull] this ObjectCreationExpressionSyntax expressionSyntax)
        {
            return expressionSyntax.Type.GetText().ToString();
        }

        public static IReadOnlyList<ExpressionSyntax> GetAllThrowExpressions(this BlockSyntax codeBlock)
        {
            var expressionFromThrowStatements = codeBlock.DescendantNodes().OfType<ThrowStatementSyntax>().Select(x => x.Expression);
            var expressionFromThrowExpressions = codeBlock.DescendantNodes().OfType<ThrowExpressionSyntax>().Select(x => x.Expression);
            return expressionFromThrowStatements.Concat(expressionFromThrowExpressions).ToList();
        }
    }
}