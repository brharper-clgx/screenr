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
        ErrorMsg: string option
        Genres: string list
        Result: string option
        Watchers: string list
    }

type Msg =
    | ServerError of exn
    | ServerReturnedRecommendation of string
    | UserChoseDecade of string
    | UserClickedNext
    | UserClickedDeleteWatcher of string
    | UserClickedDismissAlert
    | UserSelectedActor of string
    | UserUpdatedGenres of string list
    | UserUpdatedWatchers of string list
