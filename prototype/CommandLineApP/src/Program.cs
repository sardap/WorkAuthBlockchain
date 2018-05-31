using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using WorkAuthBlockChain;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WorkAuthBlockChain;

// Create
// ref create	0x25922333d41f0f3f40be629f81af6983634d0fb6 passphrase pauliscool 0xf8d7ed06ee59ee030f5b5a5b0ad9777c00e89c3d

// Share		Sender Address								Geth Password	Target Address								Contract Address
// ref share	0xf8d7ed06ee59ee030f5b5a5b0ad9777c00e89c3d	passphrase		0x7217461990542841aa38d247419be2af405c4282	0x18bbc7ef51db1b20273f24ff0428b5feff011d3f

//args 1 
// respond		Sender Address								Geth password	Contract Address
//respond			0x25922333d41f0f3f40be629f81af6983634d0fb6	passphrase		0x18bbc7ef51db1b20273f24ff0428b5feff011d3f

namespace prototype.src
{
    class Program
    {
		private static async Task EmpMenu(string[] args)
		{
			WorkHistroySmartContract workHistroySmartContract = new WorkHistroySmartContract();
			RSACryptoServiceProvider rsa;
			DataSubjectSharer dataSubjectSharer = new DataSubjectSharer
			{
				WorkHistroySmartContract = workHistroySmartContract,
			};

			using (rsa = new RSACryptoServiceProvider(Consts.RSA_KEY_LENGTH))
			{
				try
				{
					dataSubjectSharer.RSA = rsa;

					string xmlString = new StreamReader(args[1]).ReadToEnd();
					RSACryptoServiceProviderExtensions.FromXmlString(rsa, xmlString);

					switch (args[0].ToLower())
					{
						case "add":
							dataSubjectSharer.AddAddress(args[2]);
							dataSubjectSharer.SaveChangesToAddresses();
							break;
						case "craete":
							await CreateWorkHistoryBundle(dataSubjectSharer);
							break;
						case "export":
							await ExportEntry(dataSubjectSharer);
							break;
					}
				}
				finally
				{
					rsa.PersistKeyInCsp = false;
				}
			}
		}

		private static async Task RefMenu(IList<string> args)
		{
			RefSharingContract RefSharingContract = new RefSharingContract();

			switch (args[0].ToLower())
			{
				//args 1 senderAddress, 2: geth password, 3: refreeText, 4: sharer address
				case "create":
					RefSharerCreator creator = new RefSharerCreator
					{
						RefSharingContract = RefSharingContract
					};

					Console.WriteLine("Contract address {0}", await creator.DeployContractAsync(args[1], args[2], args[3], args[4]));

					break;

				//args 1 senderAddress, 2: geth password, 3: targetAddress
				case "share":
					RefSharer sharer = new RefSharer
					{
						RefSharingContract = RefSharingContract
					};

					Console.WriteLine("Comeplted transaction hash {0}", await sharer.Share(args[1], args[2], args[3], args[4]));

					break;

				//args 1 senderAddress, 2: geth password, 3: contractAddress
				case "respond":
					RefRespond respond = new RefRespond
					{
						RefSharingContract = RefSharingContract
					};

					await respond.UnlockAccountLoadContractAsync(args[1], args[2], args[3]);

					var addresses = await respond.GetRequests();

					for(int i = 0; i < addresses.Count; i++)
					{
						Console.WriteLine("{0}: Address: {1}", i, addresses[i]);
					}

					Console.WriteLine("Enter address to respond to");
					string input = Console.ReadLine();
					int number;
					Int32.TryParse(input, out number);

					Console.WriteLine("Do you approve?");
					input = Console.ReadLine();

					Console.WriteLine("Transaction sent {0}", await respond.RespondRequest(addresses[number], input.ToLower() == "yes" ? true : false));

					break;
			}

			return;
		}

		public static async Task MainAsync(string[] args)
		{
			string mode = args[0].ToLower();
			var argsList = args.ToList();
			argsList.RemoveAt(0);

			switch (mode)
			{
				case "emp":
					await EmpMenu(argsList.ToArray());
					break;

				case "ref":
					await RefMenu(argsList);
					break;
			}
			
		}

		private static async Task CreateWorkHistoryBundle(DataSubjectSharer dataSubjectSharer)
		{
			DataBundle dataBundle = new DataBundle();
			List<Entry> workHistory = await dataSubjectSharer.GetAllWorkHistory();

			workHistory.ForEach(i =>
				Console.WriteLine("ELEMENT:" + (workHistory.IndexOf(i) + 1) + ":\n" + i.ToPrettyString())
			);


			Console.WriteLine("Please enter index for each entry you would like to add After type done");

			string input;

			do
			{
				input = Console.ReadLine();
				try
				{
					int i = Int32.Parse(input) - 1;
					dataBundle.WorkHistory.Add(workHistory[i]);
				}
				catch (Exception e)
				{
				}

			} while (input != "done");

			Console.WriteLine("Please enter the file name for each referee to add. After enter done");

			while ((input = Console.ReadLine()) != "done")
			{
				dataBundle.Referees.Add(await Entry.ReadEntry(input));
			}

			Console.WriteLine("Enter file name");
			input = Console.ReadLine();

			Utils.ExportToJsonFile(input, dataBundle);
		}

		private static async Task ExportEntry(DataSubjectSharer dataSubjectSharer)
		{
			List<Entry> workHistory = await dataSubjectSharer.GetAllWorkHistory();

			workHistory.ForEach(entry =>
				Console.WriteLine("ELEMENT:" + (workHistory.IndexOf(entry) + 1) + ":\n" + entry.ToPrettyString())
			);

			Console.WriteLine("Select Which entry you would like to export");

			int i = Int32.Parse(Console.ReadLine()) - 1;

			Console.WriteLine("Enter file path");

			Utils.ExportToJsonFile(Console.ReadLine(), workHistory[i]);
		}

		static void Main(string[] args)
		{
			MainAsync(args).GetAwaiter().GetResult();
			Console.ReadLine();
		}
	}
}
