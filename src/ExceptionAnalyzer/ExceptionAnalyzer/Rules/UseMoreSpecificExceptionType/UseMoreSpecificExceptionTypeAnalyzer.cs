using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExceptionAnalyzer.Rules.UseMoreSpecificExceptionType
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseMoreSpecificExceptionTypeAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = "EX001";
        private static readonly LocalizableString Title = "Don not use generic exception types";
        private static readonly LocalizableString MessageFormat = "Use more specific exception type than '{0}'";
        private const string Category = "UseMoreSpecificExceptionTypeAnalyzer Category";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

        private static readonly string[] forbiddenExceptionTypes = new []{ "Exception" , "ApplicationException", "SystemException"};

        public override void Initialize([NotNull] AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(syntaxNodeContext=>
            {
                var throwStatement = (ThrowStatementSyntax) syntaxNodeContext.Node;
                AnalyzeThrowSubExpression(syntaxNodeContext, throwStatement.Expression);
            }, SyntaxKind.ThrowStatement);

            context.RegisterSyntaxNodeAction(syntaxNodeContext=>
            {
                var throwExpression = (ThrowExpressionSyntax) syntaxNodeContext.Node;
                AnalyzeThrowSubExpression(syntaxNodeContext, throwExpression.Expression);
            }, SyntaxKind.ThrowExpression);
        }

        private static void AnalyzeThrowSubExpression(SyntaxNodeAnalysisContext syntaxNodeContext, [NotNull] ExpressionSyntax expression)
        {
            if (expression is ObjectCreationExpressionSyntax objectCreation)
            {
                var typeName = objectCreation.GeTypeName();
                if (forbiddenExceptionTypes.Contains(typeName))
                {
                    var diagnostic = Diagnostic.Create(Rule, objectCreation.Type.GetLocation(), typeName);
                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
