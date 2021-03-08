namespace Server.Configuration

open Microsoft.Extensions.Configuration

[<CLIMutable>]
type Config = {
    ApiKey: string
}