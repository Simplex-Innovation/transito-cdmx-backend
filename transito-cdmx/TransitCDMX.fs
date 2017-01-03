module TransitCDMX

open System
open System.Text
open System.IO
open System.Net
open System.Collections.Generic
open FSharp.Data
open FSharp.Data.HttpRequestHeaders

type TransitoSchema = JsonProvider<"http://40.84.146.161:8080/sample.json">

let callApi placa email =
    let loaded = TransitoSchema.Load("http://40.84.146.161:3000/api/v1/vehiculos/" + placa)
    let withEmail =
        TransitoSchema.Root(email,loaded.AdeudosTenencias,
          loaded.Infracciones,loaded.MontoAdeudoTenencias,loaded.MontoAdeudoInfracciones,loaded.MontoTotalAdeudos)
    //loaded.Placa,loaded.Modelo,loaded.NumCilindros, loaded.ProcedenciaNacional,loaded.ValorFactura,loaded.FechaFactura,loaded.Depreciacion,loaded.DepreciacionRestante,

    if email <> ""
    then
        let bytes = Encoding.Default.GetBytes(withEmail.JsonValue.ToString())
        let convertedBytes = Encoding.Convert(Encoding.UTF8,Encoding.ASCII, bytes)
        let encodedJson = Encoding.ASCII.GetString(convertedBytes)

        let subject = "Información sobre placa: " + placa
        let message = encodedJson
        Email.send "contacto@splxi.com" "Csplxi12#" "smtp.office365.com" 587 email subject message
    withEmail.JsonValue.ToString()