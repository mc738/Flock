#! /bin/bash

# Make the folder to contain the block chain.
mkdir Data

# Make the node directories
mkdir ./Data/node01 ./Data/node02 ./Data/node03 ./Data/Users

# Create user password files later
echo "password" > ./Data/Users/user1_pw
echo "password" > ./Data/Users/user2_pw


# Add a user to the first node
geth --datadir ./Data/node01 account new --password ./Data/Users/user1_pw > ./Data/Users/user1_result
geth --datadir ./Data/node01 account new --password ./Data/Users/user2_pw > ./Data/Users/user2_result

echo "Environment created!"


