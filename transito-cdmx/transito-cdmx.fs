open Suave                 // always open suave
open Suave.Successful      // for OK-result
open Suave.Web             // for config


[<EntryPoint>]
let main args =
  let port =  System.UInt16.Parse args.[0]
  let ip = System.Net.IPAddress.Parse "127.0.0.1"
  let serverConfig =
    { Web.defaultConfig with
        homeFolder = Some __SOURCE_DIRECTORY__
        logger = Logging.Loggers.saneDefaultsFor Logging.LogLevel.Warn
        bindings = [ HttpBinding.mk HTTP ip port ] }

  startWebServer serverConfig (OK "Hello World!")
  0