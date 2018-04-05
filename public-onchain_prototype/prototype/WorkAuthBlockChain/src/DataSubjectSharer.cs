using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace WorkAuthBlockChain
{
	public class DataSubjectSharer
    {
		private const string FILE_PATH = "addresses.json";
		private List<string> _addresses;

		public DataSubjectSharer()
		{
			if(File.Exists(FILE_PATH))
			{
				using (StreamReader file = new StreamReader(FILE_PATH))
				{
					string jsonSource = file.ReadToEnd();
					_addresses = JsonConvert.DeserializeObject<List<string>>(jsonSource);
				}
			}
			else
			{
				_addresses = new List<string>();
			}
		}

		public List<string> Addresses
		{
			get
			{
				return _addresses;
			}
		}

		public RSACryptoServiceProvider RSA
		{
			get;
			set;
		}

		public WorkHistroySmartContract WorkHistroySmartContract
		{
			get;
			set;
		}

		public async Task<List<Entry>> GetAllWorkHistory()
		{
			List<Entry> result = new List<Entry>();

			foreach (string address in Addresses)
			{
				if(WorkHistroySmartContract.LoadContract(address))
				{
					string onChainData = await DecryptDataFromContract();
					result.Add(new Entry(address + "," + onChainData));
				}
			}

			return result;
		}

		public void AddAddress(string address)
		{
			_addresses.Add(address);
		}

		public void SaveChangesToAddresses()
		{
			Utils.ExportToJsonFile(FILE_PATH, _addresses);
		}

		private string DecryptData(string encryptedData)
		{
			var resultBytes = Convert.FromBase64String(encryptedData);
			var decryptedBytes = RSA.Decrypt(resultBytes, true);
			var decryptedData = Encoding.UTF8.GetString(decryptedBytes);

			return decryptedData;
		}

		public async Task<string> DecryptDataFromContract()
		{
			return DecryptData(await WorkHistroySmartContract.GetData());
		}

		
	}
}
