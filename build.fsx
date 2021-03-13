#r "paket: groupref build //"
#load "./.fake/build.fsx/intellisense.fsx"
#r "netstandard"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Farmer
open Farmer.Builders

Target.initEnvironment ()

let sourcePath = Path.getFullName "./src"
let sharedPath = Path.getFullName "./src/Shared"
let serverPath = Path.getFullName "./src/Server"
let deployDir = Path.getFullName "./deploy"
let sharedTestsPath = Path.getFullName "./tests/Shared"
let serverTestsPath = Path.getFullName "./tests/Server"
let clientPath = Path.combine sourcePath "Client"
let clientPublicPath = Path.combine clientPath "public"

let buildRawCmd cmd args workingDir =
    let path =
        match ProcessUtils.tryFindFileOnPath cmd with
        | Some path -> path
        | None ->
            sprintf "%s was not found in path. Please install and add it to path." cmd
            |> failwith

    RawCommand(path, args |> String.split ' ' |> Arguments.OfArgs)
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory workingDir

let execRawHandled cmd args workingDir =
    buildRawCmd cmd args workingDir
    |> CreateProcess.ensureExitCode
    |> Proc.run
    |> ignore

let execRaw cmd args workingDir =
    buildRawCmd cmd args workingDir |> Proc.run

let npm args workingDir =
    let npmPath =
        match ProcessUtils.tryFindFileOnPath "npm" with
        | Some path -> path
        | None ->
            "npm was not found in path. Please install it and make sure it's available from your path. "
            + "See https://safe-stack.github.io/docs/quickstart/#install-pre-requisites for more info"
            |> failwith

    let arguments =
        args |> String.split ' ' |> Arguments.OfArgs

    Command.RawCommand(npmPath, arguments)
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory workingDir
    |> CreateProcess.ensureExitCode
    |> Proc.run
    |> ignore

let dotnet cmd workingDir =
    let result =
        DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""

    if result.ExitCode <> 0
    then failwithf "'dotnet %s' failed in %s" cmd workingDir

let sass input output =
    let args = sprintf "%s %s" input output
    execRawHandled "sass" args clientPublicPath

Target.create "Clean" (fun _ -> Shell.cleanDir deployDir)

Target.create "InstallClient" (fun _ -> npm "install" clientPath)

Target.create "Bundle" (fun _ ->
    dotnet (sprintf "publish -c Release -o \"%s\"" deployDir) serverPath
    npm "run build" clientPath)

Target.create "Sass"
<| fun _ -> sass "styles.scss" "styles.css"


Target.create "Azure" (fun _ ->
    let web =
        webApp {
            name "Screenr"
            app_insights_off
            zip_deploy "deploy"
        }

    let deployment =
        arm {
            location Location.CentralUS
            add_resource web
        }

    deployment
    |> Deploy.execute "Screenr" Deploy.NoParameters
    |> ignore)

Target.create "Watch" (fun _ ->
    dotnet "build" sharedPath

    [
        async { dotnet "watch run" serverPath }
        async { npm "run start" clientPath }
    ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore)

Target.create "WatchTests" (fun _ ->
    dotnet "build" sharedTestsPath

    [
        async { dotnet "watch run" serverTestsPath }
        async { npm "run test:live" clientPath }
    ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore)

Target.create "RunTests" (fun _ ->
    dotnet "build" sharedTestsPath

    [
        async { dotnet "run" serverTestsPath }
        async { npm "run test" clientPath }
    ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore)

open Fake.Core.TargetOperators

"Clean" ==> "InstallClient" ==> "Sass" ==> "Bundle"

"InstallClient" ==> "Azure"

"Bundle" ==> "Azure"

"Sass" ==> "Watch"

"InstallClient" ==> "WatchTests"

Target.runOrDefault "List"
