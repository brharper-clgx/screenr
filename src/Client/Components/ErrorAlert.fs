module Client.Components.ErrorAlert

open Feliz
open Feliz.Bulma
open Client.Styles

let render dismiss (body: string) =
    Bulma.message [
        prop.classes [ Bulma.IsDanger]
        prop.children [
            Bulma.messageHeader [
                Html.p "There was an error with your request...."
                Bulma.delete [
                    prop.onClick dismiss
                ]
            ]
            Bulma.messageBody body
        ]
    ]
