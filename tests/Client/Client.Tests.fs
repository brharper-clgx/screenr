module Client.Tests

open Fable.Mocha

open Shared

let client = testList "Client" []

let all =
    testList "All"
        [
#if FABLE_COMPILER // This preprocessor directive makes editor happy
            Shared.Tests.All.all
#endif
            client
        ]

[<EntryPoint>]
let main _ = Mocha.runTests all