using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public async Task<VerfiyResult> Verfiy(string data, string address)
		{
			string error = WorkHistroySmartContract.DataValid(data);

			if(error == "")
			{
				WorkHistroySmartContract.LoadContract(address);

				VerfiyResult result = new VerfiyResult
				{
					Sender = await CompareSender(data),
					Data = await CheckHashes(data)
				};

				return result;
			}
			else
			{
				throw new WorkHistroySmartContractInValidDataException(error);
			}
		}

		private async Task<bool> CheckHashes(string data)
		{
			return Utils.GetHash(data).SequenceEqual(await WorkHistroySmartContract.GetHash());
		}

		private async Task<bool> CompareSender(string data)
		{
			using (WebClient wc = new WebClient())
			{
				string URL = "http://" + data.Split(',')[0] + "/PKNR.json";

				string jsonSource = wc.DownloadString(URL);

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
