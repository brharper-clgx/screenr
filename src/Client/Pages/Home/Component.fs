module Client.Pages.Home.Component

open System
open Client.Pages.Home.Types
open Elmish
open Fable.Remoting.Client
open Shared.ApiContract
open Shared.Extensions

let api =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IInternalApi>

let init (): State * Cmd<Msg> =
    let state =
        {
            Actor = ""
            CurrentStep = Step.Watchers
            Decade = ""
            ErrorMsg = None
            Genres = []
            Result = None
            Watchers = []
        }

    state, Cmd.none

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | ServerError ex ->
        { state with
            Result = None
            ErrorMsg = Some ex.Message
            CurrentStep = Step.Result
        },
        Cmd.none
    | ServerReturnedRecommendation r ->
        { state with
            Result = Some r
            CurrentStep = Step.Result
        },
        Cmd.none
    | UserUpdatedGenres genre -> { state with Genres = genre }, Cmd.none
    | UserChoseDecade decade -> { state with Decade = decade }, Cmd.none
    | UserClickedNext ->
        let incrementedState =
            { state with
                CurrentStep = Step.next state.CurrentStep
            }

        match state.CurrentStep with
        | Step.Watchers ->
            { incrementedState with
                Watchers = List.shuffle state.Watchers
            },
            Cmd.none
        | Step.Decade ->
            let details =
                {
                    Actor = state.Actor
                    Decade = state.Decade
                    Genres = state.Genres
                }

            incrementedState, Cmd.OfAsync.either api.GetRecommendation details ServerReturnedRecommendation ServerError
        | _ -> incrementedState, Cmd.none
    | UserClickedDeleteWatcher watcher ->
        { state with
            Watchers = state.Watchers |> List.filter ((<>) watcher)
        },
        Cmd.none
    | UserClickedDismissAlert ->
        init ()
    | UserSelectedActor actor -> { state with Actor = actor }, Cmd.none
    | UserUpdatedWatchers watchers -> { state with Watchers = watchers }, Cmd.none