using PinPadLib;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var pinpad = PinPad.Open("COM3"))
            {
                pinpad.GetInfo(0);
            }
        }
    }
}
