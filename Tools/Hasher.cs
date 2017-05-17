namespace SkromPlexer.Tools
{
    public class Hasher
    {
        private static string StringTable = "0123456789ABCDEF";

        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            foreach (byte b in buff)
            {
                int c;

                c = b / 16;
                sbinary += StringTable[c];
                c = b % 16;
                sbinary += StringTable[c];
            }

            return (sbinary);
        }
    }
}
