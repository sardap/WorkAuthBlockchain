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

		public async Task<VerfiyResult> Verfiy(Entry entry)
		{
			string error = WorkHistroySmartContract.DataValid(entry.OnChainString());

			if(error == "")
			{
				WorkHistroySmartContract.LoadContract(entry.Address);

				VerfiyResult result = new VerfiyResult
				{
					Sender = await CompareSender(entry.Domain),
					Data = await CheckHashes(entry.OnChainString())
				};

				return result;
			}
			else
			{
				throw new WorkHistroySmartContractInValidDataException(error);
			}
		}

		private async Task<bool> CheckHashes(string onChainString)
		{
			return Utils.GetHash(onChainString).SequenceEqual(await WorkHistroySmartContract.GetHash());
		}

		private async Task<bool> CompareSender(string domain)
		{
			try
			{
				using (WebClient wc = new WebClient())
				{
					string URL = "http://" + domain + "/PKNR.json";

					string jsonSource = wc.DownloadString(URL);

					dynamic jsonParsed = JObject.Parse(jsonSource);

					string creatorAddress = await WorkHistroySmartContract.GetCreator();

					foreach (string s in jsonParsed.public_keys)
					{
						if (s == creatorAddress)
						{
							return true;
						}
					}
				}
			}
			catch(Exception e)
			{
				return false;
			}

			return false;
		}
	}
}
