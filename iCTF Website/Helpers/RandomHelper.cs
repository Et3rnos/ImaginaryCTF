using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace iCTF_Website.Helpers
{
    public class RandomHelper
    {
        public static string GenerateRandomString(int length = 32, string charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            using var rng = new RNGCryptoServiceProvider();
            var bytes = new byte[4 * length];
            rng.GetBytes(bytes);

            var str = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                uint index = BitConverter.ToUInt32(bytes, i * 4);
                char chr = charset[(int)(index % charset.Length)];
                str.Append(chr);
            }
            return str.ToString();
        }
    }
}
