using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace WorkAuthBlockChain
{
	public class DataProdcuer
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

		private string EncyptData(string data)
		{
			byte[] dataInBytes = Encoding.UTF8.GetBytes(data);
			byte[] encryptedData = RSA.Encrypt(dataInBytes, true);

			return Convert.ToBase64String(encryptedData);
		}

		public async Task<string> PublishWorkHistoryAsync(string data, string senderAddress, string senderPassword)
		{
			// This seems fucking dumb
			string error = WorkHistroySmartContract.DataValid(data);

			if (error == "")
			{
				bool unlockAcountResult = await WorkHistroySmartContract.UnlockAccount(senderAddress, senderPassword);

				string encryptedData = EncyptData(data);

				string trasnactionHash = await WorkHistroySmartContract.Deploy(encryptedData, data.GetHashCode());
					
				return trasnactionHash;
			}
			else
			{
				throw new WorkHistroySmartContractInValidDataException(error);
			}
		}

		public async Task<bool> CompareHashAsync(string data)
		{
			return data.GetHashCode() == await WorkHistroySmartContract.GetHash();
		}

	}
}
