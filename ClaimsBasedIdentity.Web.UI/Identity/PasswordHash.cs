using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ClaimsBasedIdentity.Web.UI.Identity
{
	public class PasswordHash
	{
		public static string HashPassword(string password)
		{
			int iterCount = 10000;
			int saltLength = 128 / 8;

			// generate a 128-bit salt using a secure PRNG
			byte[] salt = new byte[saltLength];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(salt);
			}

			// derive a 256-bit subkey (use HMACSHA256 with 10,000 iterations)
			byte[] subkey = KeyDerivation.Pbkdf2(
				password: password,
				salt: salt,
				prf: KeyDerivationPrf.HMACSHA256,
				iterationCount: iterCount,
				numBytesRequested: 256 / 8);

			return CreateHash(salt, subkey, iterCount, saltLength);
		}

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

		private static string CreateHash(byte[] s, byte[] sk, int ic, int sl)
		{
			byte formatMarker = 0x01;
			KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA256;
			byte[] hashBytes = new byte[13 + s.Length + sk.Length];

			hashBytes[0] = (byte)formatMarker;
			WriteNetworkByteOrder(hashBytes, 1, (uint)prf);
			WriteNetworkByteOrder(hashBytes, 5, (uint)ic);
			WriteNetworkByteOrder(hashBytes, 9, (uint)sl);
			Buffer.BlockCopy(s, 0, hashBytes, 13, s.Length);
			Buffer.BlockCopy(sk, 0, hashBytes, 13 + s.Length, sk.Length);

			return Convert.ToBase64String(hashBytes);
		}

		private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
		{
			return ((uint)(buffer[offset + 0]) << 24)
							| ((uint)(buffer[offset + 1]) << 16)
							| ((uint)(buffer[offset + 2]) << 8)
							| ((uint)(buffer[offset + 3]));
		}

		private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
		{
			buffer[offset + 0] = (byte)(value >> 24);
			buffer[offset + 1] = (byte)(value >> 16);
			buffer[offset + 2] = (byte)(value >> 8);
			buffer[offset + 3] = (byte)(value >> 0);
		}

	}
}
