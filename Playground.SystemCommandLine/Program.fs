open System.CommandLine
open System.IO
open System.Threading.Tasks

let DoStuff(fi: FileInfo, tags: string[]) =
    task {
            let tagSet = Set.ofArray tags
            let! lines = System.IO.File.ReadAllLinesAsync fi.FullName
            let matchingLines = 
                lines
                |> Array.filter (fun line -> 
                    let words = line.Split(' ')
                    words |> Array.exists tagSet.Contains
                )
                |> Array.length
            return matchingLines
        } :> Task

let CreateCli() : RootCommand =
    let tagCommand = Command("tag-extract", "Get content with the given tag")
    let inputfileArg = Argument<FileInfo>("input-file", "the file to extract content from")
    tagCommand.AddArgument inputfileArg
    let tagsOpt = Option<string[]>([| "--tags"; "-t"|], "One or more tags marking content to extract")
    tagCommand.AddOption tagsOpt
    let root = RootCommand("Example Description")
    root.AddCommand tagCommand
    tagCommand.SetHandler(DoStuff, inputfileArg, tagsOpt)
    root

//let CreateCliPossible() : RootCommand =
//    let tagCommand = Command("tag-extract", "Get content with the given tag")
//    let inputFileArg = tagCommand.AddArgument (Argument<FileInfo>("input-file", "the file to extract content from"))
//    let tagsOpt = tagCommand.AddOption (Option<string[]>([| "--tags"; "-t"|], "One or more tags marking content to extract"))
//    let root = RootCommand("Example Description")
//    root.AddCommand tagCommand
//    tagCommand.SetHandler(DoStuff, inputFileArg, tagsOpt)
//    root



[<EntryPoint>]
let main args =
    let root = CreateCli()
    root.Invoke args 