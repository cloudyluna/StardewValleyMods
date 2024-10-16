namespace SelectiveEating.Model

open CloudyCore.Prelude
open CloudyCore.Prelude.Math
open SelectiveEating

type Food =
    {
        Id : string
        Name : string
        Stack : int
        Edibility : int
        HealthRecoveredOnConsumption : int
        StaminaRecoveredOnConsumption : int
        SellToStorePrice : int
    }

[<Struct>]
type FoodItem =
    {
        Food : Food
        Item : StardewValley.Object
        IsPriority : bool
    }

[<Struct>]
type VitalStatus = { Current : int ; Max : int }

type VitalsPriority =
    | Health of VitalStatus
    | Stamina of VitalStatus
    | DoingOK

module VitalsSelector =

    /// Make percentage of current player's health.
    let private makePlayerHealth (health : int) (maxHealth : int) =
        ofPercentage health maxHealth

    /// Make percentage of current player's stamina.
    let private makePlayerStamina (stamina : int) (maxStamina : int) =
        ofPercentage stamina maxStamina

    let private makeVitalStatus current max = { Current = current ; Max = max }

    let getVitalsPriority
        (config : ModConfig)
        (playerHealth : VitalStatus)
        (playerStamina : VitalStatus)
        =
        let minimumActive p = not (p <= 0)

        if
            minimumActive config.MinimumHealthToStartAutoEat
            && makePlayerHealth playerHealth.Current playerHealth.Max
               <= config.MinimumHealthToStartAutoEat
        then
            Health ^ makeVitalStatus playerHealth.Current playerHealth.Max
        elif
            minimumActive config.MinimumStaminaToStartAutoEat
            && makePlayerStamina playerStamina.Current playerStamina.Max
               <= config.MinimumStaminaToStartAutoEat
        then
            Stamina
            ^ makeVitalStatus (int playerStamina.Current) playerStamina.Max
        else
            DoingOK

    let closestNeededToReplenish recoveredAmount amountToRefill maxStat =
        abs (ofPercentage recoveredAmount (maxStat - amountToRefill))

    module Edibles =
        let private parsedForbiddenFood (str : string) : string array =
            str.Trim().Split (',', System.StringSplitOptions.RemoveEmptyEntries)
            |> Array.map (fun i -> i.Trim ())

        let private isNotForbidden
            (food : Food)
            (forbiddenFood : string array)
            : bool
            =
            not <| Array.exists (fun id -> food.Id = id) forbiddenFood

            // We don't care about food that gives negative health/stamina or nothing
            // positive of value at all.
            && food.Edibility > 0

        let makeFoodItem
            unprocessedFood
            forbiddenFood
            isPartOfActiveInventoryRow
            item
            =
            seq {
                if
                    isNotForbidden
                        unprocessedFood
                        (parsedForbiddenFood forbiddenFood)
                then
                    {
                        Food = unprocessedFood
                        Item = item
                        IsPriority = isPartOfActiveInventoryRow
                    }
            }
