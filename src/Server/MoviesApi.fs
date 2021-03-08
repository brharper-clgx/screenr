namespace Server.Apis

open Shared.ApiContract

module MoviesApi =
    let value =
        {
          GetMovie = fun details ->

              async { return "28 Weeks Later" }
        }