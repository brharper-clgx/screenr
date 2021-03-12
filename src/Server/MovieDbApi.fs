namespace Server.Apis

open Microsoft.Extensions.Configuration
open Newtonsoft.Json

open Shared.MovieDb
open Server.Configuration
open Server.Http

type IMovieDbApi =
    abstract ActorSearch: string -> Async<Result<MovieDbResult<Actor>, string>>
    abstract DiscoverResult: int option -> int * int -> string list -> Async<Result<MovieDbResult<Movie>, string>>


type MovieDbApi(config: IConfiguration) =
    let baseUrl = "https://api.themoviedb.org/3"
    let apiKey = config.Get<Config>().ApiKey

    let actorQuery (actorId: int option) =
        match actorId with
        | None -> ""
        | Some id -> sprintf "&with_cast=%i" id

    let genreQuery genres =
        let genres' =
            genres
            |> List.map Genre.labelToId
            |> List.fold (sprintf "%s,%i") ""

        sprintf "&with_genres=%s" genres'

    let decadeQuery startYear endYear =
        sprintf "&release_date.gte=%i-01-01&release_date.lte=%i-01-01" startYear endYear

    interface IMovieDbApi with
        member this.ActorSearch name =
            let url =
                sprintf "%s/search/person?api_key=%s&query=%s" baseUrl apiKey name

            async {
                let! (status, r) = HttpClient.get url

                return
                    match status with
                    | Success ->
                        Ok
                        <| JsonConvert.DeserializeObject<MovieDbResult<Actor>> r
                    | _ -> Error r
            }

        member this.DiscoverResult actorIdOp (startYear, endYear) genres =
            async {
                let! (status, r) =
                    baseUrl
                    + "/discover/movie"
                    + sprintf "?api_key=%s&language=en-US&sort_by=popularity.desc" apiKey
                    + decadeQuery startYear endYear
                    + actorQuery actorIdOp
                    + genreQuery genres
                    |> HttpClient.get

                return
                    match status with
                    | Success ->
                        Ok
                        <| JsonConvert.DeserializeObject<MovieDbResult<Movie>> r
                    | _ -> Error r
            }
