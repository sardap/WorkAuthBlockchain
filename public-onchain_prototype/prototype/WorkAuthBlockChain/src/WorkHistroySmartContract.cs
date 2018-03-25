using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts;
using Nethereum.Web3;

namespace WorkAuthBlockChain
{

	public class WorkHistroySmartContractInvaildDataException : Exception
	{
		public WorkHistroySmartContractInvaildDataException()
		{
		}

		public WorkHistroySmartContractInvaildDataException(string message) : base(message)
		{
		}

		public WorkHistroySmartContractInvaildDataException(string message, Exception inner) : base(message, inner)
		{
		}
	}

	public class WorkHistroySmartContract
    {
		public const string ABI = @"[{""constant"":true, ""inputs"":[],""name"":""getCreator"",""outputs"":[{""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getData"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getHash"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""name"":""data"",""type"":""string""},{""name"":""hash"",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""}]";
		public const string BYTE_CODE = "6060604052341561000f57600080fd5b6040516103d53803806103d583398101604052808051820191906020018051906020019091905050816000908051906020019061004d92919061009d565b508060018190555033600260006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff1602179055505050610142565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f106100de57805160ff191683800117855561010c565b8280016001018555821561010c579182015b8281111561010b5782518255916020019190600101906100f0565b5b509050610119919061011d565b5090565b61013f91905b8082111561013b576000816000905550600101610123565b5090565b90565b610284806101516000396000f300606060405260043610610057576000357c0100000000000000000000000000000000000000000000000000000000900463ffffffff1680630ee2cb101461005c5780633bc5de30146100b1578063d13319c41461013f575b600080fd5b341561006757600080fd5b61006f610168565b604051808273ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200191505060405180910390f35b34156100bc57600080fd5b6100c4610192565b6040518080602001828103825283818151815260200191508051906020019080838360005b838110156101045780820151818401526020810190506100e9565b50505050905090810190601f1680156101315780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b341561014a57600080fd5b61015261023a565b6040518082815260200191505060405180910390f35b6000600260009054906101000a900473ffffffffffffffffffffffffffffffffffffffff16905090565b61019a610244565b60008054600181600116156101000203166002900480601f0160208091040260200160405190810160405280929190818152602001828054600181600116156101000203166002900480156102305780601f1061020557610100808354040283529160200191610230565b820191906000526020600020905b81548152906001019060200180831161021357829003601f168201915b5050505050905090565b6000600154905090565b6020604051908101604052806000815250905600a165627a7a723058203b6d3af13aa3aa8ac954a19a8a7826010e04050722a39032b7def6c251bb039f0029";
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

		public async Task<string> Deploy(string encryptedData, int rawDataHashCode)
		{
			_trasnactionHash = await _web3.Eth.DeployContract.SendRequestAsync(ABI, BYTE_CODE, _senderAddress, GAS_LIMIT, encryptedData, rawDataHashCode);

			TransactionReceipt recpit = null;
			while (recpit == null)
			{
				recpit = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(_trasnactionHash);
			}

			return recpit.ContractAddress;
		}

		public async Task<int> GetHash()
		{
			Function getHashFunction = _contract.GetFunction("getHash");

			int onChainHash = await getHashFunction.CallAsync<int>();

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

			return creatorAddress;
		}

		public string DataVaild(string data)
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
