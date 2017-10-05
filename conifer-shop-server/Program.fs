namespace ConiferShop

open System.Threading
open System
open Suave

open Router

module Server = 

  [<EntryPoint>]
  let main argv = 
      let cts = new CancellationTokenSource()
      let conf = { defaultConfig with cancellationToken = cts.Token }
      let listening, server = startWebServerAsync conf appRouting
    
      Async.Start(server, cts.Token)
  
      printfn "Press any key to close server..."
      Console.ReadKey true |> ignore
      cts.Cancel()

      0 // return an integer exit code