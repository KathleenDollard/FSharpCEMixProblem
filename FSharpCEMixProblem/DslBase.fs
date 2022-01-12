namespace CEBase 

    type membershipTest<'TItem> = 
        { Name: string option
          IsMember: bool option
          IsMember2: bool  // confirming that shape of the container is not restricted, just need clear default the CE understands
          Members: 'TItem list }

    type M<'T, 'Vars> =
        { Model: 'T
          Variables: 'Vars }

    type M<'T> = M<'T, unit>

    [<AbstractClass>]
    type DslBase<'T, 'TItem> () =
        abstract member Empty: unit -> 'T
        abstract member CombineModels: 'T -> 'T -> 'T
        abstract member NewMember: 'TItem -> 'T

        member this.Zero() : M<'T, unit> = 
            { Model = this.Empty() 
              Variables = () }

        member this.Combine (varModel1: M<'T, unit>, varModel2: M<'T, unit>) : M<'T, unit> =
            { Model = this.CombineModels varModel1.Model varModel2.Model
              Variables = () }

        member _.Delay(f) : M<'T, 'Vars> = f()

        member this.Run(varModel: M<'T, 'Vars>) : M<'T, unit> =
            { Model = this.CombineModels varModel.Model (this.Empty())
              Variables = () }

        member this.For(methods, f) :M<'T, unit> = 
            let methodList = Seq.toList methods
            match methodList with 
            | [] -> this.Zero()
            | [x] -> f(x)
            | head::tail ->
                let mutable headResult = f(head)
                for x in tail do 
                    headResult <- this.Combine(headResult, f(x))
                headResult

        member this.Yield (item: 'TItem) : M<'T, unit> = 
            { Model = this.NewMember item
              Variables = () }

        // Only for packing/unpacking the implicit variable space
        member this.Bind (varModel1: M<'T, 'Vars>, f: ('Vars -> M<'T, unit>)) : M<'T, unit>  =
            let varModel2 = f varModel1.Variables
            let combined = this.CombineModels varModel1.Model varModel2.Model
            { Model = combined
              Variables = varModel2.Variables }

        // Only for packing/unpacking the implicit variable space
        member this.Return (varspace: 'Vars) : M<'T, 'Vars> = 
            { Model = this.Empty() 
              Variables = varspace }

        member _.SetModel (varModel: M<'T, 'Vars>) (newModel: 'T) =
            { Model = newModel
              Variables = varModel.Variables }            

    type CE<'TItem>() =
        inherit DslBase<membershipTest<'TItem>, 'TItem>()

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

        override this.NewMember (item: 'TItem) =
            { this.Empty() with Members = [ item ] }

        [<CustomOperation("Name", MaintainsVariableSpaceUsingBind = true)>]
        member this.setName (model: M<'T, 'Vars>, [<ProjectionParameter>] name: ('Vars -> string)) : M<'T, 'Vars>  =
            this.SetModel model { model.Model with Name = Some (name model.Variables) }

        [<CustomOperation("IsMember", MaintainsVariableSpaceUsingBind = true)>]
        member this.setIsMember (model: M<'T, 'Vars>, [<ProjectionParameter>] isMember: ('Vars -> bool)) : M<'T, 'Vars>  =
            this.SetModel model { model.Model with IsMember = Some (isMember model.Variables) }

        // We can skip 
        [<CustomOperation("IsMember2", MaintainsVariableSpaceUsingBind = true)>]
        member this.setIsMember2 (model: M<'T, 'Vars>, [<ProjectionParameter>] isMember: ('Vars -> bool)) : M<'T, 'Vars>  =
            let newIsMember2 = isMember model.Variables
            this.SetModel model { model.Model with IsMember2 = newIsMember2 }

        [<CustomOperation("Member", MaintainsVariableSpaceUsingBind = true)>]
        member this.addMember (model: M<'T, 'Vars>, [<ProjectionParameter>] item: ('Vars -> 'TItem))  : M<'T, 'Vars>  =
            this.SetModel model { model.Model with Members = List.append model.Model.Members [ item model.Variables ] }

        // Note, using ParamArray doesn't work in conjunction with ProjectionParameter
        [<CustomOperation("Members", MaintainsVariableSpaceUsingBind = true)>]
        member this.addMembers (model: M<'T, 'Vars>, [<ProjectionParameter>] items: ('Vars -> 'TItem list)) : M<'T, 'Vars>  =
            this.SetModel model { model.Model with Members = List.append model.Model.Members (items model.Variables) }


