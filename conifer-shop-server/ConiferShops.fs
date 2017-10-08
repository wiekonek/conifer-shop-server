namespace ConiferShop

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful
open Suave.Headers

[<AutoOpen>]
module ConiferShop =
  open ConiferShop.Repositories
  open ConiferShop.Rest
  open Microsoft.FSharp.Core.LanguagePrimitives

  let resourceIdPath = new PrintfFormat<(int -> string),unit,string,string,int>("/conifers/%d/shops")
  
  let getAllShopsForConifer coniferId =
    let handler (req : HttpRequest) =
      let queryParams =
        match (req.queryParam "offset", req.queryParam "limit") with
        | Choice1Of2 o, Choice1Of2 l -> Some { Offset = ParseInt32 o; Limit = ParseInt32 l }
        | _ -> None
      getAllShopsWithConifer coniferId queryParams |> toJson
    request(handler)



  let conifersShopsWebPart = 
    choose [
        //path resourcePath >=> choose [
        //    GET >=> getAll
        //    POST >=> request (getResourceFromReq >> resource.Create >> getData >> toJson)
        //]
        GET >=> pathScan resourceIdPath getAllShopsForConifer
        //GET >=> getResourceId getResourceById
        //PUT >=> getResourceId updateResourceById
        //HEAD >=> getResourceId isResourceExists
    ]



