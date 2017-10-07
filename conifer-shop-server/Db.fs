namespace ConiferShop.Db

[<AutoOpen>]
module Db = 
  open System.Collections.Generic
  open System

  type Entity<'a> = {
    LastModified: DateTime
    Data: 'a
  }

  type Address = {
    Street: string
    Number: int
    City: string
  }

  type Genus = { // plural = Genera
    Id: int
    Name: string
  }

  type Conifer = {
    Id: int
    Name: string
    GenusId: int
    Species: string
    PhotoUrl: string
  }

  type Shop = {
    Id: int
    Name: string
    Addresss: Address
    Telephone: int
    Email: string
  }

  let generaStorage =
    new Dictionary<int, Entity<Genus>>(
      dict [
        (0, {LastModified=new DateTime(2017, 01, 01, 12, 0, 0); Data={Id = 0; Name = "Pinus"}});
        (1, {LastModified=new DateTime(2017, 01, 01, 12, 0, 0); Data={Id = 1; Name = "Abies"}});
        (3, {LastModified=new DateTime(2017, 01, 01, 12, 0, 0); Data={Id = 3; Name = "Picea"}});
        (4, {LastModified=new DateTime(2017, 01, 01, 12, 0, 0); Data={Id = 4; Name = "Chamaecyparis"}});
        (5, {LastModified=new DateTime(2017, 01, 01, 12, 0, 0); Data={Id = 5; Name = "Thuja"}});
        (6, {LastModified=new DateTime(2017, 01, 01, 12, 0, 0); Data={Id = 6; Name = "Taxus"}});
      ]
    )

  let conifersStorage = 
    new Dictionary<int, Entity<Conifer>>(
      dict [
        (0, {LastModified=new DateTime(2017, 01, 01, 12, 0, 0); Data={Id = 0; Name = "Winter Gold"; GenusId = 0; Species = "Mugo"; PhotoUrl = "http://www.futuregardens.pl/media/products/a1a72aba34e0fc5ec7130422b7424e4d/images/thumbnail/big_Sosna_kosodrzewina_WINTER_GOLD1.jpg?lm=1497530223"}});
        (1, {LastModified=new DateTime(2017, 01, 01, 12, 0, 0); Data={Id = 1; Name = "Luminetta"; GenusId = 1; Species = "Koreana"; PhotoUrl = "https://lh6.googleusercontent.com/IqlpDC943S1SQUpihS_yYF3PCfhxyv_W6Nj7c7gxuBQ=w380-h509-no"}});
        (2, {LastModified=new DateTime(2017, 01, 01, 12, 0, 0); Data={Id = 2; Name = "Tom Thumb Gold"; GenusId = 3; Species = "Abies"; PhotoUrl = "http://www.szkolka-przytok.pl/userdata/gfx/412b43831abe9562105817f379e91697.jpg"}})
        (3, {LastModified=new DateTime(2017, 01, 01, 12, 0, 0); Data={Id = 3; Name = "Amersfort"; GenusId = 6; Species = "Baccata"; PhotoUrl = "https://worldplants.ca/photos/Taxus-baccata-amersfoort-form.jpg"}})
      ]
    )

  let shopStorage =
    new Dictionary<int, Entity<Shop>>(
      dict [
        (0, {LastModified=new DateTime(2017, 01, 01, 12, 0, 0); Data={Id = 0; Name = "Końca"; Addresss = { Street = "Krokusowa"; Number=20; City="Zgierz"}; Telephone = 634991420; Email = "konca@gmail.com"}});
        (1, {LastModified=new DateTime(2017, 01, 01, 12, 0, 0); Data={Id = 1; Name = "Tracz"; Addresss = { Street = "Zielona"; Number=22; City="Łódź"}; Telephone = 123991420; Email = "tracz@gmail.com"}});
        (2, {LastModified=new DateTime(2017, 01, 01, 12, 0, 0); Data={Id = 2; Name = "Lewandowski"; Addresss = { Street = "Kolejowa"; Number=32; City="Zgierz"}; Telephone = 634456420; Email = "lewandowski@gmail.com"}});
        (3, {LastModified=new DateTime(2017, 01, 01, 12, 0, 0); Data={Id = 3; Name = "Petertil"; Addresss = { Street = "Słoneczna"; Number=1; City="Częstochowa"}; Telephone = 632341476; Email = "petertil@gmail.com"}})
      ]
    )

  let nextUniqId (storage: Dictionary<int, 'a> ) =
    let mutable i = 0
    while(storage.ContainsKey(i)) do
      i <- i + 1
    i