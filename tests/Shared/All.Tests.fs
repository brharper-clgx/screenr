module Shared.Tests.All

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

let all = testList "Shared" [
    Casing.all
]