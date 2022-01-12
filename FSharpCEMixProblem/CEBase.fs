namespace CEBase 

    type membership<'TItem> = 
        { Name: string option
          IsMember: bool option
          IsMember2: bool  // confirming that shape of the container is not restricted, just need clear default the CE understands
          Members: 'TItem list }

    type M<'T, 'Vars> =
        { Model: 'T
          Variables: 'Vars }

    type M<'T> = M<'T, unit>

    type DslBase() =
        let empty() = 
             { Name = None
               IsMember = None
               IsMember2 = false
               Members = [] }

        let combine model1 model2 =
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

        member _.Zero() : M<'T, unit> = 
            { Model = empty() 
              Variables = () }

        member _.Combine (model1: M<'T, unit>, model2: M<'T, unit>) : M<'T, unit> =
            { Model = combine model1.Model model2.Model
              Variables = () }

        member _.Delay(f) : M<'T, 'Vars> = f()

        member _.Run(model: M<'T, 'Vars>) : M<'T, unit> =
            { Model = combine model.Model (empty())
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

        member _.Yield (item: 'TItem) : M<'T, unit> = 
            { Model = { empty() with Members = [ item ] }
              Variables = () }

        // Only for packing/unpacking the implicit variable space
        member _.Bind (model1: M<'T, 'Vars>, f: ('Vars -> M<'T, unit>)) : M<'T, unit>  =
            let model2 = f model1.Variables
            let combined = combine model1.Model model2.Model
            { Model = combine model1.Model model2.Model
              Variables = model2.Variables }

        // Only for packing/unpacking the implicit variable space
        member _.Return (varspace: 'Vars) : M<'T, 'Vars> = 
            { Model = empty() 
              Variables = varspace }

        member _.SetModel (model: M<'T, 'Vars>) (newModel: 'T) =
            { Model = newModel
              Variables = model.Variables }            

    type CE() =
        inherit DslBase()

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


