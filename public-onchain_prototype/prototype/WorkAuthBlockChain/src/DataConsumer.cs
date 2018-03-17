using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WorkAuthBlockChain
{
	public class DataConsumer
    {
		public WorkHistroySmartContract WorkHistroySmartContract
		{
			get;
			set;
		}

		public async Task<List<bool>> Verfiy(string data, string address)
		{
			string error = WorkHistroySmartContract.DataVaild(data);

			if(error == "")
			{
				List<bool> testsPassed = new List<bool>();

				WorkHistroySmartContract.LoadContract(address);
				testsPassed.Add(await CheckHashes(data));
				testsPassed.Add(await CompareSender(data));

				return testsPassed;
			}
			else
			{
				throw new WorkHistroySmartContractInvaildDataException(error);
			}
		}

		private async Task<bool> CheckHashes(string data)
		{
			return data.GetHashCode() == await WorkHistroySmartContract.GetHash();
		}

		private async Task<bool> CompareSender(string data)
		{
			using (WebClient wc = new WebClient())
			{
				//string jsonSource = wc.DownloadString(data.Split(",")[0]);
				string jsonSource = wc.DownloadString("http://localhost/PKNR.json");

				dynamic jsonParsed = JObject.Parse(jsonSource);

				string creatorAddress = await WorkHistroySmartContract.GetCreator();

				foreach(string s in jsonParsed.public_keys)
				{
					if(s == creatorAddress)
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
