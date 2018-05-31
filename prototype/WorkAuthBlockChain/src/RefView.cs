using System;
using System.Collections.Generic;
using System.Text;

namespace WorkAuthBlockChain
{
    public class RefView
    {
		public RefSharingContract RefSharingContract { get; set; }

		public async System.Threading.Tasks.Task<string> GetRefence(string contractAddress)
		{
			RefSharingContract.LoadContract(contractAddress);
			return await RefSharingContract.getData();
		}
	}
}
