    using System.Security.Cryptography;
    using System.Text;

    namespace PruebaTecnicaAPI.Helpers
    {
        public static class AesHelper
        {
            private static readonly string Key = "EsteEsElTextoClaveParaElCifradoAES256Prueba123456"; // Debe tener 32 bytes para AES-256
            private static readonly string Iv = "EsteEsElVectorIVparaAES"; // Debe tener 16 bytes

            public static string Encrypt(string plainText)
            {
                if (string.IsNullOrEmpty(plainText))
                {
                    return null;
                }

                byte[] keyBytes = Encoding.UTF8.GetBytes(Key.PadRight(32)); // Asegura 32 bytes
                byte[] ivBytes = Encoding.UTF8.GetBytes(Iv.PadRight(16)); // Asegura 16 bytes

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = ivBytes;

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plainText);
                            }
                            return Convert.ToBase64String(msEncrypt.ToArray());
                        }
                    }
                }
            }

            public static string Decrypt(string cipherText)
            {
                if (string.IsNullOrEmpty(cipherText))
                {
                    return null;
                }

                byte[] keyBytes = Encoding.UTF8.GetBytes(Key.PadRight(32)); // Asegura 32 bytes
                byte[] ivBytes = Encoding.UTF8.GetBytes(Iv.PadRight(16)); // Asegura 16 bytes
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = ivBytes;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
    }