namespace CrabPotCollectJellies;

using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley;
using StardewValley.Extensions;

internal class CrabPotPatcher
{
    private static IMonitor? Monitor;
    private static ModConfig? Config;

    internal static void Initialize(IMonitor monitor, ModConfig config)
    {
        Monitor = monitor;
        Config = config;
    }

    // Most of this code was taken from Stardew's code, which is not subjected to AGPL-3.0 license.
    // The only changes I made here are some of the variable names so I can understand what the heck is going on.
    //
    internal static bool DayUpdate_Prefix(CrabPot __instance)
    {
        try
        {
            var crabPot = __instance;

            // ************************************* Stardew Code ************************************************
            GameLocation location = crabPot.Location;
            var lureMaster = 11;
            var mariner = 10;
            bool isALureMaster = Game1.getFarmer(crabPot.owner.Value) != null && Game1.getFarmer(crabPot.owner.Value).professions.Contains(lureMaster);
            bool isAMariner = Game1.getFarmer(crabPot.owner.Value) != null && Game1.getFarmer(crabPot.owner.Value).professions.Contains(mariner);

            if (crabPot.owner.Value == 0L && Game1.player.professions.Contains(lureMaster))
            {
                isAMariner = true;
            }
            if (!(crabPot.bait.Value != null || isALureMaster) || crabPot.heldObject.Value != null)
            {
                return false;
            }

            crabPot.tileIndexToShow = 714;
            crabPot.readyForHarvest.Value = true;
            Random random = Utility.CreateDaySaveRandom(crabPot.tileLocation.X * 1000f, crabPot.tileLocation.Y * 255f, crabPot.directionOffset.X * 1000f + crabPot.directionOffset.Y);
            Dictionary<string, string> fishIds = DataLoader.Fish(Game1.content);
            if (!location.TryGetFishAreaForTile(crabPot.tileLocation.Value, out var _, out var data))
            {
                data = null;
            }
            double crabPotFishCatchChance = (isAMariner ? 0.0 : (((double?)data?.CrabPotJunkChance) ?? 0.2));
            int initialStack = 1;
            int crabPotFishQuality = 0;
            string targetedBaitId = null;
            if (crabPot.bait.Value != null && crabPot.bait.Value.QualifiedItemId == "(O)DeluxeBait")
            {
                crabPotFishQuality = 1;
                crabPotFishCatchChance /= 2.0;
            }
            else if (crabPot.bait.Value != null && crabPot.bait.Value.QualifiedItemId == "(O)774")
            {
                crabPotFishCatchChance /= 2.0;
                if (random.NextBool(0.25))
                {
                    initialStack = 2;
                }
            }
            else if (crabPot.bait.Value != null && crabPot.bait.Value.Name.Contains("Bait") && crabPot.bait.Value.preservedParentSheetIndex != null && crabPot.bait.Value.preserve.Value.HasValue)
            {
                targetedBaitId = crabPot.bait.Value.preservedParentSheetIndex.Value;
                crabPotFishCatchChance /= 2.0;
            }

            List<string> collectedFishIdsForMariner = new List<string>();
            IList<string> crabPotFishForTile = location.GetCrabPotFishForTile(crabPot.tileLocation.Value);
            if (!random.NextBool(crabPotFishCatchChance))
            {
                foreach (KeyValuePair<string, string> item in fishIds)
                {
                    if (!item.Value.Contains("trap"))
                    {
                        continue;
                    }
                    string[] array = item.Value.Split('/');
                    string[] array2 = ArgUtility.SplitBySpace(array[4]);
                    bool flag3 = false;
                    string[] array3 = array2;
                    foreach (string text2 in array3)
                    {
                        foreach (string item2 in crabPotFishForTile)
                        {
                            if (text2 == item2)
                            {
                                flag3 = true;
                                break;
                            }
                        }
                    }
                    if (!flag3)
                    {
                        continue;
                    }
                    if (isAMariner)
                    {
                        collectedFishIdsForMariner.Add(item.Key);
                        continue;
                    }
                    double num3 = Convert.ToDouble(array[2]);
                    if (targetedBaitId != null && targetedBaitId == item.Key)
                    {
                        num3 *= (double)((num3 < 0.1) ? 4 : ((num3 < 0.2) ? 3 : 2));
                    }
                    if (!(random.NextDouble() < num3))
                    {
                        continue;
                    }

                    TryCollectJellies(random, location, crabPotFishForTile, item.Key, out List<string> resultIds);
                    string chosenId = random.ChooseFrom(resultIds);
                    crabPot.heldObject.Value = new Object(chosenId, initialStack, isRecipe: false, -1, crabPotFishQuality);
                    break;
                }
            }

            if (crabPot.heldObject.Value == null)
            {
                if (isAMariner && collectedFishIdsForMariner.Count > 0)
                {
                    var randomFishId = random.ChooseFrom(collectedFishIdsForMariner);
                    TryCollectJellies(random, location, crabPotFishForTile, randomFishId, out List<string> resultIds);
                    var chosenId = random.ChooseFrom(resultIds);

                    if (chosenId == randomFishId)
                    {
                        crabPot.heldObject.Value = ItemRegistry.Create<Object>("(O)" + chosenId);
                    }
                    else
                    {
                        // We only alter our jellies type to follow the same
                        // collection chance that is when player doesn't have 
                        // the mariner skill.
                        //
                        // Why not alter the vanilla version too? 
                        //
                        // We are trying to be conservative with addition here
                        // and to keep things simple as possible.
                        // It will also probably breaks existing crab pot
                        // fixes that were added by more thoroughly changed
                        // crab pot mods.
                        // Though, we certainly can introduce this addition
                        // if there is demand, of course.
                        crabPot.heldObject.Value = ItemRegistry.Create<Object>(
                            "(O)" + chosenId,
                            amount: initialStack,
                            quality: crabPotFishQuality
                        );
                    }
                }
                else // trash only
                {
                    crabPot.heldObject.Value = ItemRegistry.Create<Object>("(O)" + random.Next(168, 173));
                }
            }

            // ************************************* Stardew Code ************************************************

            return false;

        }

        catch (Exception ex)
        {
            Monitor?.Log(
                $"Failed in patched code of: {nameof(DayUpdate_Prefix)}:\n{ex}",
                LogLevel.Error
            );
            return true;
        }
    }

    static private void TryCollectJellies(
        Random random,
        GameLocation location,
        IList<string> crabPotFishForTile,
        string originalId,
        out List<string> resultIds
    )
    {
        resultIds = new List<string> { originalId };
        if (Config != null && Config.IsModEnabled)
        {
            CollectJellies(random, location, crabPotFishForTile, ref resultIds);
        }
    }

    static private void CollectJellies(
        Random random,
        GameLocation location,
        IList<string> crabPotFishForTile,
        ref List<string> ids)
    {
        foreach (string jellyId in crabPotFishForTile)
        {
            var baseChance = 0.6;
            if (jellyId == "ocean" && !(location.NameOrUniqueName == "UndergroundMine"))
            {
                if (random.NextBool(baseChance + 0.1)) ids.Add("SeaJelly");
            }
            else
            {
                // Make this rarer because:
                // Cave jellies are expensive and they give luck bonus.
                // To see cave jelly appear as often as river's
                // doesn't make a lot of sense,
                // so hence why we randomize this addition a bit further.
                var riverJelly = "RiverJelly";
                var caveJelly = "CaveJelly";
                var _ids = ids;

                void weightedCaveJellyChance(double chance)
                {
                    if (random.NextBool(baseChance - chance)) _ids.Add(riverJelly);
                    if (random.NextBool(baseChance + chance)) _ids.Add(caveJelly);
                }

                if (location.NameOrUniqueName == "WitchSwamp")
                {
                    weightedCaveJellyChance(0.1);
                    ids = _ids;
                }
                else if (location.NameOrUniqueName == "FarmCave")
                {

                    weightedCaveJellyChance(0.2);
                    ids = _ids;
                }
                else if (location.NameOrUniqueName == "UndergroundMine")
                {
                    if (random.NextBool(baseChance + 0.2)) ids.Add(caveJelly);
                }
                else
                {
                    if (random.NextBool(baseChance + 0.1)) ids.Add(riverJelly);
                    if (random.NextBool(baseChance - 0.3)) ids.Add(caveJelly);
                }
            }

            break;
        }
    }
}
