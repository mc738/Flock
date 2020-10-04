#!/bin/bash

# Start node01.
geth --identity "node01" --rpc --rpcport "8000" --rpccorsdomain "*" --datadir ./Data/node01 --port "30303" --nodiscover --rpcapi "db,eth,net,web3,personal,miner,admin" --networkid 1900 --nat "any" --allow-insecure-unlock
