namespace ConiferShop.Repositories

[<AutoOpen>]
module ConifersRepository =
  open ConiferShop.Db
  open System

  let getAllConifers() =
    Console.WriteLine(conifersStorage.Count);
    conifersStorage.Values :> seq<Entity<Conifer>> // [ :> ] - converts a type to type that is higher in the hierarchy.
    
  let getConifer id =
    match conifersStorage.ContainsKey(id) with
    | true -> Some conifersStorage.[id]
    | false -> None

  let createConifer (conifer: Conifer) =
    let newConiferEntity = {LastModified=DateTime.Now; Data={conifer with Id = nextUniqId conifersStorage}}
    conifersStorage.Add(newConiferEntity.Data.Id, newConiferEntity);
    newConiferEntity
  
  let updateConifer id (conifer: Conifer) =
    match conifersStorage.ContainsKey(id) with
    | true -> 
      let updatedConifer = {LastModified=DateTime.Now; Data={conifer with Id = id}} 
      conifersStorage.[id] <- updatedConifer
      Some updatedConifer
    | false -> None 

  let deleteConifer id =
    conifersStorage.Remove(id) |> ignore

  let coniferExists = conifersStorage.ContainsKey


