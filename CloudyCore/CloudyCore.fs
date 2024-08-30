namespace CloudyCore.Prelude

open StardewValley
open StardewModdingAPI

[<AutoOpen>]
module Debug =
    let private console level (monitor : IMonitor) text =
        monitor.Log (text, level)

    /// Print debug message to console with appended newline.
    let debug (monitor : IMonitor) (text : string) =
        console LogLevel.Debug monitor text

    /// Same as debug but prints at info level.
    let info (monitor : IMonitor) (text : string) =
        console LogLevel.Info monitor text

    /// Same as debug and info but prints at error level.
    let error (monitor : IMonitor) (text : string) =
        console LogLevel.Error monitor text

[<AutoOpen>]
module Conditional =
    let unless (predicate : bool) (action : unit) : unit =
        if not predicate then action else ()

/// Casts things from StardewValley related APIs. These will return either
/// option or result type due to their nullability.
module TryCast =
    let toObject (obj : obj) : Object option =
        match obj with
        | :? Object as o -> Some o
        | _ -> None

[<AutoOpen>]
module Movement =
    /// NPC and player looking/movement direction.
    type Direction =
        | Up = 0
        | Right = 1
        | Down = 2
        | Left = 3

[<AutoOpen>]
module Combinator =
    /// <summary>
    /// Executes a side-effect function and returns the original input value.
    /// </summary>
    /// <category index="0">Common Combinators</category>
    let tap (f : 'T -> unit) x =
        f x
        x

    let flip (f : 'a -> 'b -> 'c) (a : 'b) (b : 'a) : 'c = f b a

    /// Synonym of `<|` or Haskell `$`.
    let (^) = (<|)

[<RequireQualifiedAccess>]
module Option =
    /// Flipped version of `iter`.
    let exec (a : 'a option) (b : 'a -> unit) : unit = flip Option.iter a b

module Math =
    type Percentage = | Percentage of int

    let ofPercentage current max =
        int <| (float current / float max) * 100.0

    let makePercentage current max = Percentage <| ofPercentage current max
    let fromPercentage (Percentage percentage) = percentage

// Ignore translation warnings.
#nowarn "3391"

[<RequireQualifiedAccess>]
module Translation =
    open System

    let stringKey
        (translationHelper : ITranslationHelper)
        (keyName : string)
        : Func<string>
        =
        Func<string> (fun _ -> translationHelper.Get keyName)

    /// `recordParam` could be anonnymous object, dictionary or class.
    let genericKey
        (translationHelper : ITranslationHelper)
        (keyName : string)
        (recordParam : 'a)
        : Translation
        =
        translationHelper.Get (keyName, recordParam)
