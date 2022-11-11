using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp11.RFID
{
     class API
    {
        [DllImport("UHFPrimeReader.dll", CharSet = CharSet.Ansi)]
        public static extern int OpenDevice(out IntPtr m_hPort, string strComPort, byte Baudrate);

        [DllImport("UHFPrimeReader.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int OpenNetConnection(out IntPtr handler, string ip, ushort port, uint timeoutMs);

        [DllImport("UHFPrimeReader.dll")]
        public static extern int CloseDevice(IntPtr m_hPort);

        [DllImport("UHFPrimeReader.dll")]
        public static extern int GetDevicePara(IntPtr m_hPort, out Devicepara devInfo);

        [DllImport("UHFPrimeReader.dll")]
        public static extern int SetDevicePara(IntPtr m_hPort, Devicepara devInfo);


        [DllImport("UHFPrimeReader.dll")]
        public static extern int SetRFPower(IntPtr hComm, byte power, byte reserved);


        [DllImport("UHFPrimeReader.dll")]
        public static extern int InventoryContinue(IntPtr hComm, byte invCount, uint invParam);

        [DllImport("UHFPrimeReader.dll")]
        public static extern int GetTagUii(IntPtr hComm, out TagInfo tag_info, ushort timeout);

        [DllImport("UHFPrimeReader.dll")]
        public static extern int InventoryStop(IntPtr hComm, ushort timeout);

        [DllImport("UHFPrimeReader.dll")]
        public static extern int Release_Relay(IntPtr hComm, byte time);

        [DllImport("UHFPrimeReader.dll")]
        public static extern int Close_Relay(IntPtr hComm, byte time);

    }
}
