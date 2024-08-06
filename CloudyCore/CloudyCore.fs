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
module Functors =
  /// <summary>
  /// Executes a side-effect function and returns the original input value.
  /// </summary>
  /// <category index="0">Common Combinators</category>
  let tap (f : 'T -> unit) x =
    f x
    x
