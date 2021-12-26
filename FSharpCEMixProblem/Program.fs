namespace X

open System

type M<'T> =
    { Name: string
      MemberList: 'T list }


type CE() =
            
    member _.Zero() : M<'T> = { Name =""; MemberList = [] }
    member _.Combine (item1: M<'T>, item2: M<'T>) : M<'T> =
        let newName = if String.IsNullOrWhiteSpace(item2.Name) then item1.Name else item2.Name
        { Name = newName; MemberList = List.append item1.MemberList item2.MemberList }
    member _.Delay(f) : M<'T> = f()
    member this.For(methods, f) :M<'T> = 
        let methodList = Seq.toList methods
        match methodList with 
        | [] -> this.Zero()
        | [x] -> f(x)
        | head::tail ->
            let mutable headResult = f(head)
            for x in tail do 
                headResult <- this.Combine(headResult, f(x))
            headResult

    member this.Yield (()) : M<'T> = 
        this.Zero() 
    member this.Yield (item: 'T) : M<'T> = 
        { this.Zero() with MemberList = [ item ]}

    [<CustomOperation("Name")>]
    member _.setName (model: M<'T>, name: string) : M<'T>  =
        { model with Name = name }
    [<CustomOperation("Member")>]
    member _.addMembers (model: M<'T>, item)  =
        { model with MemberList = List.append model.MemberList [ item ] }
    [<CustomOperation("Members", MaintainsVariableSpace = true)>]
    member _.addMembers (model: M<'T>, [<ParamArray>] items: 'T[]) : M<'T>  =
        { model with MemberList = List.append model.MemberList (List.ofArray items) }

module Test =
    let x =
        CE() {
            Name "Fred"
        }

    let y = 
        CE() {
            42
        }

    let z =
        CE() {
            Member 42
        }

    let z2 =
        CE() {
            Members 42
        }

    let failure = 
        CE() {
            Name "Fred"
            42
        }

    
