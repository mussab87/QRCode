using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Toolkit
{
    public class Randomizer
    {
        private static Random random;
        private static object syncObj = new object();

        public static void InitRandomNumber(int seed)
        {
            random = new Random(seed);
        }

        public static int GenerateRandomNumber(int max)
        {
            lock (syncObj)
            {
                if (random == null)
                    random = new Random();

                return random.Next(max);
            }
        }

        public static int GenerateRandomNumber(int min, int max)
        {
            lock (syncObj)
            {
                if (random == null)
                    random = new Random();

                return random.Next(min, max);
            }
        }

        public static int GenerateRandonId()
        {
            return GenerateRandomNumber(100, 999999999);
        }

        public static string GenerateRandomString(int stringLength)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[stringLength];

            if (random == null)
                random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }
    }
}
