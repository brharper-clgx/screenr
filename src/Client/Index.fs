module Index

open Client.Types
open Elmish

type Step =
    | Names = 1
    | Actor = 2
    | Genre = 3
    | Decade = 4
    | Result = 5

module Step =
    let next (step: Step) = step |> int |> (+) 1 |> enum<Step>

type State =
    {
        Actor: Input<string>
        CurrentStep: Step
        Decade: Input<string * string>
        Genre: Input<string>
        Name: Input<string>
        Names: string list
    }

type Msg =
    | UserChangedName of string
    | UserChoseActor of string
    | UserChoseDecade of string * string
    | UserChoseGenre of string
    | UserClickedAddName of string
    | UserClickedDeleteName of string
    | UserClickedNext

let init (): State * Cmd<Msg> =
    let state =
        {
            Actor = Input.NotEditing
            CurrentStep = Step.Names
            Decade = Input.NotEditing
            Genre = Input.NotEditing
            Name = Input.NotEditing
            Names = []
        }

    state, Cmd.none

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | UserChangedName name -> { state with Name = Input.Editing name }, Cmd.none
    | UserChoseActor actor ->
        { state with
            Actor = Input.Editing actor
        },
        Cmd.none
    | UserChoseDecade (startY, endY) ->
        { state with
            Decade = Input.Editing(startY, endY)
        },
        Cmd.none
    | UserChoseGenre genre ->
        { state with
            Genre = Input.Editing genre
        },
        Cmd.none
    | UserClickedAddName name ->
        { state with
            Names = name :: state.Names
        },
        Cmd.none
    | UserClickedDeleteName name ->
        { state with
            Names = state.Names |> List.filter ((<>) name)
        },
        Cmd.none
    | UserClickedNext ->
        match state.CurrentStep with
        | Step.Decade -> state, Cmd.none // Todo: Make API Call
        | _ ->
            { state with
                CurrentStep = Step.next state.CurrentStep
            },
            Cmd.none

open Feliz
open Feliz.Bulma
open Client.Styles
open Client.Components

//let tagsInput =


let render (state: State) (dispatch: Msg -> unit) =
    Html.div [
        Bulma.hero [
            hero.isFullHeight
            prop.className Bulma.IsSuccess
            prop.children [
                Bulma.heroHead [
                    Navbar.render
                ]
                Bulma.heroBody [
                   Bulma.container [
                       prop.classes [ Bulma.HasTextCentered ]
                       prop.children [
                           Html.p [
                                prop.className Bulma.Title
                                prop.text "Step One:"
                           ]
                           Html.p [
                                prop.className Bulma.Subtitle
                                prop.text "Who's watching?"
                           ]
                       ]
                   ]
                ]
            ]
        ]
    ]
