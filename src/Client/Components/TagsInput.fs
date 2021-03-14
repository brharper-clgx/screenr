namespace Client.Components

open System
open Browser.Types
open Fable.Core
open Feliz
open Feliz.Bulma

open Client.Styles


module Const =
    let updater = "updater"
    let source = "source"
    let placeholder = "placeholder"

type ITagsInputProperty = interface end

[<Erase>]
type tagsInput =
    static member inline updater (fn:string list -> unit) : ITagsInputProperty = unbox (Const.updater, fn)
//    static member inline autoCompleteSource (src:string -> Async<string list>) : ITagsInputProperty = unbox ("autoCompleteSource", src)
    static member inline source (tags:string list) : ITagsInputProperty = unbox (Const.source, tags)
//    static member inline delimiter (value:char) : ITagsInputProperty = unbox ("delimiter", value)
    static member inline placeholder (value:string) : ITagsInputProperty = unbox (Const.placeholder, value)
//    static member inline allowDuplicates (value:bool) : ITagsInputProperty = unbox ("allowDuplicates", value)
//    static member inline allowOnlyAutoCompleteValues (value:bool) : ITagsInputProperty = unbox ("allowOnlyAutoCompleteValues", value)

module private Props =
    let inline setDefault<'a> (name:string, value:obj) (props:List<'a>) =
        let found =
            props
            |> List.map unbox<string * _>
            |> List.exists (fun (n,_) -> n = name)
        match found with
        | true -> props
        | false -> (unbox (name, value)) :: props

    let inline get<'a> name (props:ITagsInputProperty list): 'a =
        props
        |> List.map unbox<string * _>
        |> List.find (fun (n,_) -> n = name)
        |> snd

module TagsInput =

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

    let private build updater placeholder source =
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
                    prop.placeholder placeholder
                    prop.onChange (fun v -> currentVal <- v)
                    prop.onKeyDown onKeyDown
                    prop.onKeyUp onKeyUp
                ]
            ]
            source |> tagList updater
        ]

    let render (props: ITagsInputProperty list) =
        let props' =
            props
            |> Props.setDefault (Const.source, [])
            |> Props.setDefault (Const.updater, fun _ -> ())
            |> Props.setDefault (Const.placeholder, "enter items...")

        let source = props' |> Props.get<string list> Const.source
        let updater = props' |> Props.get<string list -> unit> Const.updater
        let placeholder = props' |> Props.get<string> Const.placeholder

        build updater placeholder source
