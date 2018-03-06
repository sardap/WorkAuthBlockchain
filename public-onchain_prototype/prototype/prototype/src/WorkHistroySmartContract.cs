using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts;

namespace prototype.src
{
    class WorkHistroySmartContract
    {
		const string ABI = @"[{""constant"":true,""inputs"":[],""name"":""getData"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getHash"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""name"":""data"",""type"":""string""},{""name"":""hash"",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""}]";
		const string BYTE_CODE = "6060604052341561000f57600080fd5b60405161030a38038061030a83398101604052808051820191906020018051906020019091905050816000908051906020019061004d92919061005c565b50806001819055505050610101565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f1061009d57805160ff19168380011785556100cb565b828001600101855582156100cb579182015b828111156100ca5782518255916020019190600101906100af565b5b5090506100d891906100dc565b5090565b6100fe91905b808211156100fa5760008160009055506001016100e2565b5090565b90565b6101fa806101106000396000f30060606040526004361061004c576000357c0100000000000000000000000000000000000000000000000000000000900463ffffffff1680633bc5de3014610051578063d13319c4146100df575b600080fd5b341561005c57600080fd5b610064610108565b6040518080602001828103825283818151815260200191508051906020019080838360005b838110156100a4578082015181840152602081019050610089565b50505050905090810190601f1680156100d15780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b34156100ea57600080fd5b6100f26101b0565b6040518082815260200191505060405180910390f35b6101106101ba565b60008054600181600116156101000203166002900480601f0160208091040260200160405190810160405280929190818152602001828054600181600116156101000203166002900480156101a65780601f1061017b576101008083540402835291602001916101a6565b820191906000526020600020905b81548152906001019060200180831161018957829003601f168201915b5050505050905090565b6000600154905090565b6020604051908101604052806000815250905600a165627a7a723058200d2f4e5a9290eb4806ba24609334b245e101d733b68118c64fc036bf87399e6c0029";
		const int LOGIN_TIMEOUT = 60;
		const int RSA_KEY_LENGTH = 1024;
		private static readonly HexBigInteger GAS_LIMIT = new HexBigInteger(3000000);

		private string EncyptData(string data, string publicKiey)
		{
			using (var rsa = new RSACryptoServiceProvider(RSA_KEY_LENGTH))
			{

				try
				{
					RSACryptoServiceProviderExtensions.FromXmlString(rsa, publicKiey);

					byte[] dataInBytes = Encoding.UTF8.GetBytes(data);
					byte[] encryptedData = rsa.Encrypt(dataInBytes, true);

					return Convert.ToBase64String(encryptedData);
				}
				finally
				{
					rsa.PersistKeyInCsp = false;
				}

			}
		}

		private Contract GetContract(string contractAddress)
		{
			Web3 web3 = new Web3();

			return web3.Eth.GetContract(ABI, contractAddress);
		}

		private string DecryptData(string encryptedData, string secretKey)
		{
			using (var rsa = new RSACryptoServiceProvider(RSA_KEY_LENGTH))
			{

				try
				{
					RSACryptoServiceProviderExtensions.FromXmlString(rsa, secretKey);

					var resultBytes = Convert.FromBase64String(encryptedData);
					var decryptedBytes = rsa.Decrypt(resultBytes, true);
					var decryptedData = Encoding.UTF8.GetString(decryptedBytes);

					return decryptedData;
				}
				finally
				{
					rsa.PersistKeyInCsp = false;
				}
			}
		}

		public async Task<string> DecryptDataFromContract(string targetAddressSecretKey, string contractAddress)
		{
			Web3 web3 = new Web3();

			Function getDataFunction = GetContract(contractAddress).GetFunction("getData");

			string encyptedData = await getDataFunction.CallAsync<string>();

			return DecryptData(encyptedData, targetAddressSecretKey);
		}

		public async Task<string> PublishWorkHistoryAsync(string data, string senderAddress, string senderPassword, string dataSubjectPK)
		{
			Web3 web3 = new Web3();

			bool unlockAcountResult = await web3.Personal.UnlockAccount.SendRequestAsync(senderAddress, senderPassword, LOGIN_TIMEOUT);

			string encryptedData = EncyptData(data, dataSubjectPK);

			string trasnactionHash = await web3.Eth.DeployContract.SendRequestAsync(ABI, BYTE_CODE, senderAddress, GAS_LIMIT, encryptedData, data.GetHashCode());

			return trasnactionHash;
		}

		public async Task<bool> CompareHashAsync(string contractAddress, string targetAddress, string data)
		{
			Function getHashFunction = GetContract(contractAddress).GetFunction("getHash");

			int onChainHash = await getHashFunction.CallAsync<int>();

			return onChainHash == data.GetHashCode();
		}

	}
}
