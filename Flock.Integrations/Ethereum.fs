namespace Flock.Integrations.Ethereum

open System
open System.Numerics
open Nethereum.ABI.FunctionEncoding.Attributes
open Nethereum.Contracts
open Nethereum.Model
open Nethereum.Util
open Nethereum.Web3
open Nethereum.Web3.Accounts

module Models =
    
    // Byte code from https://docs.nethereum.com/en/latest/nethereum-smartcontrats-gettingstarted/.    
    let erc20ByteCode = "0x60606040526040516020806106f5833981016040528080519060200190919050505b80600160005060003373ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005081905550806000600050819055505b506106868061006f6000396000f360606040523615610074576000357c010000000000000000000000000000000000000000000000000000000090048063095ea7b31461008157806318160ddd146100b657806323b872dd146100d957806370a0823114610117578063a9059cbb14610143578063dd62ed3e1461017857610074565b61007f5b610002565b565b005b6100a060048080359060200190919080359060200190919050506101ad565b6040518082815260200191505060405180910390f35b6100c36004805050610674565b6040518082815260200191505060405180910390f35b6101016004808035906020019091908035906020019091908035906020019091905050610281565b6040518082815260200191505060405180910390f35b61012d600480803590602001909190505061048d565b6040518082815260200191505060405180910390f35b61016260048080359060200190919080359060200190919050506104cb565b6040518082815260200191505060405180910390f35b610197600480803590602001909190803590602001909190505061060b565b6040518082815260200191505060405180910390f35b600081600260005060003373ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005060008573ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600050819055508273ffffffffffffffffffffffffffffffffffffffff163373ffffffffffffffffffffffffffffffffffffffff167f8c5be1e5ebec7d5bd14f71427d1e84f3dd0314c0f7b2291e5b200ac8c7c3b925846040518082815260200191505060405180910390a36001905061027b565b92915050565b600081600160005060008673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600050541015801561031b575081600260005060008673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005060003373ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000505410155b80156103275750600082115b1561047c5781600160005060008573ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000828282505401925050819055508273ffffffffffffffffffffffffffffffffffffffff168473ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef846040518082815260200191505060405180910390a381600160005060008673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008282825054039250508190555081600260005060008673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005060003373ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000828282505403925050819055506001905061048656610485565b60009050610486565b5b9392505050565b6000600160005060008373ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000505490506104c6565b919050565b600081600160005060003373ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600050541015801561050c5750600082115b156105fb5781600160005060003373ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008282825054039250508190555081600160005060008573ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000828282505401925050819055508273ffffffffffffffffffffffffffffffffffffffff163373ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef846040518082815260200191505060405180910390a36001905061060556610604565b60009050610605565b5b92915050565b6000600260005060008473ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005060008373ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005054905061066e565b92915050565b60006000600050549050610683565b9056"
 
    
    type ERC20() = 
        
        inherit ContractDeploymentMessage(erc20ByteCode)
         // member val GasPrice = Nethereum.Web3.Web3.Convert.ToWei(25, UnitConversion.EthUnit.Gwei)
            
        [<Parameter("uint256", "totalSupply")>]
        member val TotalSupply = BigInteger 0 with get, set

    [<Function("balanceOf", "uint256")>]
    type BalanceOfFunction() =
        inherit FunctionMessage()

        [<Parameter("address", "_owner", 1)>]
        member val Owner = String.Empty with get, set


    [<Function("transfer", "bool")>]
    type TransferFunction() =
        inherit FunctionMessage()

        [<Parameter("address", "_to", 1)>]
        member val To = String.Empty with get, set

        [<Parameter("uint256", "_value", 2)>]
        member val TokenAmount = BigInteger 0 with get, set

    [<Event("Transfer")>]
    type TransferEventDTO() =
        interface IEventDTO
        
        [<Parameter("address", "_from", 1, true)>]
        member val From = String.Empty with get, set
        
        [<Parameter("address", "_to", 2, true)>]
        member val To = String.Empty with get, set
        
        [<Parameter("uint256", "_value", 3, false)>]
        member val Value = BigInteger 0 with get, set

module Handler =
    
    let createInstance (url:string) (privateKey:string) =
        let chainId = Option.toNullable (Some (BigInteger 444444444500L))
        let account = Account(privateKey, chainId)
       
        Web3(account, url)
        
        
    let deployContract (instance:Web3) (contract:Models.ERC20) = async {
        
        contract.GasPrice <- Option.toNullable (Some (Web3.Convert.ToWei(25, UnitConversion.EthUnit.Wei)))
        
        let deploymentHandler = instance.Eth.GetContractDeploymentHandler<Models.ERC20>()
      
        let! transactionReceipt = deploymentHandler.SendRequestAndWaitForReceiptAsync(contract) |> Async.AwaitTask
        let address = transactionReceipt.ContractAddress
        
        return Ok transactionReceipt
    }
        
    let queryBalance (instance: Web3) address func = async {
        let handler = instance.Eth.GetContractQueryHandler<Models.BalanceOfFunction>()
        return! handler.QueryAsync<BigInteger>(address, func) |> Async.AwaitTask        
    }
    
    let transfer (instance: Web3) address func = async {
        let handler = instance.Eth.GetContractTransactionHandler<Models.TransferFunction>()
        return! handler.SendRequestAndWaitForReceiptAsync(address, func) |> Async.AwaitTask
    }