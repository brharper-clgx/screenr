module Shared.Decades

let all =
    [ 1910 .. 10 .. 2020 ] |> List.sortDescending

let allLabels = all |> List.map (sprintf "%is")

let labelToRange (label: string) =
    label
    |> String.filter (fun c -> c <> 's')
    |> int
    |> fun d -> d, d + 10
