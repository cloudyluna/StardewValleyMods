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

    internal static bool DayUpdate_Prefix(CrabPot __instance)
    {
        try
        {
            var crabPot = __instance;

            GameLocation location = crabPot.Location;
            bool flag = Game1.getFarmer(crabPot.owner.Value) != null && Game1.getFarmer(crabPot.owner.Value).professions.Contains(11);
            bool flag2 = Game1.getFarmer(crabPot.owner.Value) != null && Game1.getFarmer(crabPot.owner.Value).professions.Contains(10);
            if (crabPot.owner.Value == 0L && Game1.player.professions.Contains(11))
            {
                flag2 = true;
            }
            if (!(crabPot.bait.Value != null || flag) || crabPot.heldObject.Value != null)
            {
                return false;
            }
            crabPot.tileIndexToShow = 714;
            crabPot.readyForHarvest.Value = true;
            Random random = Utility.CreateDaySaveRandom(crabPot.tileLocation.X * 1000f, crabPot.tileLocation.Y * 255f, crabPot.directionOffset.X * 1000f + crabPot.directionOffset.Y);
            Dictionary<string, string> dictionary = DataLoader.Fish(Game1.content);
            List<string> list = new List<string>();
            if (!location.TryGetFishAreaForTile(crabPot.tileLocation.Value, out var _, out var data))
            {
                data = null;
            }
            double num = (flag2 ? 0.0 : (((double?)data?.CrabPotJunkChance) ?? 0.2));
            int initialStack = 1;
            int num2 = 0;
            string text = null;
            if (crabPot.bait.Value != null && crabPot.bait.Value.QualifiedItemId == "(O)DeluxeBait")
            {
                num2 = 1;
                num /= 2.0;
            }
            else if (crabPot.bait.Value != null && crabPot.bait.Value.QualifiedItemId == "(O)774")
            {
                num /= 2.0;
                if (random.NextBool(0.25))
                {
                    initialStack = 2;
                }
            }
            else if (crabPot.bait.Value != null && crabPot.bait.Value.Name.Contains("Bait") && crabPot.bait.Value.preservedParentSheetIndex != null && crabPot.bait.Value.preserve.Value.HasValue)
            {
                text = crabPot.bait.Value.preservedParentSheetIndex.Value;
                num /= 2.0;
            }
            IList<string> crabPotFishForTile = location.GetCrabPotFishForTile(crabPot.tileLocation.Value);
            if (!random.NextBool(num))
            {
                foreach (KeyValuePair<string, string> item in dictionary)
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
                    if (flag2)
                    {
                        list.Add(item.Key);
                        continue;
                    }
                    double num3 = Convert.ToDouble(array[2]);
                    if (text != null && text == item.Key)
                    {
                        num3 *= (double)((num3 < 0.1) ? 4 : ((num3 < 0.2) ? 3 : 2));
                    }
                    if (!(random.NextDouble() < num3))
                    {
                        continue;
                    }
                    crabPot.heldObject.Value = new Object(item.Key, initialStack, isRecipe: false, -1, num2);
                    break;
                }
            }
            if (crabPot.heldObject.Value == null)
            {
                if (flag2 && list.Count > 0)
                {
                    crabPot.heldObject.Value = ItemRegistry.Create<Object>("(O)" + random.ChooseFrom(list));
                }
                else
                {
                    var trashId = random.Next(168, 173).ToString();
                    TryCollectJellies(random, crabPotFishForTile, trashId, out List<string> resultIds);
                    string chosenId = (resultIds.Count > 0) ? random.ChooseFrom(resultIds) : trashId;
                    crabPot.heldObject.Value = ItemRegistry.Create<Object>("(O)" + chosenId);
                }
            }

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
        IList<string> crabPotFishForTile,
        string trashId,
        out List<string> resultIds
    )
    {
        resultIds = new List<string> { };
        if (Config != null && Config.IsModEnabled)
        {
            if (!Config.IsReplaceAllTrashSelected)
                resultIds.Add(trashId);
            CollectJellies(random, crabPotFishForTile, ref resultIds);

        }
    }

    static private void CollectJellies(
        Random random,
        IList<string> crabPotFishForTile,
        ref List<string> ids)
    {
        foreach (string jellyId in crabPotFishForTile)
        {
            if (jellyId == "ocean")
            {
                ids.Add("SeaJelly");
            }
            else
            {
                ids.Add("RiverJelly");

                // Make this rarer because:
                // 1. We cannot put pots in caves, so we gotta put
                // them wherever valid freshwater location is and
                // that would mean..rivers.
                // To see cave jelly appear as often as river's
                // doesn't make a lot of sense,
                // so hence why we randomize this addition a bit further.
                if (random.NextBool()) ids.Add("CaveJelly");

            }

            break;
        }
    }
}