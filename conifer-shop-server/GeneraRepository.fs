namespace ConiferShop.Repositories

[<AutoOpen>]
module GeneraRepository =
  open ConiferShop.Db
  open System

  let getAllGenera() =
    conifersStorage.Values :> seq<Entity<Conifer>> // [ :> ] - converts a type to type that is higher in the hierarchy.
    
  let getGenre id =
    match conifersStorage.ContainsKey(id) with
    | true -> Some conifersStorage.[id]
    | false -> None

  let createGenre (conifer: Conifer) =
    let newConiferEntity = {LastModified=DateTime.Now; Data={conifer with Id = nextUniqId conifersStorage}}
    conifersStorage.Add(newConiferEntity.Data.Id, newConiferEntity);
    newConiferEntity
  
  let updateGenre id (conifer: Conifer) =
    match conifersStorage.ContainsKey(id) with
    | true -> 
      let updatedConifer = {LastModified=DateTime.Now; Data={conifer with Id = id}} 
      conifersStorage.[id] <- updatedConifer
      Some updatedConifer
    | false -> None 

  let deleteGenre id =
    conifersStorage.Remove(id) |> ignore

  let coniferGenre = conifersStorage.ContainsKey


