using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WorkAuthBlockChain
{
    public class Entry
    {
		public static async Task<Entry> ReadEntry(string workHistoryBundlePath)
		{
			using (StreamReader r = new StreamReader(workHistoryBundlePath))
			{
				string json = await r.ReadToEndAsync();
				return await Task.Run(() => JsonConvert.DeserializeObject<Entry>(json));
			}
		}

		public string Address { get; set; }
		public string Domain { get; set; }
		public string Date { get; set; }
		public string Department { get; set; }
		public string Position { get; set; }
		public string Name { get; set; }
		public string ExtraData { get; set; }

		public Entry()
		{
		}

		public Entry(string data)
		{
			string[] dataSplit = data.Split(',');

			Address = dataSplit[0];
			Domain = dataSplit[1];
			Date = dataSplit[2];
			Department = dataSplit[3];
			Position = dataSplit[4];
			Name = dataSplit[5];
			ExtraData = dataSplit[6];
		}

		public string OnChainString()
		{
			return Domain + "," + Date + "," + Department + "," + Position + "," + Name + "," + ExtraData;
		}

		public string ToPrettyString()
		{
			return "Address:\t" + Address + "\n" +
				"Domain:\t\t" + Domain + "\n" +
				"Date:\t\t" + Date + "\n" +
				"Department:\t" + Department + "\n" +
				"Postion:\t" + Position + "\n" +
				"Name:\t\t" + Name + "\n" +
				"Extra Data:\t" + ExtraData + "\n";
		}

		public override string ToString()
		{
			return Address + "," + OnChainString();
		}
	}
}
