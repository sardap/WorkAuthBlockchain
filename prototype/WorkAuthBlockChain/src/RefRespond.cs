using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WorkAuthBlockChain
{
    public class RefRespond
    {
		public RefSharingContract RefSharingContract { get; set; }


		public async Task UnlockAccountLoadContractAsync(string senderAddress, string gethPassword, string contractAddress)
		{
		 	await RefSharingContract.UnlockAccount(senderAddress, gethPassword);
			RefSharingContract.LoadContract(contractAddress);
		}

		public async Task<IList<string>> GetRequests()
		{
			const int ETHER_ADDRESS_LENGTH = 40;

			List<string> addresses = new List<string>();

			string concatedAddresses = await RefSharingContract.getShareRecords();

			for(int i = 0; i < concatedAddresses.Length; i += ETHER_ADDRESS_LENGTH)
			{
				addresses.Add(concatedAddresses.Substring(i, ETHER_ADDRESS_LENGTH));
			}

			return addresses;
		}

		public async Task<string> RespondRequest(string approvedAddress, bool response)
		{
			return await RefSharingContract.RespondRequest(approvedAddress, response);
		}
	}
}
