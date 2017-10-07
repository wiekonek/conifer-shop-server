namespace ConiferShop.Rest

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave
open Suave.Operators
open Suave.Http
open Suave.RequestErrors
open Suave.Filters
open Suave.Writers
open ConiferShop.Db
open Suave.Successful

[<AutoOpen>]
module Restful =    
    open System
    open Microsoft.FSharp.Core.LanguagePrimitives
    open Suave.Headers


    let eTagHeaderName = "ETag"
    let eTagIfHeaderName = "If-Match"

    let lastModifiedHeaderName = "Last-Modified"
    let ifUnmodifiedSinceHeaderName = "If-Unmodified-Since"
   
    
    let toJson v =     
        let jsonSerializerSettings = new JsonSerializerSettings()
        jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
    
        JsonConvert.SerializeObject(v, jsonSerializerSettings)
        |> OK >=> Writers.setMimeType "application/json; charset=utf-8"

    let fromJson<'a> json =
        JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a    

    let getResourceFromReq<'a> (req : HttpRequest) = 
        let getString rawForm = System.Text.Encoding.UTF8.GetString(rawForm)
        req.rawForm |> getString |> fromJson<'a>

    type UpdateRequest<'a> = {
      ETag: int;
      LastModified: DateTime;
      Resource: 'a;
    }

    let getUpdateFromReq<'a> (req : HttpRequest) = 
      let getString rawForm = System.Text.Encoding.UTF8.GetString(rawForm)
      {
        ETag =
          match req.header eTagIfHeaderName with 
          | Choice1Of2 h -> ParseInt32 h
          | Choice2Of2 e -> 0;
        LastModified = 
          match req.header ifUnmodifiedSinceHeaderName with
          | Choice1Of2 h -> new DateTime(ParseInt64 h)
          | Choice2Of2 e -> DateTime.UtcNow;
        Resource = req.rawForm |> getString |> fromJson<'a>
      }
        

    let setETagHeader entity =
      setHeader eTagHeaderName (entity.GetHashCode().ToString())

    let setLasModifiedHeader (entity: Entity<'a>) =
      setHeader lastModifiedHeaderName (entity.LastModified.Ticks.ToString())

    type UpdateResult<'a> =
    | Updated of 'a
    | PreconditionFailed
    | NotFound

    type RestResource<'a> = {
        GetAll : unit -> Entity<'a> seq
        GetById : int -> Entity<'a> option
        IsExists : int -> bool
        Create : 'a -> Entity<'a>
        UpdateById : int -> int -> DateTime  -> 'a  -> UpdateResult<Entity<'a>>
        Delete : int -> unit
    }
    
    let getData entity =
      entity.Data

    let rest resourcePath resource =
        let resourceIdPath = new PrintfFormat<(int -> string),unit,string,string,int>(resourcePath + "/%d")
        
        let notFoundResponse = NOT_FOUND "Resource not found"

        let handleUpdate result =
          match result with
          | NotFound -> notFoundResponse
          | PreconditionFailed -> PRECONDITION_REQUIRED "Precondition Failed"
          | Updated r -> setETagHeader r >=> setLasModifiedHeader r >=> toJson (r.Data) 
        

        let getAll =
          warbler(fun _ -> resource.GetAll() |> Seq.map getData |> toJson)

        let deleteResourceById id =
            resource.Delete id
            NO_CONTENT

        let getResourceById id = 
          match resource.GetById id with
          | Some r ->
            setETagHeader r >=> setLasModifiedHeader r >=> toJson (r.Data)
          | None -> notFoundResponse
        
        let updateResourceById id =
          let update updateReq =
            resource.UpdateById id updateReq.ETag updateReq.LastModified updateReq.Resource
          request (getUpdateFromReq >> update >> handleUpdate)

        let isResourceExists id =
            if resource.IsExists id then OK "" else notFoundResponse

        let getResourceId = pathScan resourceIdPath

        choose [
            path resourcePath >=> choose [
                GET >=> getAll
                POST >=> request (getResourceFromReq >> resource.Create >> getData >> toJson)
            ]
            DELETE >=> getResourceId deleteResourceById
            GET >=> getResourceId getResourceById
            PUT >=> getResourceId updateResourceById
            HEAD >=> getResourceId isResourceExists
        ]
