namespace ContentPatcherDataGenerator.Projects

open ContentPatcherDataGenerator.Core


module CrabPotJellies =
  let Data =
    {

      Manifest =
        {
          Name = "CrabPotJellies"
          Author = "Cloudyluna"
          Version = "0.1.0"
          Description = "Make crab pot trap jellies too."
          UniqueID = None
          UpdateKeys = [| "" |]
          ContentPackFor = None
        }
      Content =
        Strict
          {
            Format = Format
            Changes =
              [|
                {
                  Action = Load
                  Target = Some "Portraits/Abigail"
                  FromFile = ""
                  Priority = None
                  Fields = None
                  ToArea = None
                  When = None
                }
              |]
          }
    }
