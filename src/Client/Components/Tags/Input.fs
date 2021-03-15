namespace Client.Components.Tags

open Fable.Core
open Fable.Core.JsInterop

module private Props =
    let inline setDefault<'a> (name: string, value: obj) (props: List<'a>) =
        let found =
            props
            |> List.map unbox<string * _>
            |> List.exists (fun (n, _) -> n = name)

        match found with
        | true -> props
        | false -> (unbox (name, value)) :: props

[<Erase>]
type TagsInput =
    static member inline render (props: ITagsInputProperty list) =
        let props' =
            props
            |> Props.setDefault (Const.autoSource, [])
            |> Props.setDefault (Const.onlyAuto, false)
            |> Props.setDefault (Const.placeholder, "enter items...")
            |> Props.setDefault (Const.source, [])
            |> Props.setDefault (Const.updater, fun _ -> ())
            |> Props.setDefault (Const.delimiter, "Enter")

        TagsInputComponent.build (unbox<TagsInputComponent.Props> (createObj !!props'))