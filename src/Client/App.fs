module App

open Elmish
open Elmish.React

open Client.Pages

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram Home.Component.init Home.Component.update Home.View.render
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
