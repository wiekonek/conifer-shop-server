﻿namespace ConiferShop

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful
open Suave.Headers

[<AutoOpen>]
module Router =
  open ConiferShop.Rest
  open ConiferShop.Repositories
  


  let conifersWebPart = rest "/conifers" {
    GetAll = getAllConifers
    GetById = getConifer
    Create = createConifer 
    UpdateById = updateConifer
    Delete = deleteConifer
    IsExists = coniferExists
  }

  let generaWebPart = rest "/genera" {
    GetAll = getAllGenera
    GetById = getGenus
    Create = createGenus
    UpdateById = updateGenus
    Delete = deleteGenus
    IsExists = genusExists
  }

  let shopsWebPart = rest "/shops" {
    GetAll = getAllShops
    GetById = getShop
    Create = createShop
    UpdateById = updateShop
    Delete = deleteShop
    IsExists = shopExists
  }




  let appRouting =
    choose [
      OPTIONS >=> NO_CONTENT
      conifersShopsWebPart
      conifersWebPart
      generaWebPart
      shopsWebPart
    ]



