let rec Recursive i =
    if (i % 1000 = 0) then
        System.Console.WriteLine(i)

    Recursive (i + 1)

[<EntryPoint>]
let main argv = 
    Recursive 0
    0