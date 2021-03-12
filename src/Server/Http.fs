module Server.Http

module HttpClient =
    let get (url:string) =
        async {
            let httpClient = new System.Net.Http.HttpClient()
            let! response = httpClient.GetAsync(url) |> Async.AwaitTask
            let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            return response.StatusCode |> int, content
        }

let (|Success|Redirect|ClientError|ServerError|InvalidStatusCode|) statusCode =
    match statusCode with
    | sc when sc >= 200 && sc < 300 -> Success
    | sc when sc >= 300 && sc < 400 -> Redirect
    | sc when sc >= 400 && sc < 500 -> ClientError
    | sc when sc >= 500 && sc < 600 -> ServerError
    | _ -> InvalidStatusCode