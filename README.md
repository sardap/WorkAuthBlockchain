# Work History Authentication Verification Service

This is a smiple prototype for authenticating work history using ethereum.

## Getting Started

You will need a copy of geth <https://github.com/ethereum/go-ethereum> 

```
geth --datadir . -rpc --rpcapi db,eth,net,web3,personal,web3 --rpccorsdomain "http://localhost:8000" --networkid 1114
```

Then open up the solution file using visual studio 2017 and run the desired project.