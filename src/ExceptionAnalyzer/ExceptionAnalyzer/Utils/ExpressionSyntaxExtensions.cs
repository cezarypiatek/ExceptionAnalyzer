using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExceptionAnalyzer.Rules.UseMoreSpecificExceptionType
{
    public static class ExpressionSyntaxExtensions
    {
        [Pure, NotNull]
        public static string GeTypeName([NotNull] this ObjectCreationExpressionSyntax expressionSyntax)
        {
            return expressionSyntax.Type.GetText().ToString();
        }
    }
}