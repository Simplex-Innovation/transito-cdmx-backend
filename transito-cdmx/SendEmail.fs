module Email

open System.Net.Mail
open System.IO

let send origin (password:string) server port destination subject message =
    use msg = new MailMessage(origin, destination, subject, message)
    use client = new SmtpClient(server, port)
    client.EnableSsl <- true
    client.Credentials <- new System.Net.NetworkCredential(origin, password)
    client.Send msg