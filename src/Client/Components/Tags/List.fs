module Client.Components.Tags.List

open Feliz
open Feliz.Bulma

open Client.Styles

let render updater source =
    Bulma.tags [
        prop.classes [ Style.LargeTagHeight ]
        source
        |> List.map (Tag.render updater source)
        |> prop.children
    ]