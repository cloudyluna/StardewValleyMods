module SelectiveEating.Tests

open Expecto


[<Tests>]
let tests =
    testList
        "model test"
        [
            testCase "A simple test"
            <| fun () ->
                let expected = 4
                Expect.equal (2 + 2) expected "2+2 = 4"
        ]
