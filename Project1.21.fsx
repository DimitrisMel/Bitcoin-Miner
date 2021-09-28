#r "nuget: Akka.FSharp"

open System
open Akka.FSharp
open System.Text
open System.Security.Cryptography

type BossMessage =
    | BitCoin of string * string
    | InitializeWorkers of string
    | Ready

let system = System.create "system" <| Configuration.load()

let blockSize = 100
let numberOfWorkers = 8
let chars = ["0";"1";"2";"3";"4";"5";"6";"7";"8";"9";"a";"b";"c";"d";"e";"f";"g";"h";"i";"j";"k";"l";"m";"n";"o";"p";"q";"r";"s";"t";"u";"v";"w";"x";"y";"z";"A";"B";"C";"D";"E";"F";"G";"H";"I";"J";"K";"L";"M";"N";"O";"P";"Q";"R";"S";"T";"U";"V";"W";"X";"Y";"Z"]
let mutable currentIndex = 0

//Read the number of leading zeros from the user
printfn "Enter the number of zeros: "
let input = Console.ReadLine()
let rec checkInt(input:string) =
    match (System.Int32.TryParse(input)) with
    | (true, value) ->
        if value >= 0 && value <= 5 then
            value
        else if value < 0 then
            printfn "The input is a negative number. Enter a positive integer for the number of zeros: "
            checkInt(Console.ReadLine())
        else
            printfn "WARNING: The input is too large. It is likely that no BitCoins will be found."
            printfn "For better results type an integer between 0 and 5."
            printfn "Starting mining now anyway..."
            value
    | (false, _) ->
        printfn "The input is not an integer. Enter an integer for the number of zeros: "
        checkInt(Console.ReadLine())
let zeros = checkInt input

//Create a string "checker", which we compare to the hashed values, to determine if we found a BitCoin
let checkerBuilder(zeros: int) : string =
    let mutable checker = ""
    for i in 1 .. zeros do
        checker <- checker + "0"
    checker <- checker + "f"
    checker
let checker = checkerBuilder(zeros)

let worker(mailbox: Actor<_>) =
    //Convert an integer to a string on base62
    let rec toBase x =
        let bse = List.length chars
        if x = 0 then
            chars.[0]
        else
            let y = x % bse
            (toBase ((x-y)/bse)) + (chars.[y])

    let rec loop() = actor {
        let mySHA256 = SHA256.Create()
        let! currentIndex = mailbox.Receive()
        for currentInt in currentIndex .. (currentIndex+blockSize-1) do
            let currentString = toBase currentInt
            let ufidString = "dmelissourgos" + ";" + currentString //Add the UFID
            let bytesArray = System.Text.Encoding.ASCII.GetBytes(ufidString)
            let hashedBytesArray = mySHA256.ComputeHash(bytesArray)
            let sb = StringBuilder(hashedBytesArray.Length * 2)
            hashedBytesArray |> Array.map (fun c -> sb.AppendFormat("{0:X2}",c)) |> ignore
            let hashed = sb.ToString()
            if hashed <= checker then
                mailbox.Sender() <! BitCoin (ufidString, hashed)
        mailbox.Sender() <! Ready
        return! loop()
    }
    loop()

let boss(mailbox: Actor<_>) =
    let rec loop() = actor {
        let! message = mailbox.Receive()
        match message with
        | BitCoin (ufidRandomString, hashed) ->
                printfn "%s \t %s" ufidRandomString hashed
        | InitializeWorkers workerID ->
            let workerRef = spawn system ("worker" + workerID) worker
            workerRef <! currentIndex
            currentIndex <- (currentIndex + blockSize)
        | Ready ->
            mailbox.Sender() <! currentIndex
            currentIndex <- (currentIndex + blockSize)
        return! loop()
    }
    loop()

let bossRef = spawn system "boss" boss
for i in 1 .. numberOfWorkers do
    bossRef <! InitializeWorkers (i.ToString())

#time
Console.ReadLine() |> ignore
