using System;
using System.IO;
using System.Security.Cryptography;

namespace ChuXinEdu.CMS.Server.Utils
{
    public class Cryptor
	{
		private static byte[] _key = new byte[] { 0x71, 0x8c, 0x99, 0x19, 0x67, 0xa9, 0x4e, 0xb2, 0x83, 0xa9, 0x2e, 0xf1, 0x20, 0x15, 0xa7, 0xb0 };
		//private static byte[] _iv = new byte[] { 0x89, 0x72, 0x14, 0x36, 0xf5, 0xbe, 0x40, 0x90, 0x8e, 0x9f, 0x48, 0xbd, 0xb8, 0xff, 0xf3, 0xd3 };
		public static string Decrypt(string inputStr)
		{
			AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
			
			byte[] bytes1 = Convert.FromBase64String(inputStr);
			MemoryStream input = new MemoryStream(bytes1);
			input.Position = 0;
			byte[] iv = new byte[16];
			input.Read(iv, 0, 16);
			MemoryStream output = new MemoryStream();
			CryptoStream cryStream = new CryptoStream(output, aes.CreateDecryptor(_key, iv), CryptoStreamMode.Write);
			int size = 4 * 1024;
			byte[] bytes = new byte[size];
			int readLen = input.Read(bytes, 0, size);
			while (readLen > 0)
			{
				cryStream.Write(bytes, 0, readLen);
				bytes = new byte[size];
				readLen = input.Read(bytes, 0, size);
			}
			cryStream.FlushFinalBlock();

			byte[] bytes2 = output.ToArray();
			string ret = System.Text.Encoding.UTF8.GetString(bytes2);
			return ret;
		}

		public static string Encrypt(string inputStr)
		{
			AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
			byte[] bytes1 = System.Text.Encoding.UTF8.GetBytes(inputStr);
			MemoryStream input = new MemoryStream(bytes1);
			input.Position = 0;
			byte[] iv = Guid.NewGuid().ToByteArray();
			MemoryStream output = new MemoryStream();
			CryptoStream cryStream = new CryptoStream(output, aes.CreateEncryptor(_key, iv), CryptoStreamMode.Write);
			cryStream.Write(iv, 0, iv.Length);

			int size = 4 * 1024;
			byte[] bytes = new byte[size];
			int readLen = input.Read(bytes, 0, size);
			while (readLen > 0)
			{
				cryStream.Write(bytes, 0, readLen);
				bytes = new byte[size];
				readLen = input.Read(bytes, 0, size);
			}
			cryStream.FlushFinalBlock();

			byte[] bytes2 = output.ToArray();
			string ret = Convert.ToBase64String(bytes2);
			return ret;
		}
	}
}