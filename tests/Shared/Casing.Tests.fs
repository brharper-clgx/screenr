module Shared.Tests.Casing

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open FsCheck
open Shared

#if !FABLE_COMPILER
let properties = testList "Casing: Property Tests" [

    testProperty "Casing.toLower: result is always the same as ToLowerInvariant()"
    <| fun (NonNull (str: string)) ->
        // arrange
        let expected = str.ToLowerInvariant()

        // act
        let result = Casing.toLower str

        // assert
        expected = result

    testProperty "Casing.toUpper: result is always the same as ToUpperInvariant()"
    <| fun (NonNull (str: string)) ->
        // arrange
        let expected = str.ToUpperInvariant()

        // act
        let result = Casing.toUpper str

        // assert
        expected = result

]
#endif

let standard = testList "Casing: Standard Tests" [

    testCase "Casing.toLower: result is always the same as ToLowerInvariant()"
    <| fun _ ->
        // arrange
        [ "ALL CAPS"; "sOmE cApS"; "all lower" ]

        //act
        |> List.map (fun str -> (Casing.toLower str, str.ToLowerInvariant()))

        // assert
        |> List.iter (fun (actual, expected) -> Expect.equal actual expected "")

    testCase "Casing.toUpper: result is always the same as ToUpperInvariant()"
    <| fun _ ->
        // arrange
        [ "ALL CAPS"; "sOmE cApS"; "all lower" ]

        //act
        |> List.map (fun str -> (Casing.toUpper str, str.ToUpperInvariant()))

        // assert
        |> List.iter (fun (actual, expected) -> Expect.equal actual expected "")

]

let all = testList "Casing" [

    standard

#if !FABLE_COMPILER
    properties
#endif

]
