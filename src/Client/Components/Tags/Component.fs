namespace Client.Components.Tags

open System
open Fable.Core
open Browser.Types
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

[<RequireQualifiedAccess; System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
module TagsInputComponent =
    type Props =
        abstract updater : (string list -> unit)
        abstract source: string list
        abstract delimiter: string
        abstract placeholder: string
        abstract autoCompleteSource: string list
        abstract allowOnlyAutoCompleteValues: bool
//        abstract allowDuplicates: bool

    [<Global>]
    let private document: Document = jsNative

    let private autoOp (v: string) = Html.option [ prop.value v ]

    let build (props: Props) =
        let id = Guid.NewGuid() |> sprintf "%A"
        let autoId = sprintf "auto-%s" id
        let mutable currentVal = ""
        let isUpdateKey key = key = props.delimiter || key = "Enter"

        let onKeyDown (ke: KeyboardEvent) =
            match isUpdateKey ke.key, props.allowOnlyAutoCompleteValues with
            | (true, false) -> [ currentVal ] |> List.append props.source |> props.updater
            | (true, true) ->
                if props.autoCompleteSource |> List.contains currentVal
                then [ currentVal ] |> List.append props.source |> props.updater
            | _ -> ()


        let onKeyUp (ke: KeyboardEvent) =
            if isUpdateKey ke.key then
                let input =
                    document.getElementById id :?> HTMLInputElement

                input.value <- ""

        Html.div [
            Bulma.input.text [
                prop.id id
                prop.classes [ Bulma.Mb1 ]
                input.isLarge
                prop.placeholder props.placeholder
                prop.custom ("list", autoId)
                prop.onChange (fun v -> currentVal <- v)
                prop.onKeyDown onKeyDown
                prop.onKeyUp onKeyUp
            ]

            props.source |> List.render props.updater

            Html.datalist [
                prop.id autoId
                props.autoCompleteSource |> List.map autoOp |> prop.children
            ]
        ]
