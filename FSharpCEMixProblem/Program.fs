open CEBase


module CEs =

    module Test =
        let ce = CE()
        let x : M<membershipTest<double>> =
            ce { Name "Fred" }

        // TODO: IsMember2 is not working in the following
        let x2 : M<membershipTest<double>> =
            ce {
                Name "Fred" 
                IsMember true
                IsMember true
                IsMember2 true // Note, I can call this twice without compiler error, but not Name in z5
                IsMember2 true
                }

        let x3 : M<membershipTest<double>> =
            ce {
                Name "Fred" 
                IsMember2 true
                }

        let x4 : M<membershipTest<double>> =
            ce {
                IsMember2 true
                }

        let y = 
            ce { 42 }

        let z1 =
            ce { Member 42 }

        let z2 = 
            ce {
                Members [ 41; 42 ]
            }
        let z3 = 
            ce {
                Name "Fred"
                42
                43
            }

        let z4 = 
            ce {
                Member 41
                Member 42
            }

        let z5 : M<membershipTest<double>> = 
            ce {
                Name "a"
                Name "b"
                //42 // removing this line results in compiler error
            }

        let z6 : M<membershipTest<double>> = 
            ce {
                let x = 1
                Name "a"
                Member 4.0
            }

        let z7 : M<membershipTest<double>> = 
            ce {
                let x = "a"
                Name (x + "b")
                Member 4.0
            }

        let z8 : M<membershipTest<double>> = 
            ce {
                let x1 = 1.0
                let y2 = 2.0
                Member (x1 + 3.0)
                Member (y2 + 4.0)
            }

        let z9 =
            ce {
                let x = 1.0
                Members [
                    42.3 
                    43.1 + x
                ]
            }
        //let empty = 
        //    ce { }
        
        System.Console.WriteLine("")

