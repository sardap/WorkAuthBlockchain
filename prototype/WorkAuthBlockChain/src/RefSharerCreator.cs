using System.Security.Cryptography;
using System;
using System.Threading.Tasks;

namespace WorkAuthBlockChain
{
	public class RefSharerCreator
	{
		public RefSharingContract RefSharingContract { get; set; }

		public async Task<string> DeployContractAsync(string senderAddress, string password, string refrereeText, string sharerAdress)
		{
			await RefSharingContract.UnlockAccount(senderAddress, password);

			return await RefSharingContract.Deploy(refrereeText, sharerAdress); ;
		}
	}
}