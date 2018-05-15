﻿using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using WorkAuthBlockChain;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WorkAuthBlockChain;

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

		private static async void RefMenu(IList<string> args)
		{

			switch(args[0].ToLower())
			{
				case "create":


					break;
			}

			throw new NotImplementedException();
		}

		public static async Task MainAsync(string[] args)
		{
			switch(args[0].ToLower())
			{
				case "emp":
					var empArgs = args.ToList();
					empArgs.RemoveAt(0);
					await EmpMenu(empArgs.ToArray());
					break;

				case "ref":
					RefMenu(args);
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
		}
	}
}
