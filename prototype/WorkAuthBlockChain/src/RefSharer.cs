using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WorkAuthBlockChain
{
    public class RefSharer
    {
		public RefSharingContract RefSharingContract { get; set; }

		public async Task<string> Share(string senderAddress, string password, string targetAddress, string contractAddress)
		{
			RefSharingContract.LoadContract(contractAddress);

			await RefSharingContract.UnlockAccount(senderAddress, password);

			return await RefSharingContract.ExecuteShare(targetAddress);
		}
	}
}
