module Client.Pages.Home.View

open System
open Browser.Types
open Feliz
open Feliz.Bulma
open Feliz.Bulma.PageLoader
open Shared
open Shared.Extensions
open Shared.MovieDb
open Client.Styles
open Client.Components
open Client.Pages.Home.Types

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
        stepTitle "Step One:" "Who's watching?"
        inputContainer [
            Bulma.control.div [
                Bulma.input.text [
                    prop.value state.WatcherInput
                    prop.placeholder "Use 'Enter' to add"
                    prop.onChange (fun v -> v |> Msg.UserChangedWatcherInput |> dispatch)
                    prop.onKeyPress (fun (k: KeyboardEvent) -> if k.key = "Enter" then dispatch Msg.UserAddedWatcher)
                ]
            ]

            state.Watchers |> List.map Bulma.tag |> Html.div

        ]
        nextBtn dispatch (state.Watchers.Length < 1)

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
                    prop.onChange (fun v -> v |> UserSelectedActor |> dispatch)
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
                            | Step.Result -> result state
                            | _ -> Html.none

                            PageLoader.pageLoader [
                                pageLoader.isWarning
                                if state.CurrentStep = Step.Submitting then pageLoader.isActive
                                prop.children [
                                    PageLoader.title "I am loading some awesomeness"
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
