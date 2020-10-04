namespace Flock.Core

open Flock.Core.Models.FlockEnvironment


module Env =

    /// Initialize the environment.
    let init (config: Config) =
        
        // Start up the event handler.
        
        // Start the action scheduler.
        
        
        
        
        Ok ()
    
    /// Run the environment, this will internally call `init`.
    let run (config: Config) =
  
        match init config with
        | Ok _ ->
            let rec loop() = async {
                
                return! loop()            
            }
            loop() |> Async.RunSynchronously |> ignore
        | Error _ -> () // TODO Log error.