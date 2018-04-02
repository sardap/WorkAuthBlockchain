﻿using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts;
using Nethereum.Web3;
using Nethereum.ABI.Decoders;
using Nethereum.ABI;
using System.Text;

namespace WorkAuthBlockChain
{

	public class WorkHistroySmartContractInValidDataException : Exception
	{
		public WorkHistroySmartContractInValidDataException()
		{
		}

		public WorkHistroySmartContractInValidDataException(string message) : base(message)
		{
		}

		public WorkHistroySmartContractInValidDataException(string message, Exception inner) : base(message, inner)
		{
		}
	}

	public class WorkHistroySmartContract
    {
		public const string ABI = @"[{""constant"":true, ""inputs"":[],""name"":""getCreator"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getData"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""b"",""type"":""bytes1""}],""name"":""char"",""outputs"":[{""name"":""c"",""type"":""bytes1""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getHash"",""outputs"":[{""name"":"""",""type"":""bytes""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""name"":""data"",""type"":""string""},{""name"":""hash"",""type"":""bytes""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""}]";
		public const string BYTE_CODE = "6060604052341561000f57600080fd5b60405161095138038061095183398101604052808051820191906020018051820191905050816000908051906020019061004a9291906100aa565b50806001908051906020019061006192919061012a565b5033600260006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff16021790555050506101cf565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f106100eb57805160ff1916838001178555610119565b82800160010185558215610119579182015b828111156101185782518255916020019190600101906100fd565b5b50905061012691906101aa565b5090565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f1061016b57805160ff1916838001178555610199565b82800160010185558215610199579182015b8281111561019857825182559160200191906001019061017d565b5b5090506101a691906101aa565b5090565b6101cc91905b808211156101c85760008160009055506001016101b0565b5090565b90565b610773806101de6000396000f300606060405260043610610062576000357c0100000000000000000000000000000000000000000000000000000000900463ffffffff1680630ee2cb10146100675780633bc5de30146100f557806369f9ad2f14610183578063d13319c414610220575b600080fd5b341561007257600080fd5b61007a6102ae565b6040518080602001828103825283818151815260200191508051906020019080838360005b838110156100ba57808201518184015260208101905061009f565b50505050905090810190601f1680156100e75780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b341561010057600080fd5b6101086104df565b6040518080602001828103825283818151815260200191508051906020019080838360005b8381101561014857808201518184015260208101905061012d565b50505050905090810190601f1680156101755780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b341561018e57600080fd5b6101c660048080357effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916906020019091905050610587565b60405180827effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff19167effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916815260200191505060405180910390f35b341561022b57600080fd5b610233610677565b6040518080602001828103825283818151815260200191508051906020019080838360005b83811015610273578082015181840152602081019050610258565b50505050905090810190601f1680156102a05780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b6102b661071f565b6102be610733565b60008060008060286040518059106102d35750595b9080825280601f01601f19166020018201604052509450600093505b60148410156104d4578360130360080260020a600260009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681151561034457fe5b047f01000000000000000000000000000000000000000000000000000000000000000292506010837f0100000000000000000000000000000000000000000000000000000000000000900460ff1681151561039b57fe5b047f0100000000000000000000000000000000000000000000000000000000000000029150817f01000000000000000000000000000000000000000000000000000000000000009004601002837f01000000000000000000000000000000000000000000000000000000000000009004037f010000000000000000000000000000000000000000000000000000000000000002905061043982610587565b858560020281518110151561044a57fe5b9060200101907effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916908160001a90535061048381610587565b856001866002020181518110151561049757fe5b9060200101907effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916908160001a90535083806001019450506102ef565b849550505050505090565b6104e761071f565b60008054600181600116156101000203166002900480601f01602080910402602001604051908101604052809291908181526020018280546001816001161561010002031660029004801561057d5780601f106105525761010080835404028352916020019161057d565b820191906000526020600020905b81548152906001019060200180831161056057829003601f168201915b5050505050905090565b6000600a7f010000000000000000000000000000000000000000000000000000000000000002827effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff19161015610626576030827f01000000000000000000000000000000000000000000000000000000000000009004017f0100000000000000000000000000000000000000000000000000000000000000029050610672565b6057827f01000000000000000000000000000000000000000000000000000000000000009004017f01000000000000000000000000000000000000000000000000000000000000000290505b919050565b61067f610733565b60018054600181600116156101000203166002900480601f0160208091040260200160405190810160405280929190818152602001828054600181600116156101000203166002900480156107155780601f106106ea57610100808354040283529160200191610715565b820191906000526020600020905b8154815290600101906020018083116106f857829003601f168201915b5050505050905090565b602060405190810160405280600081525090565b6020604051908101604052806000815250905600a165627a7a723058204453c8a7c6554244d9d349e359f78f5b21b4206f52eafd9708c40d3b2da138d30029";
		public const int LOGIN_TIMEOUT = 60;
		private static readonly HexBigInteger GAS_LIMIT = new HexBigInteger(3000000);

		private Nethereum.Web3.Web3 _web3 = new Web3();
		private string _senderAddress;
		private string _senderPassword;
		private string _trasnactionHash;
		private Nethereum.Contracts.Contract _contract;

		public RSACryptoServiceProvider RSA
		{
			get;
			set;
		}

		public void LoadContract(string address)
		{
			_contract = _web3.Eth.GetContract(ABI, address);
		}

		public async Task<bool> UnlockAccount(string address, string password)
		{
			_senderAddress = address;
			_senderPassword = password;

			return await _web3.Personal.UnlockAccount.SendRequestAsync(_senderAddress, _senderPassword, LOGIN_TIMEOUT);
		}

		public async Task<string> Deploy(string encryptedData, byte[] rawDataHashCode)
		{
			_trasnactionHash = await _web3.Eth.DeployContract.SendRequestAsync(ABI, BYTE_CODE, _senderAddress, GAS_LIMIT, encryptedData, rawDataHashCode);

			TransactionReceipt recpit = null;
			while (recpit == null)
			{
				recpit = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(_trasnactionHash);
			}

			return recpit.ContractAddress;
		}

		public async Task<byte[]> GetHash()
		{
			Function getHashFunction = _contract.GetFunction("getHash");

			byte[] onChainHash = await getHashFunction.CallAsync<byte[]>();

			return onChainHash;
		}

		public async Task<string> GetData()
		{
			Function getDataFunction = _contract.GetFunction("getData");

			string encyptedData = await getDataFunction.CallAsync<string>();

			return encyptedData;
		}

		public async Task<string> GetCreator()
		{
			Function getCreatorFunction = _contract.GetFunction("getCreator");

			string creatorAddress = await getCreatorFunction.CallAsync<string>();

			return "0x" + creatorAddress;
		}

		public string DataValid(string data)
		{
			string[] dataSplit = data.Split(',');
			string error = "";
			Regex urlRegex = new Regex(
				@"^([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$",
				RegexOptions.IgnoreCase
			);
			Match m = urlRegex.Match(dataSplit[0]);

			if (m == null)
			{
				error += "URL must be before first ,";
			}


			return error;
		}



	}
}
