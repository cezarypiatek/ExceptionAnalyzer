using System;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace ExceptionAnalyzer.Rules.UseMoreSpecificExceptionType
{
    internal static class SymbolExtensions
    {
        [Pure]
        public static bool HasSignature([NotNull] this IMethodSymbol methodSymbol, [NotNull] string[] parameters)
        {
            if (methodSymbol.Parameters.Length != parameters.Length)
            {
                return false;
            }

            for (int index = 0; index < parameters.Length; index++)
            {
                if (IsOfType(methodSymbol.Parameters[index], parameters[index]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        [Pure]
        private static bool IsOfType(this IParameterSymbol parameter, string typeName)
        {
            return parameter.Type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase);
        }

        [Pure]
        public static bool IsAssignableToType( [NotNull] this ITypeSymbol symbol, [NotNull] ITypeSymbol type)
        {
            var baseType = symbol;
            do
            {
                if (type.Equals(baseType))
                    return true;

                baseType = baseType.BaseType;
            } while (baseType != null);

            return false;
        }
    }
}