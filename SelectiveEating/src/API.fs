namespace SelectiveEating.API

open CloudyCore.Prelude
open CloudyCore.Prelude.Math
open SelectiveEating

type Food =
    {
        Id : string
        Edibility : int
        HealthRecoveredOnConsumption : int
        StaminaRecoveredOnConsumption : int
        SellToStorePrice : int
        IsPriority : bool
    }

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

    /// Percentage lost that is needed to be replenished.
    let closestPercentToReplenish foodRecoveryPoints statMax percentToRefill =
        abs <| ofPercentage foodRecoveryPoints (statMax - percentToRefill)

    let private replenishVitalStatus
        (stat : VitalStatus)
        (edibles : Food array)
        (foodRecoveryGetter : Food -> int)
        : Food
        =
        let percentToRefill = stat.Max - stat.Current

        edibles
        |> Array.minBy (fun fi ->
            closestPercentToReplenish
                (foodRecoveryGetter fi)
                stat.Current
                percentToRefill
        )

    let replenishHealth (health : VitalStatus) (edibles : Food array) : Food =

        _.HealthRecoveredOnConsumption |> replenishVitalStatus health edibles


    let replenishStamina (stamina : VitalStatus) (edibles : Food array) : Food =
        _.StaminaRecoveredOnConsumption |> replenishVitalStatus stamina edibles

module CheapestSelector =
    let get (edibles : Food array) : Food =
        edibles |> Array.minBy _.SellToStorePrice

module Edibles =
    let parseForbiddenFood (str : string) : string array =
        str.Trim().Split (',', System.StringSplitOptions.RemoveEmptyEntries)
        |> Array.map (fun i -> i.Trim ())

    let isNotForbidden (food : Food) (forbiddenFood : string array) : bool =
        not <| Array.exists (fun id -> food.Id = id) forbiddenFood

        // We don't care about food that gives negative health/stamina or nothing
        // positive of value at all.
        && food.Edibility > 0
