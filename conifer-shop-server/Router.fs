namespace ConiferShop

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful
open Suave.Headers

[<AutoOpen>]
module Router =
  open ConiferShop.Rest
  open ConiferShop.Repositories
  open System.Net.Http.Headers
  
  let conifersWebPart = rest "/conifers" {
    GetAll = getAllConifers
    GetById = getConifer
    Create = createConifer 
    UpdateById = updateConifer
    Delete = deleteConifer
    IsExists = coniferExists
  }

  let appRouting =
    choose [
      OPTIONS >=> NO_CONTENT
      conifersWebPart
    ]



