module SelectiveEating.Tests

open Expecto
open SelectiveEating.Model

module ModelTest =
    let equal (r : 'a) (e : 'a) : unit = Expect.equal r e ""

    [<Tests>]
    let tests : Test =
        testList
            "model test"
            [
                testCase
                    "getVitalsPriority minimum vitals set to 0 should be DoingOk"
                <| fun () ->
                    let config = ModConfig ()
                    config.MinimumHealthToStartAutoEat <- 0
                    config.MinimumStaminaToStartAutoEat <- 0

                    let health = { Current = 50 ; Max = 100 }
                    let stamina = { Current = 20 ; Max = 128 }

                    let result =
                        VitalsSelector.getVitalsPriority config health stamina

                    let expected = DoingOK
                    equal result expected
            ]
