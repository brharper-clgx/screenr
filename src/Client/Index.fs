module Index

open System
open Elmish
open Fable.Remoting.Client
open Shared
open Shared.ApiContract
open Shared.MovieDb
open Shared.Extensions
open Shared.Extensions.String.Operators


let api =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IMovieApi>


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
        Genres: string list
        Result: string
        Watchers: string list
    }

type Msg =
    | ServerError of exn
    | ServerReturnedRecommendation of string
    | UserAddedWatcher of string list
    | UserChangedActor of string
    | UserChoseDecade of string
    | UserAddedGenre of string list
    | UserClickedAddName of string
    | UserClickedDeleteName of string
    | UserClickedNext

let init (): State * Cmd<Msg> =
    let state =
        {
            Actor = ""
            CurrentStep = Step.Watchers
            Decade = ""
            Genres = []
            Result = ""
            Watchers = []
        }

    state, Cmd.none

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | ServerError _ -> state, Cmd.none
    | ServerReturnedRecommendation r ->
        { state with Result = r; CurrentStep = Step.Result }, Cmd.none
    | UserAddedGenre genres -> { state with Genres = genres }, Cmd.none
    | UserAddedWatcher names -> { state with Watchers = names }, Cmd.none
    | UserChangedActor actor -> { state with Actor = actor }, Cmd.none
    | UserChoseDecade decade -> { state with Decade = decade }, Cmd.none
    | UserClickedAddName name ->
        { state with
            Watchers = name :: state.Watchers
        },
        Cmd.none
    | UserClickedDeleteName name ->
        { state with
            Watchers = state.Watchers |> List.filter ((<>) name)
        },
        Cmd.none
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

            incrementedState, Cmd.OfAsync.either api.GetMovie details ServerReturnedRecommendation ServerError
        | _ -> incrementedState, Cmd.none

open Feliz
open Feliz.Bulma
open Client.Styles
open Client.Components

let stepTitle (title: string) (sub: string) =
    Html.div [
        Html.p [
            prop.className Bulma.Title
            prop.text title
        ]
        Html.p [
            prop.className Bulma.Subtitle
            prop.text sub
        ]
    ]

let nextBtn dispatch disabled =
    Bulma.button.button [
        button.isLarge
        prop.disabled disabled
        prop.className Bulma.IsInfo
        prop.text "Next"
        prop.onClick (fun _ -> dispatch UserClickedNext)
    ]

let watchersStep dispatch state =
    Html.div [
        prop.hidden (state.CurrentStep <> Step.Watchers)
        prop.children [
            stepTitle "Step One:" "Who's watching?"
            Html.div [
                prop.classes [
                    Bulma.Column
                    Bulma.IsOffsetOneThird
                    Bulma.IsOneThird
                ]
                prop.children [
                    TagsInput.input [
                        tagsInput.placeholder "List everyone watching... [use , or ENTER]"
                        tagsInput.defaultValue state.Watchers
                        tagsInput.onTagsChanged (fun v -> v |> UserAddedWatcher |> dispatch)
                        tagsInput.tagProperties [
                            tag.isMedium
                            tag.isRounded
                            color.isInfo
                        ]
                        tagsInput.allowDuplicates false
                    ]
                ]
            ]
            nextBtn dispatch (state.Watchers.Length < 1)
        ]
    ]

let genresStep dispatch state =
    let watcher =
        match state.Watchers with
        | [] -> "Someone"
        | watcher :: _ -> watcher

    Html.div [
        prop.hidden (state.CurrentStep <> Step.Genres)
        prop.children [
            stepTitle "Step Two:" $"{watcher}, pick a genre"
            Html.div [
                prop.classes [
                    Bulma.Column
                    Bulma.IsOffsetOneThird
                    Bulma.IsOneThird
                ]
                prop.children [
                    TagsInput.input [
                        tagsInput.placeholder "List genres... [use , or ENTER]"
                        tagsInput.defaultValue []
                        tagsInput.onTagsChanged (fun v -> v |> UserAddedGenre |> dispatch)
                        tagsInput.tagProperties [
                            tag.isMedium
                            tag.isRounded
                            color.isInfo
                        ]
                        tagsInput.allowDuplicates false
                        tagsInput.allowOnlyAutoCompleteValues true
                        tagsInput.autoCompleteSource (fun input ->
                            Genre.all
                            |> List.map snd
                            |> List.filter (fun genre -> genre |<~ input)
                            |> async.Return)
                    ]
                ]
            ]
            nextBtn dispatch (state.Genres.Length < 1)
        ]
    ]

let actorStep dispatch state =
    let watcher =
        match state.Watchers with
        | [] -> "Someone"
        | [ watcher ] -> watcher
        | _ :: (watcher :: _) -> watcher

    Html.div [
        prop.hidden (state.CurrentStep <> Step.Actor)
        prop.children [
            stepTitle "Step Three:" $"{watcher}, name an actor / actress."
            Html.div [
                prop.classes [
                    Bulma.Column
                    Bulma.IsOffsetOneThird
                    Bulma.IsOneThird
                ]
                prop.children [
                    Bulma.input.text [
                        prop.onChange (fun v -> v |> UserChangedActor |> dispatch)
                    ]
                ]
            ]
            nextBtn dispatch (String.IsNullOrWhiteSpace state.Actor)
        ]
    ]

let decadeStep dispatch state =
    let watcher =
        match state.Watchers with
        | [] -> "Someone"
        | [ watcher ] -> watcher
        | [ _; _ ] -> state.Watchers |> List.shuffle |> List.head
        | _ :: (_ :: (watcher :: _)) -> watcher

    let option (decade: string) =
        Html.option [
            prop.value decade
            prop.text decade
            prop.onChange (fun v -> v |> UserChoseDecade |> dispatch)
        ]

    Html.div [
        prop.hidden (state.CurrentStep <> Step.Decade)
        prop.children [
            stepTitle "Step Three:" $"{watcher}, pick a decade."
            Html.div [
                prop.classes [
                    Bulma.Column
                    Bulma.IsOffsetOneThird
                    Bulma.IsOneThird
                ]
                prop.children [
                    Decades.allLabels
                    |> List.map option
                    |> Bulma.select
                ]
            ]
            nextBtn dispatch (String.IsNullOrWhiteSpace state.Actor)
        ]
    ]

let loading state =
    Html.div [
        prop.hidden (state.CurrentStep <> Step.Submitting)
        prop.children [
            Bulma.title.h2 "Thinking..."
        ]
    ]

let result state =
    Html.div [
        prop.hidden (state.CurrentStep <> Step.Result)
        prop.children [
            Bulma.title.h2 $"Tonight you'll watch: {state.Result}"
        ]
    ]



let render (state: State) (dispatch: Msg -> unit) =
    Html.div [
        Bulma.hero [
            hero.isFullHeight
            prop.className Bulma.IsSuccess
            prop.children [
                Bulma.heroHead [ Navbar.render ]
                Bulma.heroBody [
                    Bulma.container [
                        prop.classes [ Bulma.HasTextCentered ]
                        prop.children [
                            // Use hidden attribute instead of match expression as otherwise tagInputs get merged
                            watchersStep dispatch state
                            genresStep dispatch state
                            actorStep dispatch state
                            decadeStep dispatch state
                            loading state
                            result state
                        ]
                    ]
                ]
            ]
        ]
    ]
