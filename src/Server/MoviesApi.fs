namespace Server.Apis

open Newtonsoft.Json
open Server.Config
open Shared
open Shared.Extensions
open Shared.MovieDb
open Shared.ApiContract
open Server

module MoviesApi =

    let private baseUrl = "https://api.themoviedb.org/3"

    let private getActorId apiKey (name: string) =
        let url =
            $"{baseUrl}/search/person?api_key={apiKey}&query={name}"

        async {
            let! (status, r) = Http.get url


            return
                if status > 299 then
                    None
                else
                    let actor =
                        JsonConvert
                            .DeserializeObject<MovieDbResult<Actor>>(r)
                            .Results
                        |> List.head

                    Some actor.Id
        }

    let getMovie (config: IConfigProvider) =
        fun details ->
            let apiKey = config.GetApiKey()

            let (startYear, endYear) = details.Decade |> Decades.labelToRange


            let decadeQuery =
                $"&release_date.gte={startYear}-01-01&release_date.lte={endYear}-01-01"

            let genres =
                details.Genres
                |> List.map Genre.labelToId
                |> List.fold (fun state g -> $"{state},{g}") ""

            let genreQuery = $"&with_genres={genres}"

            async {
                let! actorId = getActorId apiKey details.Actor

                let actorQuery =
                    match actorId with
                    | None -> ""
                    | Some id -> $"&with_cast={id}"

                let! (status, r) =
                    Http.get
                        $"{baseUrl}/discover/movie?api_key={apiKey}&language=en-US&sort_by=popularity.desc{decadeQuery}{actorQuery}{genreQuery}"

                return
                    if status > 299 then
                        "...no results. Try again"
                    else
                        let movies =
                            JsonConvert
                                .DeserializeObject<MovieDbResult<Movie>>(r)
                                .Results

                        match movies with
                        | [] -> "No Results..."
                        | ms ->
                            let m =
                                ms
                                |> List.shuffle
                                |> List.head

                            m.Title

            }


    let reader =
        reader {
            let! config = resolve<IConfigProvider>()

            return
                {
                    GetMovie = getMovie config
                }
        }