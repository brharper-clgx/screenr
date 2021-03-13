﻿module Client.Pages.Home.Types

type Step =
    | Watchers = 1
    | Genres = 2
    | Actor = 3
    | Decade = 4
    | Submitting = 5
    | Result = 6

module Step =
    let next (step: Step) = step |> int |> (+) 1 |> enum<Step>

type State =
    {
        Actor: string
        CurrentStep: Step
        Decade: string
        ErrorMsg: string option
        Genre: string
        Result: string option
        Watchers: string list
        WatcherInput: string
    }

type Msg =
    | ServerError of exn
    | ServerReturnedRecommendation of string
    | UserAddedGenre of string
    | UserAddedWatcher
    | UserChangedWatcherInput of string
    | UserChoseDecade of string
    | UserClickedNext
    | UserClickedDeleteWatcher of string
    | UserClickedDismissAlert
    | UserSelectedActor of string
