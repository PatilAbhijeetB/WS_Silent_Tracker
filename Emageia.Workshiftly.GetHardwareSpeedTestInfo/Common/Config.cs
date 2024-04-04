using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Common
{
    internal class Config
    {
        public static SqlConnection Conn;
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static SqlConnection Connection()
        {
            string strcnn = "GEsoStY3654jC/QoDCutZpwlJvQfYfaayqCTM5EeXwjfmCz9YGDm3MjPb0CXtByWv6FpvYd7N+OcRzvGYXHJpetDASoB6H0S+R23urMnkJGGDHzfIIoFHnBVNPsqstg+wo3ojVjthXcQVAtgZRHyeLycBJq4iU9pKujhQxbJ1wTL4e+46FnWlfjnErMQavWhfZ25lxxXzqNchjMZZhY578NeOBa/AG5NWwZN7TYp8N1xhA1+nO2jSYCsFgJPbaqJ";
            string ConnectionString = StrDecrypt(strcnn);
            Conn = new SqlConnection(ConnectionString);
            return Conn;
        }

        public static string StrDecrypt(string Text)
        {
            string dcrptData = "";
            string encString = Text;
            string strpass = "";
            Employee emp = new Employee();
            string password = emp.Encryptsp.ToString();
            string salt = emp.Salt.ToString();
            string dataString = emp.Encryptstr;
            byte[] dataBytes = Encoding.Unicode.GetBytes(dataString);

            // Encrypt/Decrypt strings
            //var encStr = CryptoHelper.EncryptString(password, salt, dataString, SymmetricCryptoAlgorithm.TripleDES_192_CBC);
            var decStr = CryptoHelper.DecryptString(password, salt, dataString, SymmetricCryptoAlgorithm.TripleDES_192_CBC);
            strpass = decStr.ToString();
            if (strpass != "")
            {
                dcrptData = Decryptstring(encString, strpass);
            }
            return dcrptData;
        }

        public static string Decryptstring(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static string DecryptFile(string inputFile, string pword)
        {
            string ext = Path.GetExtension(inputFile);
            string outputFile = inputFile.Replace(ext, "_enc" + ext);
            string password = pword;

            //Encrypt the password and make sure its a 128Bit / 16 byte key
            System.Text.UTF8Encoding UTF8 = new UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] key = HashProvider.ComputeHash(UTF8.GetBytes(password)); ;

            //Prepare the file for decryption by getting it into a stream
            FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

            //Setup the Decryption Standard using Read mode
            RijndaelManaged RMCrypto = new RijndaelManaged();
            CryptoStream cs = new CryptoStream(fsCrypt, RMCrypto.CreateDecryptor(key, key), CryptoStreamMode.Read);

            //Write the decrypted file stream
            FileStream fsOut = new FileStream(outputFile, FileMode.Create);
            int data;
            while ((data = cs.ReadByte()) != -1)
            { fsOut.WriteByte((byte)data); }

            //Close all the Writers
            fsOut.Close();
            cs.Close();
            fsCrypt.Close();

            //Delete the original file
            File.Delete(inputFile);
            //Rename the encrypted file to that of the original
            File.Copy(outputFile, inputFile);
            File.Delete(outputFile);
            //File.Create(outputFile);
            string outputpath = Path.GetFullPath(inputFile);
            return outputpath;
        }

        private static void SetAccessRights(string file)
        {

            DirectoryInfo dInfo = new DirectoryInfo(file);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);

        }
    }
}
