namespace Flock.Core.Models

open System
open System.Numerics

module Accounts =
    
    /// A system user.
    type User =
        { Username: string
          Reference: Guid
          Balance: BigInteger
          LastSynced: DateTime
          Address: string }

/// Models for handling `Supply Chain Tracking` (SCT).
module SCT =

    let i = ()

/// Models for handling the general environment of a flock instance.
module FlockEnvironment =

    type ChainType = | Ethereum

    type Config = { ChainType: ChainType; SyncRate: int }

    type Value = { Name: string; Data: ValueType }

    and ValueType =
        | Static of obj
        | FromDataSource
        | FromSettings
        | Deferred

    type Command = { Name: string }

    type SystemEvent =
        { Name: string
          Commands: Command seq option }

    type Action = { name: string }

    type PostBack =
        | Action of Action
        | Event of SystemEvent

    type ActionResult =
        { Name: string
          Success: bool
          Events: SystemEvent seq option }