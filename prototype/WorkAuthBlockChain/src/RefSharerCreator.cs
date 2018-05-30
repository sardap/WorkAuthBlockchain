using System.Security.Cryptography;
using System;
using System.Threading.Tasks;

namespace WorkAuthBlockChain
{
	public class RefSharerCreator
	{
		public RefSharingContract RefSharingContract { get; set; }

		public async Task<byte[]> DeployContractAsync(string address, string password, string refrereeText, string sharerAdress)
		{
			int x = 0;

			await RefSharingContract.UnlockAccount(address, password);

			await RefSharingContract.Deploy(refrereeText, sharerAdress);

			return new byte[] { };
		}
	}
}