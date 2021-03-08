module Server.Program

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn

open Shared.ApiContract

open Server.Apis

let webApp =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue MoviesApi.value
    |> Remoting.buildHttpHandler

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

run app
