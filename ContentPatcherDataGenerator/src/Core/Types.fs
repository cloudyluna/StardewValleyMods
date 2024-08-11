namespace ContentPatcherDataGenerator.Core


[<AutoOpen>]
module Base =
  [<Literal>]
  let Format = "2.3.0"

  type ToArea =
    {
      X : int
      Y : int
      Width : int
      Height : int
    }

  type Priority =
    | Low
    | Medium
    | High

  type Action =
    | Load
    | EditData
    | Include

  type BaseChanges =
    {
      Action : Action
      Target : string option
      FromFile : string
      Priority : Priority option
      Fields : obj option
      ToArea : ToArea option
      When : obj option
    }

  type Base =
    {
      Format : string
      Changes : BaseChanges array
    }

  type ContentStrictness =
    | Loose of obj
    | Strict of Base


  type PackFor = { UniqueID : string }

  type Manifest =
    {
      Name : string
      Author : string
      Version : string
      Description : string
      /// Leave as None. This will be automatically patched by the generator.
      UniqueID : string option
      UpdateKeys : string array
      /// Leave as None. This will be automatically patched by the generator.
      ContentPackFor : PackFor option
    }


  type Project =
    {
      Manifest : Manifest
      Content : ContentStrictness
    }
