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
			MD5 md5 = MD5.Create();

			byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(data));

			return bytes;
		}
	}
}