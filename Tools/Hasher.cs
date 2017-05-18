namespace SkromPlexer.Tools
{
    /// <summary>
    /// A class that will convert byte arrays into a lisible string in Hexadecimal
    /// </summary>
    public class Hasher
    {
        private static string StringTable = "0123456789ABCDEF";

        /// <summary>
        /// Converts a byte array to a lisible string
        /// </summary>
        /// <param name="buff">the byte array to convert</param>
        /// <returns>A lisible string</returns>
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
