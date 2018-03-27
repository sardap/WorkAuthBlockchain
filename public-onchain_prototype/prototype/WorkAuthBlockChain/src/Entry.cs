using System;
using System.Collections.Generic;
using System.Text;

namespace WorkAuthBlockChain.src
{
    public class Entry
    {
		public string Address { get; set; }
		public string Domain { get; set; }
		public string Department { get; set; }
		public string Position { get; set; }
		public string Name { get; set; }
		public string ExtraData { get; set; }

		public string OnChainString()
		{
			return Domain + "," + Department + "," + Position + "," + Name + "," + ExtraData;
		}

		public override string ToString()
		{
			return Address + "," + OnChainString();
		}
	}
}
