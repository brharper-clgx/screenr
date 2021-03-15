module Client.Components.Tags.Tag

open Feliz
open Feliz.Bulma

open Client.Styles

let render (updater: string list -> unit) (source: string list) (item: string) =
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