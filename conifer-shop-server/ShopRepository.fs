namespace ConiferShop.Repositories

[<AutoOpen>]
module ShopRepository =
  open ConiferShop.Db
  open System
  open ConiferShop.Rest

  type PaginationQuery = {
    Offset: int;
    Limit: int;
  }

  type Pagination = {
    Offset: int;
    Limit: int;
    Total: int;
  }

  type PagedResponse<'a> = {
    Data: 'a;
    Pagination: Pagination;
  }

  let getAllShops() =
    shopStorage.Values :> seq<Entity<Shop>>

  let getAllShopsWithConifer coniferId (pagination: PaginationQuery option) =
    match pagination with 
    | Some p ->
      let sliceArray (arr: int[]) =
        let limitToArraySize value =
          if value > arr.Length-1 then arr.Length-1 else value 
        arr.[limitToArraySize p.Offset..limitToArraySize (p.Offset+p.Limit-1)]

      shopsPerConiferStorage.Item(coniferId) 
      |> sliceArray
      |> Seq.map (fun shopId -> shopStorage.Item(shopId).Data)
      |> (fun arr -> { Data=arr; Pagination={ Offset=p.Offset; Limit=p.Limit; Total=shopsPerConiferStorage.Item(coniferId).Length}})
    | None -> 
      shopsPerConiferStorage.Item(coniferId)
      |> Seq.map (fun shopId -> shopStorage.Item(shopId).Data)
      |> (fun arr -> { Data=arr; Pagination={ Offset=0; Limit=0; Total=shopsPerConiferStorage.Item(coniferId).Length}})
    
  let getShop id =
    match shopStorage.ContainsKey(id) with
    | true -> Some (shopStorage.Item(id))
    | false -> None

  let createShop (conifer: Shop) =
    let newShopEntity = {LastModified=DateTime.Now; Data={conifer with Id = nextUniqId shopStorage}}
    shopStorage.Add(newShopEntity.Data.Id, newShopEntity)
    shopsPerConiferStorage.Add(newShopEntity.Data.Id, [||])
    newShopEntity
  
  let updateShop id eTag lastModified (shop: Shop) =
    if shopStorage.ContainsKey(id) then
      if( shopStorage.Item(id).LastModified.CompareTo(lastModified) = 0 &&  shopStorage.Item(id).GetHashCode() = eTag) then
        let updatedShop = {LastModified=DateTime.Now; Data={shop with Id = id}} 
        shopStorage.Item(id) <- updatedShop
        Updated updatedShop
      else
        PreconditionFailed
    else
      NotFound

  let deleteShop id =
    shopsPerConiferStorage.Remove(id) |> ignore
    shopStorage.Remove(id) |> ignore

  let shopExists = shopStorage.ContainsKey