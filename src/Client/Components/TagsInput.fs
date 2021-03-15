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
    let autoSource = "autoCompleteSource"
    let onlyAuto = "allowOnlyAutoCompleteValues"
    let delimiter = "delimiter"

type ITagsInputProperty =
    interface
    end

[<Erase>]
type tagsInput =
    static member inline updater(fn: string list -> unit): ITagsInputProperty = unbox (Const.updater, fn)
    static member inline autoCompleteSource(src: string list): ITagsInputProperty = unbox (Const.autoSource, src)
    static member inline allowOnlyAutoCompleteValues(value: bool): ITagsInputProperty = unbox (Const.onlyAuto, value)
    static member inline source(tags: string list): ITagsInputProperty = unbox (Const.source, tags)
    static member inline delimiter (value:string) : ITagsInputProperty = unbox (Const.delimiter, value)
    static member inline placeholder(value: string): ITagsInputProperty = unbox (Const.placeholder, value)
//    static member inline allowDuplicates (value:bool) : ITagsInputProperty = unbox ("allowDuplicates", value)

module private Props =
    let inline setDefault<'a> (name: string, value: obj) (props: List<'a>) =
        let found =
            props
            |> List.map unbox<string * _>
            |> List.exists (fun (n, _) -> n = name)

        match found with
        | true -> props
        | false -> (unbox (name, value)) :: props

    let inline get<'a> name (props: ITagsInputProperty list): 'a =
        props
        |> List.map unbox<string * _>
        |> List.find (fun (n, _) -> n = name)
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

    let autoOp (v: string) = Html.option [ prop.value v ]

    let private build updater placeholder autoSource (onlyAuto: bool) delimiter source =
        let id = Guid.NewGuid() |> sprintf "%A"
        let autoId = sprintf "auto-%s" id
        let mutable currentVal = ""
        let isUpdateKey key = key = delimiter || key = "Enter"

        let onKeyDown (ke: KeyboardEvent) =
            match isUpdateKey ke.key, onlyAuto with
            | (true, false) -> [ currentVal ] |> List.append source |> updater
            | (true, true) ->
                if autoSource |> List.contains currentVal
                then [ currentVal ] |> List.append source |> updater
            | _ -> ()


        let onKeyUp (ke: KeyboardEvent) =
            if isUpdateKey ke.key then
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
                    prop.custom ("list", autoId)
                    prop.onChange (fun v -> currentVal <- v)
                    prop.onKeyDown onKeyDown
                    prop.onKeyUp onKeyUp
                ]
            ]
            source |> tagList updater

            Html.datalist [
                prop.id autoId
                autoSource |> List.map autoOp |> prop.children
            ]
        ]

    let render (props: ITagsInputProperty list) =
        let props' =
            props
            |> Props.setDefault (Const.autoSource, [])
            |> Props.setDefault (Const.onlyAuto, false)
            |> Props.setDefault (Const.placeholder, "enter items...")
            |> Props.setDefault (Const.source, [])
            |> Props.setDefault (Const.updater, fun _ -> ())
            |> Props.setDefault (Const.delimiter, "Enter")

        let source =
            props' |> Props.get<string list> Const.source

        let updater =
            props'
            |> Props.get<string list -> unit> Const.updater

        let placeholder =
            props' |> Props.get<string> Const.placeholder

        let autoSrc =
            props' |> Props.get<string list> Const.autoSource

        let onlyAuto = props' |> Props.get<bool> Const.onlyAuto

        let delimiter = props' |> Props.get<string> Const.delimiter

        build updater placeholder autoSrc onlyAuto delimiter source
