namespace ContentPatcherDataGenerator

open ContentPatcherDataGenerator.Core
open ContentPatcherDataGenerator.Projects

module Serializer =
  open System.Text.Json
  open System.Text.Json.Serialization

  let private contentPatcherOptions =
    JsonFSharpOptions
      .Default()
      // Add any .WithXXX() calls here to customize the format
      .WithSkippableOptionFields()
      .WithUnionUnwrapFieldlessTags()
      .ToJsonSerializerOptions ()

  let make data =
    JsonSerializer.Serialize (data, contentPatcherOptions)


module Generator =
  open System.IO

  let private dirPath (projectName : string) =
    [| "json" ; $"[CP] {projectName}" |]

  let private createDir projectName =
    Directory.CreateDirectory (Path.Combine <| dirPath projectName) |> ignore

  /// projectName is just plain project name without spaces
  let generate (projectName : string, manifest : Manifest, jsonBlob : obj) =
    createDir projectName

    let makefullPath file =
      Path.Combine <| Array.append (dirPath projectName) [| file |]

    let fullManifestPath = makefullPath "manifest.json"
    let fullContentPath = makefullPath "content.json"

    File.WriteAllText (fullManifestPath, Serializer.make manifest)
    File.WriteAllText (fullContentPath, Serializer.make jsonBlob)


module Main =
  let patchedManifest project =
    let newManifest =
      let newContentPackFor =
        Some
          {
            UniqueID = "Pathoschild.ContentPatcher"
          }

      { project.Manifest with
          UniqueID = Some $"{project.Manifest.Author}.{project.Manifest.Name}"
          ContentPackFor = newContentPackFor
      }

    { project with Manifest = newManifest }

  [| CrabPotJellies.Data |]
  |> Array.map patchedManifest
  |> Array.iter (fun project ->
    Generator.generate (
      project.Manifest.Name.ToString (),
      project.Manifest,
      match project.Content with
      | Loose loose -> loose
      | Strict strict -> strict
    )
  )
