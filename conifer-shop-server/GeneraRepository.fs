namespace ConiferShop.Repositories

[<AutoOpen>]
module GeneraRepository =
  open ConiferShop.Db
  open System
  open ConiferShop.Rest

  let getAllGenera() =
    generaStorage.Values :> seq<Entity<Genus>> // [ :> ] - converts a type to type that is higher in the hierarchy.
    
  let getGenus id =
    match generaStorage.ContainsKey(id) with
    | true -> Some (generaStorage.Item(id))
    | false -> None

  let createGenus (genus: Genus) =
    let newGenusEntity = {LastModified=DateTime.Now; Data={genus with Id = nextUniqId generaStorage}}
    generaStorage.Add(newGenusEntity.Data.Id, newGenusEntity);
    newGenusEntity
  
  let updateGenus id _ _ (genus: Genus) =
    if generaStorage.ContainsKey(id) then
      let updatedGenus = {LastModified=DateTime.Now; Data={genus with Id = id}} 
      generaStorage.Item(id) <- updatedGenus
      Updated updatedGenus
    else
      NotFound

  let deleteGenus id =
    generaStorage.Remove(id) |> ignore

  let genusExists = generaStorage.ContainsKey


