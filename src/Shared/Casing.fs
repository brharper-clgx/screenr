module Shared.Casing

let inline toUpper (value: string) = value.ToUpperInvariant()

let inline toLower (value: string) = value.ToLowerInvariant()
