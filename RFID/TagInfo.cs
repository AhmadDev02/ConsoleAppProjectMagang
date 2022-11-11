using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp11.RFID
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TagInfo
    {
        /// <summary>
        /// 标签序号
        /// </summary>
        private ushort m_no;
        /// <summary>
        /// RSSI，单位：0.1dBm
        /// </summary>
        private short m_rssi;
        /// <summary>
        /// 天线索引
        /// </summary>
        private byte m_ant;
        /// <summary>
        /// 信道
        /// </summary>
        private byte m_channel;
        /// <summary>
        /// CRC
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        private byte[] m_crc;
        /// <summary>
        /// 标签的PC或编码长度+编码头数据
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        private byte[] m_pc;
        /// <summary>
        /// code中有效数据的长度
        /// </summary>
        private byte m_len;
        /// <summary>
        /// 标签的响应数据，长度255个byte
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
        private byte[] m_code;

        /// <summary>
        /// 标签序号
        /// </summary>
        public ushort NO
        {
            get { return m_no; }
        }

        /// <summary>
        /// 标签的PC或编码长度+编码头数据，长度2个byte
        /// </summary>
        public byte[] PC
        {
            get { return m_pc; }
        }

        /// <summary>
        /// code中有效数据的长度
        /// </summary>
        public byte CodeLength
        {
            get { return m_len; }
        }

        /// <summary>
        /// 标签的响应数据，长度255个byte
        /// </summary>
        public byte[] Code
        {
            get { return m_code; }
        }

        /// <summary>
        /// RSSI，单位：0.1dBm
        /// </summary>
        public short Rssi
        {
            get { return m_rssi; }
        }

        /// <summary>
        /// 天线接口序号
        /// </summary>
        public byte Antenna
        {
            get { return m_ant; }
        }

        /// <summary>
        /// 信道
        /// </summary>
        public byte Channel
        {
            get { return m_channel; }
        }

        /// <summary>
        /// CRC
        /// </summary>
        public byte[] CRC
        {
            get { return m_crc; }
        }

    }


}
