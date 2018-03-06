using System;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;

namespace prototype.src
{
    class Program
    {
		const int DATA_LENGTH = 86;

		public static async Task MainAsync()
		{
			string senderAddress = "0x25922333d41f0f3f40be629f81af6983634d0fb6";
			string targetAddress = "<RSAKeyValue><Modulus>21wEnTU+mcD2w0Lfo1Gv4rtcSWsQJQTNa6gio05AOkV/Er9w3Y13Ddo5wGtjJ19402S71HUeN0vbKILLJdRSES5MHSdJPSVrOqdrll/vLXxDxWs/U0UT1c8u6k/Ogx9hTtZxYwoeYqdhDblof3E75d9n2F0Zvf6iTb4cI7j6fMs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
			string targetAddressSecretKey = "<RSAKeyValue><Modulus>21wEnTU+mcD2w0Lfo1Gv4rtcSWsQJQTNa6gio05AOkV/Er9w3Y13Ddo5wGtjJ19402S71HUeN0vbKILLJdRSES5MHSdJPSVrOqdrll/vLXxDxWs/U0UT1c8u6k/Ogx9hTtZxYwoeYqdhDblof3E75d9n2F0Zvf6iTb4cI7j6fMs=</Modulus><Exponent>AQAB</Exponent><P>/aULPE6jd5IkwtWXmReyMUhmI/nfwfkQSyl7tsg2PKdpcxk4mpPZUdEQhHQLvE84w2DhTyYkPHCtq/mMKE3MHw==</P><Q>3WV46X9Arg2l9cxb67KVlNVXyCqc/w+LWt/tbhLJvV2xCF/0rWKPsBJ9MC6cquaqNPxWWEav8RAVbmmGrJt51Q==</Q><DP>8TuZFgBMpBoQcGUoS2goB4st6aVq1FcG0hVgHhUI0GMAfYFNPmbDV3cY2IBt8Oj/uYJYhyhlaj5YTqmGTYbATQ==</DP><DQ>FIoVbZQgrAUYIHWVEYi/187zFd7eMct/Yi7kGBImJStMATrluDAspGkStCWe4zwDDmdam1XzfKnBUzz3AYxrAQ==</DQ><InverseQ>QPU3Tmt8nznSgYZ+5jUo9E0SfjiTu435ihANiHqqjasaUNvOHKumqzuBZ8NRtkUhS6dsOEb8A2ODvy7KswUxyA==</InverseQ><D>cgoRoAUpSVfHMdYXW9nA3dfX75dIamZnwPtFHq80ttagbIe4ToYYCcyUz5NElhiNQSESgS5uCgNWqWXt5PnPu4XmCXx6utco1UVH8HGLahzbAnSy6Cj3iUIQ7Gj+9gQ7PkC434HTtHazmxVgIR5l56ZjoQ8yGNCPZnsdYEmhJWk=</D></RSAKeyValue>";
			string password = "passphrase";
			string data = new string('*', DATA_LENGTH);
			WorkHistroySmartContract workHistroySmartContract = new WorkHistroySmartContract();

			Console.WriteLine("Original Data: {0}", data);

			string trasnactionHash = await workHistroySmartContract.PublishWorkHistoryAsync(data, senderAddress, password, targetAddress);
			Web3 web3 = new Web3();

			TransactionReceipt recpit = null;

			while (recpit == null)
			{
				recpit = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(trasnactionHash);
			}

			bool match = await workHistroySmartContract.CompareHashAsync(recpit.ContractAddress, targetAddress, data);

			Console.WriteLine("Are they a match {0}", match);

			string decryptedData = await workHistroySmartContract.DecryptDataFromContract(targetAddressSecretKey, recpit.ContractAddress);

			Console.WriteLine("Decryped Data from blockchain: {0}", decryptedData);
		}


		static void Main()
		{
			MainAsync().GetAwaiter().GetResult();

			Console.ReadLine();
		}
	}
}
