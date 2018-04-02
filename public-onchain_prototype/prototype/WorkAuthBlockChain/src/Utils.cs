using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace WorkAuthBlockChain
{
    static class Utils
    {
		public static byte[] GetHash(string data)
		{
			SHA256 md5 = SHA256.Create();

			byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(data));

			return bytes;
		}
	}
}