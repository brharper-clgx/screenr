module Shared.ApiContract

[<CLIMutable>]
type MovieDetails =
    {
        Actor: string
        Genres: string list
        Decade: string
    }

type IInternalApi =
    {
         GetRecommendation: MovieDetails -> Async<string>
    }

module Route =
    let builder typeName methodName = sprintf "/api/%s/%s" typeName methodName