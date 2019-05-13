using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace SmartanAlyzers.ExceptionAnalyzer.Rules.UseContextAwareConstructor
{
    class MethodSignature
    {
        public MethodParameter[] Parameters { get;  }

        public MethodSignature(params MethodParameter[] parameters)
        {
            Parameters = parameters;
        }


        [Pure]
        public bool IsSimilarTo(IMethodSymbol methodSymbol)
        {
            if (methodSymbol.Parameters.Length != this.Parameters.Length)
            {
                return false;
            }

            for (int index = 0; index < this.Parameters.Length; index++)
            {
                if (this.Parameters[index].IsSameAs(methodSymbol.Parameters[index]) == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}