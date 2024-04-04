/*****************************************************************

 * Author:   Sameera Niroshan 
 * Email:    sameera@emagia.com
 * Create Date:  18/02/2022 
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/
using System.Security.Cryptography;
using System.Text; 
using System.IO;


namespace Emageia.Workshiftly.AutoUpdater.AutoUpdateHelper
{
    internal enum HashType
    {
        MD5, SHA1, SHA512
    }
    internal static class Hasher
    {
        public static string HashFile(string filePath, HashType algo)
        {
            switch (algo)
            {
                case HashType.MD5:
                    return MakeHashString(MD5.Create().ComputeHash(new FileStream(filePath, FileMode.Open)));
                case HashType.SHA1:
                    return MakeHashString(SHA1.Create().ComputeHash(new FileStream(filePath, FileMode.Open)));
                case HashType.SHA512:
                    return MakeHashString(SHA512.Create().ComputeHash(new FileStream(filePath, FileMode.Open)));
                default:
                    return "";
            }
        }

        private static string MakeHashString(byte[] hash)
        {
            StringBuilder sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
                sb.Append(b.ToString("X2").ToLower());
            return sb.ToString();
        }
    }
}
