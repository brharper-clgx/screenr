module Shared.ApiContract

[<CLIMutable>]
type MovieDetails =
    {
        Actor: string
        Genre: string
        Decade: string
    }

type IInternalApi =
    {
         GetRecommendation: MovieDetails -> Async<string>
    }

module Route =
    let builder typeName methodName = sprintf "/api/%s/%s" typeName methodName