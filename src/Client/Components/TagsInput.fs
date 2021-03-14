module Client.Components.TagsInput

open System
open Browser.Types
open Fable.Core
open Feliz
open Feliz.Bulma

open Client.Styles

[<Global>]
let private document: Document = jsNative

let private tag (updater: string list -> unit) (source: string list) (item: string) =
    Bulma.tag [
        tag.isLarge
        prop.classes [ Bulma.IsPrimary ]
        prop.children [
            Html.text item
            Html.button [
                prop.classes [
                    Bulma.IsSmall
                    Bulma.Delete
                ]
                prop.onClick (fun _ -> source |> List.filter ((<>) item) |> updater)
            ]
        ]
    ]

let private tagList updater source =
    Bulma.tags [
        prop.classes [ Style.LargeTagHeight ]
        source
        |> List.map (tag updater source)
        |> prop.children
    ]

let render (updater: string list -> unit) (source: string list) =
    let id = Guid.NewGuid() |> sprintf "%A"
    let mutable currentVal = ""

    let onKeyDown (ke: KeyboardEvent) =
        if ke.key = ","
        then [ currentVal ] |> List.append source |> updater

    let onKeyUp (ke: KeyboardEvent) =
        if ke.key = "," then
            let input =
                document.getElementById id :?> HTMLInputElement

            input.value <- ""

    Html.div [
        Bulma.control.div [
            Bulma.input.text [
                prop.id id
                prop.classes [ Bulma.Mb1 ]
                input.isLarge
                prop.placeholder "Bob, Sue, Ted"
                prop.onChange (fun v -> currentVal <- v)
                prop.onKeyDown onKeyDown
                prop.onKeyUp onKeyUp
            ]
        ]
        source |> tagList updater
    ]

