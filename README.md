A set of Roslyn analyzers inspired by [The art of designing exceptions](https://cezarypiatek.github.io/post/the-art-of-designing-exceptions/). A detailed description of currently implemented rules can be find in [Exception usage analyzer](https://cezarypiatek.github.io/post/exceptions-usages-analyzer/) article.


## Currently implemented rules:

- EX001: Don not use generic exception types
- EX002: Use context aware exception constructor
- EX003: Always provide inner exception when throw from the catch clauses
- EX005: Use parameter related exception only for method parameters
- EX006: Do not write logic driven by exceptions.
