module SuaveApp

open System
open System.Runtime.Serialization
open System.IO
open System.Text

open Suave                 // always open suave
open Suave.Successful      // for OK-result
open Suave.Web             // for config
open Suave.Json
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
open Suave.Writers

let loadString path =
    File.ReadAllText path

let loadBytes path =
    File.ReadAllBytes path

let save64 path bytes =
    File.WriteAllText(path, Convert.ToBase64String bytes)

let loadBytesFrom64 path =
    File.ReadAllText path
    |> Convert.FromBase64String

let transitocdmx =
    request (fun r ->
        match r.queryParam "placa" with
        | Choice2Of2 msg -> BAD_REQUEST msg
        | Choice1Of2 plate ->
            let email =
                match r.queryParam "email" with
                | Choice1Of2 email -> email
                | Choice2Of2 _ -> ""
            OK (TransitCDMX.callApi plate email)
            >=> setMimeType "application/json; charset-utf-8"
    )

let webPart = 
    choose [
        path "/" >=> (OK "SPLXI")
        path "/transitocdmx" >=> transitocdmx
    ]

[<EntryPoint>]
let main args =
  let port =  System.UInt16.Parse args.[0]
  let ip = System.Net.IPAddress.Parse "127.0.0.1"
  let serverConfig =
    { Web.defaultConfig with
        homeFolder = Some __SOURCE_DIRECTORY__
        logger = Logging.Loggers.saneDefaultsFor Logging.LogLevel.Warn
        bindings = [ HttpBinding.mk HTTP ip port ] }

  startWebServer serverConfig webPart
  0