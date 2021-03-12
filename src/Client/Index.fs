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
    |> Remoting.buildProxy<IInternalApi>


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
    | UserChangedActor of string
    | UserChangedWatcherInput of string
    | UserChoseDecade of string
    | UserAddedGenre of string
    | UserClickedAddName of string
    | UserClickedDeleteName of string
    | UserClickedNext

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
        { state with
            Watchers = state.WatcherInput :: state.Watchers
            WatcherInput = ""
        },
        Cmd.none
    | UserChangedActor actor -> { state with Actor = actor }, Cmd.none
    | UserChangedWatcherInput input -> { state with WatcherInput = input }, Cmd.none
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
                    Genre = state.Genre
                }

            incrementedState, Cmd.OfAsync.either api.GetRecommendation details ServerReturnedRecommendation ServerError
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

let inputContainer (contents: ReactElement list) =
    Html.div [
        prop.classes [
            Bulma.Column
            Bulma.IsOffsetOneThird
            Bulma.IsOneThird
        ]
        prop.children contents
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
        prop.children [
            stepTitle "Step One:" "Who's watching?"
            inputContainer [
                Bulma.field.div [
                    prop.classes [
                        Bulma.HasAddons
                        Bulma.IsJustifyContentCenter
                    ]
                    prop.children [
                        Bulma.control.div [
                            Bulma.input.text [
                                prop.value state.WatcherInput
                                prop.onChange (fun v -> v |> Msg.UserChangedWatcherInput |> dispatch)
                            ]
                        ]
                        Bulma.control.div [
                            Bulma.button.a [
                                prop.classes [ Bulma.IsInfo ]
                                prop.onClick (fun _ -> Msg.UserAddedWatcher |> dispatch)
                                prop.children [
                                    Bulma.icon [
                                        icon.isSmall
                                        prop.children [
                                            Html.i [
                                                prop.classes [ FA.Fa; FA.FaPlus ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]

                state.Watchers |> List.map Bulma.tag |> Html.div

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
        prop.children [
            stepTitle "Step Two:" (sprintf "%s, pick a genre." watcher)
            inputContainer [
                Bulma.select [
                    prop.onChange (fun v -> v |> Msg.UserAddedGenre |> dispatch)

                    Genre.all
                    |> List.map (fun (_, s) ->
                        Html.option [
                            prop.value s
                            prop.text s
                        ])
                    |> prop.children

                ]
            ]
            nextBtn dispatch (state.Genre = "")
        ]
    ]

let actorStep dispatch state =
    let watcher =
        match state.Watchers with
        | [] -> "Someone"
        | [ watcher ] -> watcher
        | _ :: (watcher :: _) -> watcher

    Html.div [
        prop.children [
            stepTitle "Step Three:" (sprintf "%s, name an actor / actress." watcher)
            inputContainer [
                Bulma.input.text [
                    prop.onChange (fun v -> v |> UserChangedActor |> dispatch)
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
        ]

    Html.div [
        prop.children [
            stepTitle "Step Four:" (sprintf "%s, pick a decade." watcher)
            inputContainer [
                Bulma.select [
                    prop.onChange (fun v -> v |> UserChoseDecade |> dispatch)

                    Decades.allLabels
                    |> List.map option
                    |> prop.children
                ]
            ]
            nextBtn dispatch (String.IsNullOrWhiteSpace state.Actor)
        ]
    ]

let loading =
    Html.div [
        prop.children [
            Bulma.title.h2 "Thinking..."
        ]
    ]

let result state =
    Html.div [
        prop.children [
            state.Result
            |> sprintf "Tonight you'll watch: %s"
            |> Bulma.title.h2
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
                            match state.CurrentStep with
                            | Step.Watchers -> watchersStep dispatch state
                            | Step.Genres -> genresStep dispatch state
                            | Step.Actor -> actorStep dispatch state
                            | Step.Decade -> decadeStep dispatch state
                            | Step.Submitting -> loading
                            | Step.Result -> result state
                            | _ -> Html.none

                        ]

                    ]

                ]

            ]

        ]

    ]
