// Learn more about F# at http://fsharp.org

open System
open System.Numerics
open Flock.Console.Handler
open Flock.Console.Models

let address1 = "0x12890d2cce102216644c59daE5baed380d84830c"
let address2 = "0x13f022d72158410433cbd66f5dd8bf6d2d129924"

let instance = createInstance "http://127.0.0.1:8000" "0xb5b1870957d373ef0eeffecc6e4812c0fd08f554b37b233526acc331bf1544f7"

let contractAddress = "0x3b01809c8f10ae35ff2d38e3e29d7456cc442f48"

let deployContract = async {
    let! accounts = instance.Personal.ListAccounts.SendRequestAsync() |> Async.AwaitTask
    
    let! b = instance.Eth.GetBalance.SendRequestAsync(instance.TransactionManager.Account.Address) |> Async.AwaitTask   
    
    let contract = ERC20()
    
    contract.TotalSupply <- BigInteger 100000

    let! contractDetails = deployContract instance contract 
    
    match contractDetails with
    | Ok c ->
        printfn "Contract address: %s" c.ContractAddress
        System.IO.File.WriteAllText("./Sandbox/contract_id.txt", c.ContractAddress)
        ()
    | Error _ ->
        printfn "Couldn't deploy contract"
        ()

    return ()    
}


let getCredits address = async {
    
    let balanceMsg = BalanceOfFunction()
    
    balanceMsg.Owner <- address
    
    let handler = instance.Eth.GetContractQueryHandler<BalanceOfFunction>()
    
    return! handler.QueryAsync<BigInteger>(contractAddress, balanceMsg) |> Async.AwaitTask
}

let transferCredits toAddress amount = async {
    let handler = instance.Eth.GetContractTransactionHandler<TransferFunction>()
    
    let transfer = TransferFunction()
    
    transfer.To <- toAddress
    transfer.TokenAmount <- amount
    
    return! handler.SendRequestAndWaitForReceiptAsync(contractAddress, transfer) |> Async.AwaitTask
}
    
    

[<EntryPoint>]
let main argv =
    // Deploy the test contract.
    // deployContract |> Async.RunSynchronously |> ignore    
    let amount = BigInteger 100
    
    let receipt = transferCredits address2 amount  |> Async.RunSynchronously
    let balance1 = getCredits address1 |> Async.RunSynchronously
    let balance2 = getCredits address2 |> Async.RunSynchronously
    
    printfn "Hello World from F#!"
    0 // return an integer exit code
