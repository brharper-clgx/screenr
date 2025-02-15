﻿module Server.Apis.InternalApi

open Shared
open Shared.Extensions
open Shared.ApiContract

let getRecommendation (api: IMovieDbApi) =
    fun (details: MovieDetails) ->
        async {
            let! actorResult = api.ActorSearch details.Actor

            let actorIdOp =
                match actorResult with
                 Error msg -> failwith msg
                | Ok r ->
                    match r.Results with
                    | [] -> None
                    | rs -> rs |> List.head |> fun a -> a.Id |> Some

            let (startYear, endYear) = details.Decade |> Decades.labelToRange

            let! discoverResult = api.DiscoverResult actorIdOp (startYear, endYear) details.Genres

            return
                match discoverResult with
                | Error msg -> failwith msg
                | Ok r ->
                    match r.Results with
                    | [] -> "No results..."
                    | movies ->
                        movies
                        |> List.shuffle
                        |> List.head
                        |> fun m -> m.Title
        }

let reader =
    reader {
        let! api = resolve<IMovieDbApi> ()

        return
            {
                GetRecommendation = getRecommendation api
            }
    }
