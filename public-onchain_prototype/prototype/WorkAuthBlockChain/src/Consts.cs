using System;
using System.Collections.Generic;
using System.Text;

namespace WorkAuthBlockChain
{
    public static class Consts
    {
		public const int RSA_KEY_LENGTH = 1024;

		public static readonly int DATA_MAX_LENGTH = ((RSA_KEY_LENGTH - 384) / 8) + 7;
	}
}
