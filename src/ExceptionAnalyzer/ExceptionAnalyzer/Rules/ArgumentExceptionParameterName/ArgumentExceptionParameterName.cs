using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExceptionAnalyzer.Rules.ArgumentExceptionParameterName
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ArgumentExceptionParameterName : ThrowExpressionBaseAnalyzer
    {
        private const string DiagnosticId = "EX005";
        private static readonly LocalizableString Title = "ArgumentExceptionParameterName Title";
        private static readonly LocalizableString MessageFormat = "Containing method does not declare '{0}' parameter";
        private static  readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, RuleCategories.ExceptionUsages, DiagnosticSeverity.Error, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);


        protected override void AnalyzeThrowSubExpression(SyntaxNodeAnalysisContext syntaxNodeContext, ExpressionSyntax expression)
        {
            if (expression is ObjectCreationExpressionSyntax objectCreation)
            {
                var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(objectCreation);
                if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
                {
                    for (int i = 0; i < methodSymbol.Parameters.Length; i++)
                    {
                        if (methodSymbol.Parameters[i].Name == "paramName")
                        {
                            var argument = objectCreation.ArgumentList.Arguments[i].Expression;

                            var constantValue = syntaxNodeContext.SemanticModel.GetConstantValue(argument);
                            if (constantValue.HasValue)
                            {
                                TryToVerifyParameterName(syntaxNodeContext, constantValue.Value.ToString(), argument);
                            }
                            return;
                        }
                    }
                }
            }
        }

        private static void TryToVerifyParameterName(SyntaxNodeAnalysisContext syntaxNodeContext, [NotNull] string argumentName, [NotNull] ExpressionSyntax literalExpression)
        {
            var methodDeclaration = syntaxNodeContext.Node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (methodDeclaration != null)
            {
                var parameterExist = methodDeclaration.ParameterList.Parameters.Any(x => x.Identifier.Text == argumentName);
                if (parameterExist == false)
                {
                    syntaxNodeContext.ReportDiagnostic(Diagnostic.Create(Rule, literalExpression.GetLocation(), argumentName));
                }
            }
        }
    }
}
