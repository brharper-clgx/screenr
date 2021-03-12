module Shared.Extensions.List

open System

let shuffle list =
    let random = Random(DateTime.Now.Millisecond)
    list |> List.sortBy (fun _ -> random.Next list.Length)