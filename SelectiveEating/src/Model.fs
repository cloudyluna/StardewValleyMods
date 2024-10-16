namespace SelectiveEating.Model

open CloudyCore.Prelude
open CloudyCore.Prelude.Math
open SelectiveEating

type Food =
    {
        id : string
        name : string
        stack : int
        edibility : int
        healthRecoveredOnConsumption : int
        staminaRecoveredOnConsumption : int
        sellToStorePrice : int
    }

[<Struct>]
type FoodItem = { Food : Food ; IsPriority : bool }

[<Struct>]
type VitalStatus = { Current : Percentage ; Max : int }

type VitalsPriority =
    | Health of VitalStatus
    | Stamina of VitalStatus
    | DoingOK

type Health = { current : int ; max : int }
type Stamina = { current : int ; max : int }

module VitalsSelector =

    /// Make percentage of current player's health.
    let private makePlayerHealth (health : int) (maxHealth : int) =
        ofPercentage health maxHealth

    /// Make percentage of current player's stamina.
    let private makePlayerStamina (stamina : int) (maxStamina : int) =
        ofPercentage stamina maxStamina

    let private makeVitalStatus current max =
        {
            Current = makePercentage current max
            Max = max
        }

    let getVitalsPriority
        (config : ModConfig)
        (playerHealth : Health)
        (playerStamina : Stamina)
        =
        let minimumActive p = not (p <= 0)

        if
            minimumActive config.MinimumHealthToStartAutoEat
            && makePlayerHealth playerHealth.current playerHealth.max
               <= config.MinimumHealthToStartAutoEat
        then
            Health ^ makeVitalStatus playerHealth.current playerHealth.max
        elif
            minimumActive config.MinimumStaminaToStartAutoEat
            && makePlayerStamina playerStamina.current playerStamina.max
               <= config.MinimumStaminaToStartAutoEat
        then
            Stamina
            ^ makeVitalStatus (int playerStamina.current) playerStamina.max
        else
            DoingOK

    let private closestNeededToReplenish
        recoveredAmount
        amountToRefill
        maxStat
        =
        abs (ofPercentage recoveredAmount (maxStat - amountToRefill))

    let replenishHealth
        (vitalStat : VitalStatus)
        (edibles : FoodItem array)
        : FoodItem
        =

        let health = fromPercentage vitalStat.Current
        let maxHealth = vitalStat.Max
        let amountToRefill = maxHealth - health

        edibles
        |> Array.minBy (fun item ->
            closestNeededToReplenish
                item.Food.healthRecoveredOnConsumption
                amountToRefill
                maxHealth
        )

    let replenishStamina
        (vital : VitalStatus)
        (edibles : FoodItem array)
        : FoodItem
        =
        let stamina = fromPercentage vital.Current
        let maxStamina = vital.Max
        let amountToRefill = maxStamina - stamina

        edibles
        |> Array.minBy (fun item ->
            closestNeededToReplenish
                item.Food.staminaRecoveredOnConsumption
                amountToRefill
                maxStamina
        )

    module Edibles =
        let private parsedForbiddenFood (str : string) : string array =
            str.Trim().Split (',', System.StringSplitOptions.RemoveEmptyEntries)
            |> Array.map (fun i -> i.Trim ())

        let private isNotForbidden
            (food : Food)
            (forbiddenFood : string array)
            : bool
            =
            not <| Array.exists (fun id -> food.id = id) forbiddenFood

            // We don't care about food that gives negative health/stamina or nothing
            // positive of value at all.
            && food.edibility > 0

        let makeFoodItem
            unprocessedFood
            forbiddenFood
            isPartOfActiveInventoryRow
            =
            seq {
                if
                    isNotForbidden
                        unprocessedFood
                        (parsedForbiddenFood forbiddenFood)
                then
                    {
                        Food = unprocessedFood
                        IsPriority = isPartOfActiveInventoryRow
                    }
            }
