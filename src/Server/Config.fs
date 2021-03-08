module Server.Config

open Microsoft.Extensions.Configuration

[<CLIMutable>]
type Config = {
    ApiKey: string
}

type IConfigProvider =
    abstract GetApiKey: unit -> string

type ConfigProvider(config: IConfiguration) =
     interface IConfigProvider with
        member this.GetApiKey () = config.Get<Config>().ApiKey