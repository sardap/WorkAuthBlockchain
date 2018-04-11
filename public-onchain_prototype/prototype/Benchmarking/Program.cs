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

		private const int NUMBER_OF_TESTS = 100;

		public class BenchmarkEntry
		{
			public string TimeStamp { get; set; }

			public double TimeElpased { get; set; }

			public override string ToString()
			{
				return TimeStamp + "," + TimeElpased;
			}
		}

		public static async Task BenchmarkWorkHistoryBundleCreation(int n)
		{
			Queue<string> inputQue = new Queue<string>();
			inputQue.Enqueue("1");
			inputQue.Enqueue("2");
			inputQue.Enqueue("0");
			inputQue.Enqueue("done");
			inputQue.Enqueue("ref.json");
			inputQue.Enqueue("done");
			inputQue.Enqueue("workBundle" + n + ".json");

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

					while ((input = inputQue.Dequeue()) != "done")
					{
						dataBundle.Referees.Add(await Entry.ReadEntry(input));
					}

					input = inputQue.Dequeue();

					//Utils.ExportToJsonFile(input, dataBundle);
				}
				finally
				{
					rsa.PersistKeyInCsp = false;
				}
			}
		}

		public static void Main(string[] args)
		{
			Stopwatch stopwatch = new Stopwatch();

			List<BenchmarkEntry> results = new List<BenchmarkEntry>();
			for (int i = 0; i < NUMBER_OF_TESTS; i++)
			{
				//string deletePath = "workBundle" + i + ".json";
				//File.Delete(deletePath);

				stopwatch.Start();
				BenchmarkWorkHistoryBundleCreation(i).GetAwaiter().GetResult();
				stopwatch.Stop();

				results.Add(
					new BenchmarkEntry
					{
						TimeElpased = ((double)stopwatch.ElapsedTicks / Stopwatch.Frequency) * 1000000000,
						TimeStamp = DateTime.Now.TimeOfDay.ToString()
					}
				);

				//File.Delete(deletePath);
				Console.WriteLine(i);
			}

			using (StreamWriter resultFile = File.CreateText("resultNoIO.csv"))
			{
				results.ForEach(i => resultFile.WriteLine(i));
			}

		}
	}
}
