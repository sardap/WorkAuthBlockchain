using System;
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
		public const string ABI = @"[{""constant"":true, ""inputs"":[],""name"":""getCreator"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getData"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""b"",""type"":""bytes1""}],""name"":""char"",""outputs"":[{""name"":""c"",""type"":""bytes1""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getHash"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""name"":""data"",""type"":""string""},{""name"":""hash"",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""}]";
public const string BYTE_CODE = "6060604052341561000f57600080fd5b6040516107c13803806107c183398101604052808051820191906020018051906020019091905050816000908051906020019061004d92919061009d565b508060018190555033600260006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff1602179055505050610142565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f106100de57805160ff191683800117855561010c565b8280016001018555821561010c579182015b8281111561010b5782518255916020019190600101906100f0565b5b509050610119919061011d565b5090565b61013f91905b8082111561013b576000816000905550600101610123565b5090565b90565b610670806101516000396000f300606060405260043610610062576000357c0100000000000000000000000000000000000000000000000000000000900463ffffffff1680630ee2cb10146100675780633bc5de30146100f557806369f9ad2f14610183578063d13319c414610220575b600080fd5b341561007257600080fd5b61007a610249565b6040518080602001828103825283818151815260200191508051906020019080838360005b838110156100ba57808201518184015260208101905061009f565b50505050905090810190601f1680156100e75780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b341561010057600080fd5b61010861047a565b6040518080602001828103825283818151815260200191508051906020019080838360005b8381101561014857808201518184015260208101905061012d565b50505050905090810190601f1680156101755780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b341561018e57600080fd5b6101c660048080357effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916906020019091905050610522565b60405180827effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff19167effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916815260200191505060405180910390f35b341561022b57600080fd5b610233610612565b6040518082815260200191505060405180910390f35b61025161061c565b610259610630565b600080600080602860405180591061026e5750595b9080825280601f01601f19166020018201604052509450600093505b601484101561046f578360130360080260020a600260009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168115156102df57fe5b047f01000000000000000000000000000000000000000000000000000000000000000292506010837f0100000000000000000000000000000000000000000000000000000000000000900460ff1681151561033657fe5b047f0100000000000000000000000000000000000000000000000000000000000000029150817f01000000000000000000000000000000000000000000000000000000000000009004601002837f01000000000000000000000000000000000000000000000000000000000000009004037f01000000000000000000000000000000000000000000000000000000000000000290506103d482610522565b85856002028151811015156103e557fe5b9060200101907effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916908160001a90535061041e81610522565b856001866002020181518110151561043257fe5b9060200101907effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916908160001a905350838060010194505061028a565b849550505050505090565b61048261061c565b60008054600181600116156101000203166002900480601f0160208091040260200160405190810160405280929190818152602001828054600181600116156101000203166002900480156105185780601f106104ed57610100808354040283529160200191610518565b820191906000526020600020905b8154815290600101906020018083116104fb57829003601f168201915b5050505050905090565b6000600a7f010000000000000000000000000000000000000000000000000000000000000002827effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff191610156105c1576030827f01000000000000000000000000000000000000000000000000000000000000009004017f010000000000000000000000000000000000000000000000000000000000000002905061060d565b6057827f01000000000000000000000000000000000000000000000000000000000000009004017f01000000000000000000000000000000000000000000000000000000000000000290505b919050565b6000600154905090565b602060405190810160405280600081525090565b6020604051908101604052806000815250905600a165627a7a7230582018fb35eed00dff9cbc8af64006378a5eade23722a568b6489ea41eedb255b7930029";
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
