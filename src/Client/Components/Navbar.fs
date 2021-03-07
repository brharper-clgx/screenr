module Client.Components.Navbar

open Feliz
open Feliz.Bulma
open Client.Styles

let render =
    Bulma.navbar [
        Bulma.navbar.isFixedTop
        prop.children [
            Bulma.navbarBrand.div [
                Bulma.navbarItem.a [
                    prop.href "#"
                    prop.children [
                        Html.img [
                            prop.src "/favicon.png"
                        ]
                        Html.p [
                            prop.classes [ Bulma.IsSize5; Bulma.Px1 ]
                            prop.text "Screenr"
                        ]
                    ]
                ]
            ]
        ]

    ]