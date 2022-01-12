namespace Test

open CEBase

    type Membership = 
        { Name: string option
          IsMember: bool option
          IsMember2: bool  // confirming that shape of the container is not restricted, just need clear default the CE understands
          Members: double list }    

    type CE() =
        inherit DslBase<Membership, double>()

        override _.Empty() =
            { Name = None
              IsMember = None
              IsMember2 = false
              Members = [] }

        override _.CombineModels model1 model2 =
            let newName = 
                match model2.Name with
                | None -> model1.Name
                | res -> res
            let newIsMember =
                match model2.IsMember with 
                | None -> model1.IsMember
                | res -> res
            let newIsMember2 =
                match model2.IsMember2 with 
                | true -> true
                | res -> res
            { Name = newName
              IsMember = newIsMember
              IsMember2 = newIsMember2
              Members =  model1.Members @ model2.Members }

        override this.NewMember (item: double) =
            { this.Empty() with Members = [ item ] }

        [<CustomOperation("Name", MaintainsVariableSpaceUsingBind = true)>]
        member this.setName (model: M<Membership, 'Vars>, [<ProjectionParameter>] name: ('Vars -> string)) : M<Membership, 'Vars>  =
            this.SetModel model { model.Model with Name = Some (name model.Variables) }

        [<CustomOperation("IsMember", MaintainsVariableSpaceUsingBind = true)>]
        member this.setIsMember (model: M<Membership, 'Vars>, [<ProjectionParameter>] isMember: ('Vars -> bool)) : M<Membership, 'Vars>  =
            this.SetModel model { model.Model with IsMember = Some (isMember model.Variables) }

        // We can skip 
        [<CustomOperation("IsMember2", MaintainsVariableSpaceUsingBind = true)>]
        member this.setIsMember2 (model: M<Membership, 'Vars>, [<ProjectionParameter>] isMember: ('Vars -> bool)) : M<Membership, 'Vars>  =
            let newIsMember2 = isMember model.Variables
            this.SetModel model { model.Model with IsMember2 = newIsMember2 }

        [<CustomOperation("Member", MaintainsVariableSpaceUsingBind = true)>]
        member this.addMember (model: M<Membership, 'Vars>, [<ProjectionParameter>] item: ('Vars -> 'TItem))  : M<Membership, 'Vars>  =
            this.SetModel model { model.Model with Members = List.append model.Model.Members [ item model.Variables ] }

        // Note, using ParamArray doesn't work in conjunction with ProjectionParameter
        [<CustomOperation("Members", MaintainsVariableSpaceUsingBind = true)>]
        member this.addMembers (model: M<Membership, 'Vars>, [<ProjectionParameter>] items: ('Vars -> 'TItem list)) : M<Membership, 'Vars>  =
            this.SetModel model { model.Model with Members = List.append model.Model.Members (items model.Variables) }


