using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace WorkAuthBlockChain
{
	public class DataSubjectSharer
    {
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
