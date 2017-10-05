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

    let setETagHeader entity =
      setHeader eTagHeaderName (entity.GetHashCode().ToString())

    let setLasModifiedHeader entity =
      setHeader lastModifiedHeaderName (entity.LastModified.ToString(System.Globalization.CultureInfo.InvariantCulture))

    type RestResource<'a> = {
        GetAll : unit -> Entity<'a> seq
        GetById : int -> Entity<'a> option
        IsExists : int -> bool
        Create : 'a -> Entity<'a>
        UpdateById : int -> 'a -> Entity<'a> option
        Delete : int -> unit
    }

    let rest resourcePath resource =
        let resourceIdPath = new PrintfFormat<(int -> string),unit,string,string,int>(resourcePath + "/%d")
        
        let notFoundResponse = NOT_FOUND "Resource not found"

        let handleResource requestError option =
            match option with
            | Some r -> 
              setETagHeader r >=> setLasModifiedHeader r >=> toJson (r.Data)
            | None -> requestError
        
        let getAll =
          let getData entity =
            entity.Data
          warbler(fun _ -> resource.GetAll() |> Seq.map getData |> toJson)

        let getResourceById = 
            resource.GetById >> handleResource notFoundResponse
        
        let updateResourceById id =
          request (getResourceFromReq >> resource.UpdateById id >> handleResource notFoundResponse)

        let deleteResourceById id =
            resource.Delete id
            NO_CONTENT

        let isResourceExists id =
            if resource.IsExists id then OK "" else notFoundResponse

        let getResourceId = pathScan resourceIdPath

        choose [
            path resourcePath >=> choose [
                GET >=> getAll
                POST >=> request (getResourceFromReq >> resource.Create >> (fun e -> e.Data) >> toJson)
            ]
            DELETE >=> getResourceId deleteResourceById
            GET >=> getResourceId getResourceById
            PUT >=> getResourceId updateResourceById
            HEAD >=> getResourceId isResourceExists
        ]
