using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace SmartanAlyzers.ExceptionAnalyzer.Utils
{
    internal static class SymbolExtensions
    {
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