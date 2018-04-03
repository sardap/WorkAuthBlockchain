using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WorkAuthBlockChain
{
    public static class Utils
    {
		public static byte[] GetHash(string data)
		{
			SHA256 md5 = SHA256.Create();

			byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(data));

			return bytes;
		}

		public static void ExportToJsonFile<T>(string filePath, T toWrite)
		{
			using (StreamWriter file = File.CreateText(filePath))
			{
				JsonSerializer serializer = new JsonSerializer();
				//serialize object directly into file stream
				serializer.Serialize(file, toWrite);
			}
		}

		public static async Task<T> ReadFromJson<T>(string filePath)
		{
			using (StreamReader r = new StreamReader(filePath))
			{
				string json = await r.ReadToEndAsync();
				return await Task.Run(() => JsonConvert.DeserializeObject<T>(json));
			}
		}

	}
}