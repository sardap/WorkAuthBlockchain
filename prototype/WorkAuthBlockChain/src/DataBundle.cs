using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WorkAuthBlockChain
{
    public class DataBundle
    {
		public List<Entry> Referees { get; set; }

		public List<Entry> WorkHistory{ get; set; }

		public DataBundle()
		{
			Referees = new List<Entry>();
			WorkHistory = new List<Entry>();
		}
	}
}
