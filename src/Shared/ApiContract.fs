module Shared.ApiContract

[<CLIMutable>]
type MovieDetails =
    {
        Actor: string
        Genres: string list
        Decade: string
    }

type IMovieApi =
    {
         GetMovie: MovieDetails -> Async<string>
    }

module Route =
    let builder typeName methodName = sprintf "/api/%s/%s" typeName methodName