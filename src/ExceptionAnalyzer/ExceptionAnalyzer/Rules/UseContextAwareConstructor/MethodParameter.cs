using System;
using Microsoft.CodeAnalysis;

namespace ExceptionAnalyzer.Rules.UseContextAwareConstructor
{
    class MethodParameter
    {
        public string TypeName { get; }
        public string Name { get;  }

        public MethodParameter(string typeName, string name)
        {
            TypeName = typeName;
            Name = name;
        }

        public bool IsSameAs(IParameterSymbol parameter)
        {
            if (parameter.Type.Name.Equals(TypeName, StringComparison.OrdinalIgnoreCase) == false)
            {
                return false;
            }

            if (parameter.Name.Equals(Name, StringComparison.OrdinalIgnoreCase) == false)
            {
                return false;
            }

            return false;
        }
    }
}