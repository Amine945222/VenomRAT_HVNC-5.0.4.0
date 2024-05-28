using System;

namespace Client.Algorithm
{
    public class GenerateKey
    {
        public int SizeOfKey;

        public GenerateKey(int sizeOfKey)
        {
            this.SizeOfKey = sizeOfKey;
        }

        public string GenerateStrenghCharacter()
        {
            string abc = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM123456789";
            string result = "";
            Random
                rnd = new Random(Guid.NewGuid()
                    .GetHashCode()); //Pour chaque fois que la fonction est appeller il y a une chaine de caractere different
            int iter = SizeOfKey;
            for (int i = 0; i < iter; i++)
                result += abc[rnd.Next(0, abc.Length)];
            return result;
        }
    }
}