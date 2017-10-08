namespace ConiferShop.Repositories

[<AutoOpen>]
module ConifersRepository =
  open ConiferShop.Db
  open System
  open ConiferShop.Rest

  let getAllConifers() =
    conifersStorage.Values :> seq<Entity<Conifer>> // [ :> ] - converts a type to type that is higher in the hierarchy.
    
  let getConifer id =
    match conifersStorage.ContainsKey(id) with
    | true -> Some (conifersStorage.Item(id))
    | false -> None

  let createConifer (conifer: Conifer) =
    let newConiferEntity = {LastModified=DateTime.Now; Data={conifer with Id = nextUniqId conifersStorage}}
    conifersStorage.Add(newConiferEntity.Data.Id, newConiferEntity);
    newConiferEntity
  
  let updateConifer id eTag lastModified (conifer: Conifer) =
    if conifersStorage.ContainsKey(id) then
      let currentHash = conifersStorage.Item(id).GetHashCode()
      let result = conifersStorage.Item(id).LastModified.CompareTo(lastModified)
      if( conifersStorage.Item(id).LastModified.CompareTo(lastModified) = 0 && currentHash = eTag) then
        let updatedConifer = {LastModified=DateTime.Now; Data={conifer with Id = id}} 
        conifersStorage.Item(id) <- updatedConifer
        Updated updatedConifer
      else
        PreconditionFailed
    else
      NotFound

  let deleteConifer id =
    conifersStorage.Remove(id) |> ignore

  let coniferExists = conifersStorage.ContainsKey


