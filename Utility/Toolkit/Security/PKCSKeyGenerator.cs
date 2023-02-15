using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Utility.Toolkit.Security
{
    public class PKCSKeyGenerator
    {
        byte[] key = new byte[8], iv = new byte[8];
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();

        public byte[] Key { get { return key; } }
        public byte[] IV { get { return IV; } }
        public ICryptoTransform Encryptor { get { return des.CreateEncryptor(key, iv); } }
        public ICryptoTransform Decryptor { get { return des.CreateDecryptor(key, iv); } }

        public PKCSKeyGenerator() { }
        public PKCSKeyGenerator(String keystring, byte[] salt, int md5iterations, int segments)
        {
            Generate(keystring, salt, md5iterations, segments);
        }

        public ICryptoTransform Generate(String keystring, byte[] salt, int md5iterations, int segments)
        {
            int HASHLENGTH = 16;    //MD5 bytes
            byte[] keymaterial = new byte[HASHLENGTH * segments];     //to store contatenated Mi hashed results

            // --- get secret password bytes ----
            byte[] psbytes;
            psbytes = Encoding.UTF8.GetBytes(keystring);

            // --- contatenate salt and pswd bytes into fixed data array ---
            byte[] data00 = new byte[psbytes.Length + salt.Length];
            Array.Copy(psbytes, data00, psbytes.Length);        //copy the pswd bytes
            Array.Copy(salt, 0, data00, psbytes.Length, salt.Length);    //concatenate the salt bytes

            // ---- do multi-hashing and contatenate results  D1, D2 ...  into keymaterial bytes ----
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = null;
            byte[] hashtarget = new byte[HASHLENGTH + data00.Length];   //fixed length initial hashtarget

            for (int j = 0; j < segments; j++)
            {
                // ----  Now hash consecutively for md5iterations times ------
                if (j == 0) result = data00;       //initialize
                else
                {
                    Array.Copy(result, hashtarget, result.Length);
                    Array.Copy(data00, 0, hashtarget, result.Length, data00.Length);
                    result = hashtarget;
                }

                for (int i = 0; i < md5iterations; i++)
                    result = md5.ComputeHash(result);

                Array.Copy(result, 0, keymaterial, j * HASHLENGTH, result.Length);  //contatenate to keymaterial
            }

            Array.Copy(keymaterial, 0, key, 0, 8);
            Array.Copy(keymaterial, 8, iv, 0, 8);

            return Encryptor;
        }
    }
}
