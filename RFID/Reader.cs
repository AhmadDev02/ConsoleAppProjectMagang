using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp11.RFID
{
    public class Reader
    {
        private static readonly byte[] s_arrEmpty = new byte[0];

        // reader handel
        IntPtr m_handler = IntPtr.Zero;

        // reader open status，0：close；1：open by serial；2：open by net
        volatile int m_iState = 0;

        string m_ip = string.Empty;
        ushort m_netPort = 0;
        string m_sport = string.Empty;

        static Reader s_reader = null;

        public bool IsOpened
        {
            get { return m_iState != 0; }
        }

        public bool IsOpenedAsCom
        {
            get { return m_iState == 1; }
        }

        public bool IsOpenedAsNetwork
        {
            get { return m_iState == 2; }
        }

        public string IPAddress
        {
            get { return m_ip; }
        }

        public ushort NetPort
        {
            get { return m_netPort; }
        }

        public string PortName
        {
            get { return m_sport; }
        }

        public void Open(string port, byte Baudrate)
        {
            if (port == null)
                throw new ArgumentNullException("port(Serial)");
            port = port.Trim();
            if (port.Length == 0)
                throw new ArgumentException("please input serial", "port(serial)");
            if (m_iState != 0)
                throw new InvalidOperationException("reader is already opened");

            if (m_handler != IntPtr.Zero)
            {
                try { API.CloseDevice(m_handler); }
                catch { }
                m_handler = IntPtr.Zero;
                s_reader = null;
            }
            int state = API.OpenDevice(out m_handler, port, Baudrate);
            if (state != ReaderException.ERROR_SUCCESS)
            {
                m_handler = IntPtr.Zero;
                s_reader = null;
                throw new IOException("serial '" + port + "' open fail");
            }
            s_reader = this;
            m_sport = port;
            m_iState = 1;
        }

        public void Open(string ip, ushort port, uint timeoutMs, bool throwExcpOnTimeout)
        {
            if (ip == null)
                throw new ArgumentNullException("ip(addr)");
            ip = ip.Trim();
            if (ip.Length == 0)
                throw new ArgumentException("please input IP addr", "ip(addr)");
            if (port == 0)
                throw new ArgumentException("port can not be 0", "port(port )");
            if (m_iState != 0)
                throw new InvalidOperationException("reader is already opened");

            if (m_handler != IntPtr.Zero)
            {
                try { API.CloseDevice(m_handler); }
                catch { }
                m_handler = IntPtr.Zero;
                s_reader = null;
            }

            int state = API.OpenNetConnection(out m_handler, ip, port, timeoutMs);
            if (state != ReaderException.ERROR_SUCCESS)
            {
                m_handler = IntPtr.Zero;
                s_reader = null;
                if (state == ReaderException.ERROR_CMD_COMM_TIMEOUT && !throwExcpOnTimeout)
                    return;
                throw new IOException("IP'" + ip + "' port " + port + " connecting fail");
            }
            s_reader = this;
            m_ip = ip;
            m_netPort = port;
            m_iState = 2;
        }


        public void Close()
        {
            if (m_handler != IntPtr.Zero)
            {
                try { API.CloseDevice(m_handler); }
                catch { }
                m_handler = IntPtr.Zero;
                s_reader = null;
            }
            m_iState = 0;
        }

        public Devicepara GetDevicePara()
        {
            if (m_iState == 0)
                throw new InvalidOperationException("Reader is not open");
            Devicepara info;
            //DegbugPrint("Start GetVer");
            int state = API.GetDevicePara(m_handler, out info);
            //DegbugPrint("End GetVer");
            if (state == ReaderException.ERROR_SUCCESS)
                return info;
            throw new ReaderException(state);
        }

        public void SetDevicePara(Devicepara info)
        {
            if (m_iState == 0)
                throw new InvalidOperationException("Reader is not open");
            //DegbugPrint("Start GetVer");
            int state = API.SetDevicePara(m_handler, info);
            //DegbugPrint("End GetVer");
            if (state == ReaderException.ERROR_SUCCESS)
                return;
            throw new ReaderException(state);
        }

        public void SetRfTxPower(byte txPower, byte reserved)
        {
            if (m_iState == 0)
                throw new InvalidOperationException("Reader is not open");

            int state = API.SetRFPower(m_handler, txPower, reserved);
            if (state == ReaderException.ERROR_SUCCESS)
                return;
            throw new ReaderException(state);
        }



        public void Release_Relay(byte time)
        {
            if (m_iState == 0)
                throw new InvalidOperationException("Reader is not open");

            int state = API.Release_Relay(m_handler, time);
            if (state == ReaderException.ERROR_SUCCESS)
                return;
            throw new ReaderException(state);
        }


        public void Close_Relay(byte time)
        {
            if (m_iState == 0)
                throw new InvalidOperationException("Reader is not open");

            int state = API.Close_Relay(m_handler, time);
            if (state == ReaderException.ERROR_SUCCESS)
                return;
            throw new ReaderException(state);
        }




        public void Inventory(byte invCount, uint invParam)
        {
            if (m_iState == 0)
                throw new InvalidOperationException("Reader is not open");

            int state = API.InventoryContinue(m_handler, invCount, invParam);
            if (state == ReaderException.ERROR_SUCCESS)
                return;
            throw new ReaderException(state);
        }



        public void InventoryStop(ushort timeoutMs)
        {
            if (m_iState == 0)
                throw new InvalidOperationException("Reader is not open");

            int state = API.InventoryStop(m_handler, timeoutMs);
            if (state == ReaderException.ERROR_SUCCESS)
                return;
            throw new ReaderException(state);
        }

        public TagItem GetTagUii(ushort timeoutMs)
        {
            if (m_iState == 0)
                throw new InvalidOperationException("Reader is not open");

            TagInfo info;
            int state = API.GetTagUii(m_handler, out info, timeoutMs);
            if (state == ReaderException.ERROR_CMD_NO_TAG)
                return null;
            if (state == ReaderException.ERROR_SUCCESS)
                return new TagItem(info);
            throw new ReaderException(state);
        }




        static readonly ushort PRESET_VALUE = 0xFFFF;
        static readonly ushort POLYNOMIAL = 0x8408;

        public unsafe ushort Crc16Cal(byte[] pucY, ushort ucX, ushort CrcValue)
        {
            ushort ucI, ucJ;
            ushort uiCrcValue;
            if (CrcValue == 0xFFFF)    // first value
            {
                uiCrcValue = PRESET_VALUE;
            }
            else
            {
                uiCrcValue = CrcValue;
            }
            for (ucI = 1; ucI < ucX; ucI++)
            {
                uiCrcValue = (ushort)(uiCrcValue ^ pucY[ucI]);
                for (ucJ = 0; ucJ < 8; ucJ++)
                {
                    if ((uiCrcValue & 0x0001) != 0)
                    {
                        uiCrcValue = (ushort)((uiCrcValue >> 1) ^ POLYNOMIAL);
                    }
                    else
                    {
                        uiCrcValue = (ushort)(uiCrcValue >> 1);
                    }
                }
            }
            return uiCrcValue;
        }

    }

}
