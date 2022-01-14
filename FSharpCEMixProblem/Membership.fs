namespace Test

open CEBase
open System

type LifetimeWord =
    Lifetime

type Person = 
    { Name: string option
      Name2: string
      IsMember: bool option
      IsMember2: bool  // confirming that shape of the container is not restricted, just need clear default the CE understands
      PhoneNumbers: string list
      MembershipNumber: string
      IsLifetime: bool option}
    static member Create() =
        { Name = None
          Name2 = ""
          IsMember = None
          IsMember2 = false
          PhoneNumbers = []
          MembershipNumber = "<Unknown>"
          IsLifetime = None }
    static member Create name =
        { Name = Some name
          Name2 = ""
          IsMember = None
          IsMember2 = false
          PhoneNumbers = []
          MembershipNumber = "<Unknown>"
          IsLifetime = None }   

type Member() =
    inherit DslBase<Person, string>()

    override _.Empty() = Person.Create()

    override _.CombineModels model1 model2 =
        let newName = 
            match model2.Name with
            | None -> model1.Name
            | res -> res
        let newName2 = 
            if String.IsNullOrWhiteSpace model2.Name2 then model1.Name2
            else model2.Name2
        let newIsMember =
            match model2.IsMember with 
            | None -> model1.IsMember
            | res -> res
        let newIsMember2 =
            match model2.IsMember2 with 
            | true -> true
            | res -> model1.IsMember2
        let newMembershipNumber =
            if String.IsNullOrWhiteSpace(model2.MembershipNumber) then model1.MembershipNumber
            elif model2.MembershipNumber = "<Unknown>" 
                && not (String.IsNullOrWhiteSpace(model1.MembershipNumber))  then model1.MembershipNumber
            else model2.MembershipNumber
        let newIsLifetime =
            match model2.IsLifetime with 
            | None -> model1.IsLifetime
            | res -> res
        { Name = newName
          Name2 = newName2
          IsMember = newIsMember
          IsMember2 = newIsMember2
          PhoneNumbers =  model1.PhoneNumbers @ model2.PhoneNumbers 
          MembershipNumber = newMembershipNumber
          IsLifetime = newIsLifetime }

    override this.NewMember item =
        { this.Empty() with PhoneNumbers = [ item ] }

    [<CustomOperation("Name", MaintainsVariableSpaceUsingBind = true)>]
    member this.setName (varModel, [<ProjectionParameter>] name)   =
        this.SetModel varModel { varModel.Model with Name = Some (name varModel.Variables) }

    [<CustomOperation("Name2", MaintainsVariableSpaceUsingBind = true)>]
    member this.setName2 (varModel, [<ProjectionParameter>] name)   =
        this.SetModel varModel { varModel.Model with Name2 = (name varModel.Variables) }

    //[<CustomOperation("IsMember", MaintainsVariableSpaceUsingBind = true)>]
    //member this.setIsMember (varModel)  =
    //    this.SetModel varModel { varModel.Model with IsMember = Some true }

    // We can skip 
    [<CustomOperation("IsMember2", MaintainsVariableSpaceUsingBind = true)>]
    member this.setIsMember2 (varModel, [<ProjectionParameter>] isMember) =
        let newIsMember2 = isMember varModel.Variables
        this.SetModel varModel { varModel.Model with IsMember2 = newIsMember2 }

    [<CustomOperation("PhoneNumber", MaintainsVariableSpaceUsingBind = true)>]
    member this.addMember (varModel, [<ProjectionParameter>] item) =
        this.SetModel varModel { varModel.Model with PhoneNumbers = List.append varModel.Model.PhoneNumbers [ item varModel.Variables ] }

    // Note, using ParamArray doesn't work in conjunction with ProjectionParameter
    [<CustomOperation("PhoneNumbers", MaintainsVariableSpaceUsingBind = true)>]
    member this.addMembers (varModel, [<ProjectionParameter>] items) =
        this.SetModel varModel { varModel.Model with PhoneNumbers = List.append varModel.Model.PhoneNumbers (items varModel.Variables) }

    [<CustomOperation("IsLifetime", MaintainsVariableSpaceUsingBind = true)>]
    member this.setIsLifetime (varModel, [<ProjectionParameter>] isLifetime)  =
        let newValue = (isLifetime varModel.Variables)
        this.SetModel varModel { varModel.Model with IsLifetime = Some newValue }

    // KAD-Don: This one needed not be just the bool, and I don't know why it works
    [<CustomOperation("IsMember", MaintainsVariableSpaceUsingBind = true)>]
    member this.setIsMember (varModel,  isLifetime)  =
        this.SetModel varModel 
            { varModel.Model with 
                IsLifetime = Some isLifetime 
                IsMember = Some true
            }

    // KAD-Don: This one needed not be just the bool, and I don't know why it works
    [<CustomOperation("IsMember", MaintainsVariableSpaceUsingBind = true)>]
    member this.setIsMember (varModel,  ?lifetime: LifetimeWord)  =
        let isLifetime = 
            match lifetime with 
            | None -> None
            | _ -> Some true
        this.SetModel varModel 
            { varModel.Model with 
                IsLifetime = isLifetime
                IsMember = Some true
            }



