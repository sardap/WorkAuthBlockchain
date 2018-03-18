using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Nethereum.Web3;
using WorkAuthBlockChain;
using System.Collections.Generic;

namespace prototype.src
{
    class Program
    {
		const int DATA_LENGTH = 428;
		const int RSA_KEY_LENGTH = 4096;

		public static async Task MainAsync()
		{
			string xmlString;

			WorkHistroySmartContract workHistroySmartContract = new WorkHistroySmartContract();
			RSACryptoServiceProvider rsa;
			string address;
			//string data = new string('*', DATA_LENGTH);
			string data = "paulsarda.com, IT Idiot";

			Console.WriteLine("DATA CUSTODIAN");
			using (rsa = new RSACryptoServiceProvider(RSA_KEY_LENGTH))
			{
				try
				{

					string senderAddress = "0x7bc0157bc7e0ae9183e6d7688009963b18855248";
					string password = "passphrase";
					workHistroySmartContract.RSA = rsa;

					xmlString = RSACryptoServiceProviderExtensions.ToXmlString(rsa);

					DataCustodianPublisher dataCustodianPublisher = new DataCustodianPublisher();
					dataCustodianPublisher.WorkHistroySmartContract = workHistroySmartContract;
					dataCustodianPublisher.RSA = rsa;

					Console.WriteLine("Original Data: {0}", data);

					address = await dataCustodianPublisher.PublishWorkHistoryAsync(data, senderAddress, password);

					while (address == null) ;

					workHistroySmartContract.LoadContract(address);

					bool match = await dataCustodianPublisher.CompareHashAsync(data);

					Console.WriteLine("Are they a match {0}", match);
				}
				finally
				{
					rsa.PersistKeyInCsp = false;
				}

				Console.WriteLine("DATA SUBJECT");
				using (rsa = new RSACryptoServiceProvider(RSA_KEY_LENGTH))
				{
					try
					{
						RSACryptoServiceProviderExtensions.FromXmlString(rsa, xmlString);

						DataSubjectSharer dataSubjectSharer = new DataSubjectSharer();
						dataSubjectSharer.WorkHistroySmartContract = workHistroySmartContract;
						dataSubjectSharer.RSA = rsa;

						string decryptedData = await dataSubjectSharer.DecryptDataFromContract();

						Console.WriteLine("Decryped Data from blockchain: {0}", decryptedData);
					}
					finally
					{
						rsa.PersistKeyInCsp = false;
					}
				}

				Console.WriteLine("DATA CONSUMER");
				DataConsumer dataConsumer = new DataConsumer();
				dataConsumer.WorkHistroySmartContract = workHistroySmartContract;

				// @Bad Should probalby not be a list of fucking bools
				List<bool> verfiyResult = await dataConsumer.Verfiy(data, address);
				Console.WriteLine("The data hash is {0}", verfiyResult[0]);
				Console.WriteLine("The data issuer is {0}", verfiyResult[1]);
			}
		}

		static void Main()
		{
			MainAsync().GetAwaiter().GetResult();

			Console.ReadLine();
		}
	}
}
