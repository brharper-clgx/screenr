module Shared.MovieDb

module Genre =
    // https://api.themoviedb.org/3/genre/movie/list?language=en-US
    let all =
        [
            28, "Action"
            12, "Adventure"
            16, "Animation"
            35, "Comedy"
            80, "Crime"
            80, "Crime"
            99, "Documentary"
            18, "Drama"
            10751, "Family"
            14, "Fantasy"
            36, "History"
            27, "Horror"
            10402, "Music"
            9648, "Mystery"
            10749, "Romance"
            878, "Science Fiction"
            10770, "TV Movie"
            53, "Thriller"
            10752, "War"
            37, "Western"
        ]

    let labelToId (label: string) =
        all
        |> List.filter (fun (_, l) -> l = label)
        |> List.map fst
        |> List.head

[<CLIMutable>]
type MovieDbResult<'t> =
    {
        Results: 't list
    }

[<CLIMutable>]
type Actor =
    {
        Id: int
        Name: string
    }

[<CLIMutable>]
type Movie =
    {
        Title: string
        Overview: string
    }
