# CorrelationIdHeaderDemo
```
Passing correlation id in request/response headers
```

> In this repo, i m exploring various ways in order to :
>
> - pass correlation id in request/response headers
>
> - use correlation id in trace identifier
>
> - enrich logs with correlation id
>
> - pass correlation id across http calls
>
>
> :one: `Example01` use a custom middleware, a custom delegation handler and a custom log enricher
>
> :two: `Example02` use a custom middleware and a custom delegation handler based on [IHttpContextAccessor](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-context)
>
> :three: `Example03` use a custom middleware and a custom delegation handler based on a custom accessor class using [AsyncLocal](https://docs.microsoft.com/en-us/dotnet/api/system.threading.asynclocal-1)
>
> :four: `Example04` use [CorrelationId middelware](https://github.com/stevejgordon/CorrelationId) and a custom delegation handler
>

**`Tools`** : vs22, net 6.0, web api, serilog, integration-testing, fluent-assertions, nsubstitute