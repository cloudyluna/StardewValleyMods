module SelectiveEating.Tests

open Expecto
open SelectiveEating.API
open SelectiveEating.Config

let (==) (r : 'a) (e : 'a) : unit = Expect.equal r e ""
let (/=) (r : 'a) (e : 'a) : unit = Expect.notEqual r e ""

let getVitalsPriorityTests =
    testList
        "API.VitalsSelector.getVitalsPriority"
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
let tests : Test =
    testList
        "API tests"
        [
            getVitalsPriorityTests
            testList
                "closest percent to replenish"
                [
                    test "health replenish" {
                        VitalsSelector.closestPercentToReplenish 38 100 20 == 47
                    }

                    test "stamina replenish" {
                        VitalsSelector.closestPercentToReplenish 165 128 49
                        == 208
                    }
                ]
            testList
                "vitalStatus replenishers"
                [
                    test "health" {
                        let food =
                            {
                                Id = "1"
                                Edibility = 1
                                HealthRecoveredOnConsumption = 128
                                StaminaRecoveredOnConsumption = 0
                                SellToStorePrice = 0
                                IsPriority = false
                            }

                        let foods =
                            [|
                                food
                                { food with
                                    Id = "2"
                                    HealthRecoveredOnConsumption = 29
                                }
                            |]

                        (VitalsSelector.replenishHealth
                            { Current = 60 ; Max = 100 }
                            foods)
                            .Id
                        == "2"
                    }

                    test "stamina" {
                        let food =
                            {
                                Id = "1"
                                Edibility = 1
                                HealthRecoveredOnConsumption = 0
                                StaminaRecoveredOnConsumption = 128
                                SellToStorePrice = 0
                                IsPriority = false
                            }

                        let foods =
                            [|
                                food
                                { food with
                                    Id = "2"
                                    StaminaRecoveredOnConsumption = 29
                                }
                            |]

                        (VitalsSelector.replenishStamina
                            { Current = 60 ; Max = 100 }
                            foods)
                            .Id
                        == "2"
                    }
                ]

            testList
                "Edibles"
                [
                    test "forbiddenFood tests" {
                        Edibles.parseForbiddenFood "91,92" == [| "91" ; "92" |]

                        Edibles.parseForbiddenFood "(O)   , 2"
                        == [| "(O)" ; "2" |]



                        let food =
                            {
                                Id = "1"
                                Edibility = 1
                                HealthRecoveredOnConsumption = 0
                                StaminaRecoveredOnConsumption = 0
                                SellToStorePrice = 0
                                IsPriority = false
                            }

                        Expect.isTrue
                            (Edibles.isNotForbidden food [| "91" ; "88" |])
                            ""

                        let food2 =
                            {
                                Id = "11"
                                Edibility = 3
                                HealthRecoveredOnConsumption = 0
                                StaminaRecoveredOnConsumption = 0
                                SellToStorePrice = 0
                                IsPriority = false
                            }

                        Expect.isFalse
                            (Edibles.isNotForbidden
                                food2
                                [| "91" ; "11" ; "82" |])
                            ""
                    }
                ]
        ]
