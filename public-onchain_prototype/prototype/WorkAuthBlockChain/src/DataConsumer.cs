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

		public async Task<VerfiyResult> Verfiy(string data, string address)
		{
			string error = WorkHistroySmartContract.DataValid(data);

			if(error == "")
			{
				WorkHistroySmartContract.LoadContract(address);
				VerfiyResult result = new VerfiyResult
				{
					Sender = await CheckHashes(data),
					Data = await CompareSender(data)
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
			return data.GetHashCode() == await WorkHistroySmartContract.GetHash();
		}

		private async Task<bool> CompareSender(string data)
		{
			using (WebClient wc = new WebClient())
			{
				string jsonSource = wc.DownloadString("http://" + data.Split(',')[0] + "/PKNR.json");

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
