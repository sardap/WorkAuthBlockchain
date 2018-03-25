using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace WorkAuthBlockChain
{
	public class DataCustodianPublisher
    {
		private double _progress;

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

		public double Progress
		{
			get
			{
				return _progress;
			}
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
			string error = WorkHistroySmartContract.DataVaild(data);

			_progress += 10;

			if (error == "")
			{
				bool unlockAcountResult = await WorkHistroySmartContract.UnlockAccount(senderAddress, senderPassword);
				_progress += 10;

				string encryptedData = EncyptData(data);
				_progress += 10;

				string trasnactionHash = await WorkHistroySmartContract.Deploy(encryptedData, data.GetHashCode());
				_progress += 10;
					
				return trasnactionHash;
			}
			else
			{
				throw new WorkHistroySmartContractInvaildDataException(error);
			}
		}

		public async Task<bool> CompareHashAsync(string data)
		{
			return data.GetHashCode() == await WorkHistroySmartContract.GetHash();
		}

	}
}
