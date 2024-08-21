namespace CloudyCore.IGenericConfigMenuApi

open StardewModdingAPI
open System

/// See https://github.com/spacechase0/StardewValleyMods/tree/develop/GenericModConfigMenu#for-c-mod-authors

[<RequireQualifiedAccess>]
module IGenericConfigMenuApi =
  module Constant =
    [<Literal>]
    let INTERFACE_ID = "spacechase0.GenericModConfigMenu"


[<AllowNullLiteral>]
type IGenericConfigMenuApi =


  /// <summary>Register a mod whose config can be edited through the UI. Must be called fiirst.</summary>
  /// <param name="manifest">The mod's manifest.</param>
  /// <param name="reset">Reset the mod's config to its default values.</param>
  /// <param name="save">Save the mod's current config to the <c>config.json</c> file.</param>
  /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
  /// <remarks>Each mod can only be registered once, unless it's deleted via <see cref="Unregister"/> before calling this again.</remarks>
  abstract member Register :
    manifest : IManifest *
    reset : Action *
    save : Action *
    titleScreenOnly : bool ->
      unit

  /// <summary>Add a section title at the current position in the form.</summary>
  /// <param name="manifest">The mod's manifest.</param>
  /// <param name="text">The title text shown in the form.</param>
  /// <param name="tooltip">The tooltip text shown when the cursor hovers on the title, or <c>null</c> to disable the tooltip.</param>
  abstract member AddSectionTitle :
    manifest : IManifest * text : Func<string> * tooltip : Func<string> -> unit

  /// <summary>Add a paragraph of text at the current position in the form.</summary>
  /// <param name="manifest">The mod's manifest.</param>
  /// <param name="text">The paragraph text to display.</param>
  abstract member AddParagraph :
    manifest : IManifest * text : Func<string> -> unit


  /// <summary>Add a boolean option at the current position in the form.</summary>
  /// <param name="manifest">The mod's manifest.</param>
  /// <param name="getValue">Get the current value from the mod config.</param>
  /// <param name="setValue">Set a new value in the mod config.</param>
  /// <param name="name">The label text to show in the form.</param>
  /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
  /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
  abstract member AddBoolOption :
    manifest : IManifest *
    getValue : Func<bool> *
    setValue : Action<bool> *
    name : Func<string> *
    tooltip : Func<string> *
    fieldId : string ->
      unit


  /// <summary>Add an integer option at the current position in the form.</summary>
  /// <param name="manifest">The mod's manifest.</param>
  /// <param name="getValue">Get the current value from the mod config.</param>
  /// <param name="setValue">Set a new value in the mod config.</param>
  /// <param name="name">The label text to show in the form.</param>
  /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
  /// <param name="min">The minimum allowed value, or <c>null</c> to allow any.</param>
  /// <param name="max">The maximum allowed value, or <c>null</c> to allow any.</param>
  /// <param name="interval">The interval of values that can be selected.</param>
  /// <param name="formatValue">Get the display text to show for a value, or <c>null</c> to show the number as-is.</param>
  /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
  abstract member AddNumberOption :
    manifest : IManifest *
    getValue : Func<int> *
    setValue : Action<int> *
    name : Func<string> *
    tooltip : Func<string> *
    min : Nullable<int> *
    max : Nullable<int> *
    interval : Nullable<int> *
    formatValue : Func<int, string> *
    fieldId : string ->
      unit

  /// <summary>Add a string option at the current position in the form.</summary>
  /// <param name="manifest">The mod's manifest.</param>
  /// <param name="getValue">Get the current value from the mod config.</param>
  /// <param name="setValue">Set a new value in the mod config.</param>
  /// <param name="name">The label text to show in the form.</param>
  /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
  /// <param name="allowedValues">The values that can be selected, or <c>null</c> to allow any.</param>
  /// <param name="formatAllowedValue">Get the display text to show for a value from <paramref name="allowedValues"/>, or <c>null</c> to show the values as-is.</param>
  /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
  abstract member AddTextOption :
    manifest : IManifest *
    getValue : Func<string> *
    setValue : Action<string> *
    name : Func<string> *
    tooltip : Func<string> *
    allowedValues : string array *
    formatAllowedValue : Func<string, string> *
    fieldId : string ->
      unit

  /// <summary>Add a key binding at the current position in the form.</summary>
  /// <param name="manifest">The mod's manifest.</param>
  /// <param name="getValue">Get the current value from the mod config.</param>
  /// <param name="setValue">Set a new value in the mod config.</param>
  /// <param name="name">The label text to show in the form.</param>
  /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
  /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
  abstract member AddKeybind :
    manifest : IManifest *
    getValue : Func<SButton> *
    setValue : Action<SButton> *
    name : Func<string> *
    tooltip : Func<string> *
    fieldId : string ->
      unit


  /// <summary>Register a method to notify when any option registered by this mod is edited through the config UI.</summary>
  /// <param name="manifest">The mod's manifest.</param>
  /// <param name="onChange">The method to call with the option's unique field ID and new value.</param>
  /// <remarks>Options use a randomized ID by default; you'll likely want to specify the <c>fieldId</c> argument when adding options if you use this.</remarks>
  abstract member OnFieldChanged :
    manifest : IManifest * onChange : Action<string, obj> -> unit
