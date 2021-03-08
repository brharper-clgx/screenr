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
        | Some id -> $"&with_cast={id}"

    let genreQuery genres =
        let genres' =
            genres
            |> List.map Genre.labelToId
            |> List.fold (fun state g -> $"{state},{g}") ""

        $"&with_genres={genres'}"

    let decadeQuery startYear endYear =
        $"&release_date.gte={startYear}-01-01&release_date.lte={endYear}-01-01"

    interface IMovieDbApi with
        member this.ActorSearch name =
            let url =
                $"{baseUrl}/search/person?api_key={apiKey}&query={name}"

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
                    $"%s{baseUrl}/discover/movie"
                    + $"?api_key=%s{apiKey}&language=en-US&sort_by=popularity.desc"
                    + $"%s{decadeQuery startYear endYear}"
                    + $"%s{actorQuery actorIdOp}"
                    + $"%s{genreQuery genres}"
                    |> HttpClient.get

                return
                    match status with
                    | Success ->
                        Ok
                        <| JsonConvert.DeserializeObject<MovieDbResult<Movie>> r
                    | _ -> Error r
            }
