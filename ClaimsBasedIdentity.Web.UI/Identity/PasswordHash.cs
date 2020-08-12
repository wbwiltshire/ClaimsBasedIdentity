using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ClaimsBasedIdentity.Web.UI.Identity
{
	public class PasswordHash
	{
		public static bool ValidateHashedPassword(string hashedPassword, string providedPassword)
		{
			int iterCount = 0;
			int saltLength = 0;
			KeyDerivationPrf prf;
			int subkeyLength = 0;

			byte[] decodedHashedPassword = Convert.FromBase64String(hashedPassword);
			prf = (KeyDerivationPrf)ReadNetworkByteOrder(decodedHashedPassword, 1);
			iterCount = (int)ReadNetworkByteOrder(decodedHashedPassword, 5);
			saltLength = (int)ReadNetworkByteOrder(decodedHashedPassword, 9);
			byte[] salt = new byte[saltLength];
			Buffer.BlockCopy(decodedHashedPassword, 13, salt, 0, salt.Length);

			// Read the subkey (the rest of the payload): must be >= 128 bits
			subkeyLength = decodedHashedPassword.Length - 13 - salt.Length;
			if (subkeyLength < saltLength)
			{
				return false;
			}
			byte[] expectedSubkey = new byte[subkeyLength];
			Buffer.BlockCopy(decodedHashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

			// Hash the incoming password and verify it
			byte[] actualSubkey = KeyDerivation.Pbkdf2(providedPassword, salt, prf, iterCount, subkeyLength);

			return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
		}

		private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
		{
			return ((uint)(buffer[offset + 0]) << 24)
							| ((uint)(buffer[offset + 1]) << 16)
							| ((uint)(buffer[offset + 2]) << 8)
							| ((uint)(buffer[offset + 3]));
		}

	}
}
