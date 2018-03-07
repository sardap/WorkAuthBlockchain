using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Nethereum.Web3;

namespace prototype.src
{
    class Program
    {
		const int DATA_LENGTH = 428;

		public static async Task MainAsync()
		{
			using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(WorkHistroySmartContract.RSA_KEY_LENGTH))
			{
				try
				{

					string senderAddress = "0x25922333d41f0f3f40be629f81af6983634d0fb6";
					string password = "passphrase";
					string data = new string('*', DATA_LENGTH);
					WorkHistroySmartContract workHistroySmartContract = new WorkHistroySmartContract();
					workHistroySmartContract.RSA = rsa;

					DataCustodianPublisher dataCustodianPublisher = new DataCustodianPublisher();
					dataCustodianPublisher.WorkHistroySmartContract = workHistroySmartContract;
					dataCustodianPublisher.RSA = rsa;

					Console.WriteLine("Original Data: {0}", data);

					await dataCustodianPublisher.PublishWorkHistoryAsync(data, senderAddress, password);

					while (workHistroySmartContract.Address == string.Empty) ;

					bool match = await dataCustodianPublisher.CompareHashAsync(data);

					DataSubjectSharer dataSubjectSharer = new DataSubjectSharer();
					dataSubjectSharer.WorkHistroySmartContract = workHistroySmartContract;
					dataSubjectSharer.RSA = rsa;

					Console.WriteLine("Are they a match {0}", match);

					string decryptedData = await dataSubjectSharer.DecryptDataFromContract();

					Console.WriteLine("Decryped Data from blockchain: {0}", decryptedData);	
				}
				finally
				{
					rsa.PersistKeyInCsp = false;
				}
			}
			
		}

		static void Main()
		{
			MainAsync().GetAwaiter().GetResult();

			Console.ReadLine();
		}
	}
}
