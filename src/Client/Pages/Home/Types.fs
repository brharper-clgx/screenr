module Client.Pages.Home.Types

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
        Genre: string
        Result: string
        Watchers: string list
        WatcherInput: string
    }

type Msg =
    | ServerError of exn
    | ServerReturnedRecommendation of string
    | UserAddedWatcher
    | UserSelectedActor of string
    | UserChangedWatcherInput of string
    | UserChoseDecade of string
    | UserAddedGenre of string
    | UserClickedNext