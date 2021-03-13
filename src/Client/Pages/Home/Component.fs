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
            Genre = ""
            Result = ""
            Watchers = []
            WatcherInput = ""
        }

    state, Cmd.none

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | ServerError ex ->
        { state with
            Result = ex.Message
            CurrentStep = Step.Result
        },
        Cmd.none
    | ServerReturnedRecommendation r ->
        { state with
            Result = r
            CurrentStep = Step.Result
        },
        Cmd.none
    | UserAddedGenre genre -> { state with Genre = genre }, Cmd.none
    | UserAddedWatcher ->
        if String.IsNullOrWhiteSpace state.WatcherInput
        then
            state, Cmd.none
        else
            { state with
                Watchers = state.WatcherInput :: state.Watchers
                WatcherInput = ""
            },
            Cmd.none
        | UserSelectedActor actor -> { state with Actor = actor }, Cmd.none
    | UserChangedWatcherInput input -> { state with WatcherInput = input }, Cmd.none
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
                    Genre = state.Genre
                }

            incrementedState, Cmd.OfAsync.either api.GetRecommendation details ServerReturnedRecommendation ServerError
        | _ -> incrementedState, Cmd.none
