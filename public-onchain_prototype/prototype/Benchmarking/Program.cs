using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WorkAuthBlockChain;

namespace Benchmarking
{
    class Program
    {

		private const int NUMBER_OF_TESTS = 100000;

		public static async Task<double> BenchmarkWorkHistoryBundleCreation()
		{

			Stopwatch stopwatch = new Stopwatch();

			Queue<string> inputQue = new Queue<string>();
			inputQue.Enqueue("1");
			inputQue.Enqueue("2");
			inputQue.Enqueue("0");
			inputQue.Enqueue("done");
			inputQue.Enqueue("ref.json");
			inputQue.Enqueue("done");
			inputQue.Enqueue("workBundle.json");

			// START OF BENCHMARK
			stopwatch.Start();

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

					string xmlString = new StreamReader("privatePaul.xml").ReadToEnd();
					RSACryptoServiceProviderExtensions.FromXmlString(rsa, xmlString);

					DataBundle dataBundle = new DataBundle();
					List<Entry> workHistory = await dataSubjectSharer.GetAllWorkHistory();

					/*
					workHistory.ForEach(i =>
						Console.WriteLine("ELEMENT:" + (workHistory.IndexOf(i) + 1) + ":\n" + i.ToPrettyString())
					);

					Console.WriteLine("Please enter index for each entry you would like to add After type done");
					*/
					string input;

					do
					{
						input = inputQue.Dequeue();
						try
						{
							int i = Int32.Parse(input) - 1;
							dataBundle.WorkHistory.Add(workHistory[i]);
						}
						catch (Exception e)
						{
						}

					} while (input != "done");

					//Console.WriteLine("Please enter the file name for each referee to add. After enter done");

					while ((input = inputQue.Dequeue()) != "done")
					{
						dataBundle.Referees.Add(await Entry.ReadEntry(input));
					}

					//Console.WriteLine("Enter file name");
					input = inputQue.Dequeue();

					Utils.ExportToJsonFile(input, dataBundle);

				}
				finally
				{
					rsa.PersistKeyInCsp = false;
				}
			}

			return stopwatch.Elapsed.TotalMilliseconds;
		}

		public static async Task MainAsync(string[] args)
		{
			List<double> results = new List<double>();

			for(int i = 0; i < NUMBER_OF_TESTS; i++)
			{
				results.Add(await BenchmarkWorkHistoryBundleCreation());
				Console.WriteLine(i + results[results.Count - 1]);
			}

			using (StreamWriter resultFile = File.CreateText("result.csv"))
			{
				results.ForEach(i => resultFile.WriteLine(i));
			}
		}

		public static void Main(string[] args)
		{
			MainAsync(args).GetAwaiter().GetResult();

		}
	}
}
