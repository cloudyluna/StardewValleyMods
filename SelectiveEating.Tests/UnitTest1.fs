module SelectiveEating.Tests

open Expecto
open SelectiveEating.API

let (==) (r : 'a) (e : 'a) : unit = Expect.equal r e ""
let (/=) (r : 'a) (e : 'a) : unit = Expect.notEqual r e ""

let getVitalsPriorityTests =
    testList
        "VitalsSelector.getVitalsPriority"
        [
            test "getVitalsPriority minimum vitals set to 0 should be DoingOk" {
                let config = ModConfig ()
                let playerHealth = { Current = 50 ; Max = 100 }
                let playerStamina = { Current = 20 ; Max = 128 }
                config.MinimumHealthToStartAutoEat <- 0
                config.MinimumStaminaToStartAutoEat <- 0

                let result =
                    VitalsSelector.getVitalsPriority
                        config
                        playerHealth
                        playerStamina

                let expected = DoingOK
                result == expected
            }

            test
                "even when current stamina is below player's health and if health is < auto eat condition and > 0, we still will get Health value" {
                let config = ModConfig ()
                let playerHealth = { Current = 50 ; Max = 100 }
                let playerStamina = { Current = 20 ; Max = 128 }
                config.MinimumHealthToStartAutoEat <- 80
                config.MinimumStaminaToStartAutoEat <- 80

                let result =
                    VitalsSelector.getVitalsPriority
                        config
                        { playerHealth with Current = 80 }
                        playerStamina

                result == Health { Current = 80 ; Max = 100 }
            }
        ]

[<Tests>]
let tests : Test = testList "API tests" [ getVitalsPriorityTests ]
