namespace PinPadLib.Utils
{
    public static class Crc16
    {
        public static ushort Compute(byte[] data)
        {
            const ushort mask = 0x1021;
            ushort crc = 0;
            foreach (var b in data)
            {
                var word = b << 8;
                for (byte i = 0; i < 8; i++)
                {
                    if (((crc ^ word) & 0x8000) != 0)
                    {
                        crc <<= 1;
                        crc ^= mask;
                    }
                    else
                    {
                        crc <<= 1;
                    }

                    word <<= 1;
                }
            }
            return crc;
        }
    }
}
