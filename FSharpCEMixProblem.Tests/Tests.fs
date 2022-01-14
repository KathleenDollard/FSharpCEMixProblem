module Tests

open System
open Xunit
// KAD-Don/Chet: I had to add a namespace to Program.fs in CEMix is that expected
open Test

[<Fact>]
let ``Name`` () =
    let expected = Person.Create "Fred"
        
    let actual =
        Member() { Name "Fred" }
    Assert.Equal(expected, actual.Model)


[<Fact>]
let ``Repeated settings `` () =
    let expected = 
        { Person.Create "Fred" with 
            IsMember = Some true
            IsMember2 = true }   
        
    let actual =
        Member() {
            Name "George" 
            Name "Fred" 
            IsMember
            IsMember
            IsMember2 true // Note, I can call this twice without compiler error, but not Name in z5
            IsMember2 true
            }
    Assert.Equal(expected, actual.Model)

[<Fact>]
let ``Bool without option`` () =
    let expected = 
        { Person.Create "Fred" with 
            IsMember2 = true }   
         
    let actual = 
        Member() 
            {
            Name "Fred" 
            IsMember2 true
            }
    Assert.Equal(expected, actual.Model)

[<Fact>]
let ``Just bool option issue`` () =
    let expected = 
        { Person.Create() with
            IsMember2 = true }   
        
    let actual = Member() {
        IsMember2 true
        }
    Assert.Equal(expected, actual.Model)

[<Fact>]
let ``Single phone number`` () =
    let expected = 
        { Person.Create() with 
            PhoneNumbers = ["123 456 89"] }   
        
    let actual = Member() {
        "123 456 89" }
    Assert.Equal(expected, actual.Model)

    let expected = 
        { Person.Create() with 
            PhoneNumbers = ["123 456 89"] }   
        
    let actual = Member() {
        PhoneNumber "123 456 89" }
    Assert.Equal(expected, actual.Model)

[<Fact>]
let ``Phone numbers with prefixing command`` () =
    let expected = 
        { Person.Create() with 
            PhoneNumbers = ["123 456 89"; "23 456 890"] }   
        
    let actual = Member() {
        PhoneNumbers [ "123 456 89"; "23 456 890"] }
    Assert.Equal(expected, actual.Model)

[<Fact>]
let ``Phone numbers without prefix and name`` () =
    let expected = 
        { Person.Create "Fred" with 
            PhoneNumbers = ["123 456 89"; "23 456 890"] }   
    let actual = Member() {
        Name "Fred"
        "123 456 89"
        "23 456 890" }
    Assert.Equal(expected, actual.Model)

[<Fact>]
let ``Phone numbers with prefix and name`` () =
    let expected = 
        { Person.Create "Fred" with 
            PhoneNumbers = ["123 456 89"; "23 456 890"] }   
    let actual = Member() {
        Name "Fred"
        PhoneNumber "123 456 89"
        PhoneNumber "23 456 890" }
    Assert.Equal(expected, actual.Model)

[<Fact>]
let ``Let binding with concatenated name`` () =
    let expected = 
        { Person.Create "Fred and George" with 
            IsMember = Some true }   
        
    let actual = Member() { 
        let x = " and George"
        Name ("Fred" + x)
        IsMember }
    Assert.Equal(expected, actual.Model)

[<Fact>]
let ``IsLifetime`` () =
    let expected = 
        { Person.Create "Fred" with 
            IsLifetime = Some true }   
        
    let actual = Member() { 
        Name ("Fred")
        IsLifetime true }
    Assert.Equal(expected, actual.Model)


[<Fact>]
let ``IsLifetime via IsMember`` () =
    let expected = 
        { Person.Create "Fred" with
            IsMember = Some true
            IsLifetime = Some true }   
        
    let actual = Member() { 
        Name ("George") 
        IsMember true
        Name ("Fred") }
    Assert.Equal(expected, actual.Model)


[<Fact>]
let ``IsLifetime via IsMember LifetimeWord`` () =
    let expected = 
        { Person.Create "Fred" with
            IsMember = Some true
            IsLifetime = Some true }   
        
    let actual = Member() { 
        Name ("George") 
        IsMember Lifetime
        Name ("Fred") }
    Assert.Equal(expected, actual.Model)

