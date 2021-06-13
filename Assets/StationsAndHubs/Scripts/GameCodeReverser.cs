using UnityEngine;

namespace StationsAndHubs.Scripts
{
    public class GameCodeReverser : MonoBehaviour
    {
        //static is temporary
        public static string CodeToIP(string code, int port)
        {
            var sum = 0;
            var i = code.Length;
            foreach (var c in code)
            {
                var x = CharToInt(c);
                sum += (int)(x * Mathf.Pow(35, i));
                i -= 1;
            }

            sum -= port;

            return IP(sum);
        }

        private static string IP(int sum)
        {
            return "";
        }

        private static int CharToInt(char c)
        {
            if (c >= 'A')
            {
                return (int)(c-'A');
            }

            return (char)(c - '1'+26);
            
        }
    }
}