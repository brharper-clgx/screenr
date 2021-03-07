module Shared.Extensions.String

open Shared

let contains (content: string) (container: string) =
    container.Contains(content)

let containsIgnoreCase (content: string) (container: string) =
    container |> Casing.toLower |> contains (Casing.toLower content)

module Operators =
    /// <summary>
    /// Returns true iff content is in container string, case insensitive.
    /// </summary>
    /// <param name="container">container</param>
    /// <param name="content">content</param>
    let (|<~) (container: string) (content: string) = container |> containsIgnoreCase content

    /// <summary>Returns true iff exact content is in container string.</summary>
    /// <param name="container">Container</param>
    /// <param name="content">Content</param>
    let (|<-) (container: string) (content: string) = container |> contains content
