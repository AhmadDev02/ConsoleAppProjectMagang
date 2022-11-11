using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp11.RFID
{
    /// <summary>
    /// 盘点到的标签项
    /// </summary>
    public class ShowTagItem
    {
        private static readonly byte[] s_arrEmpty = new byte[0];

        /// <summary>
        /// 被盘点到的次数，每个天线分别统计
        /// </summary>
        private int[] m_counts;

        /// <summary>
        /// 标签序号
        /// </summary>
        private ushort m_no;
        /// <summary>
        /// 标签的PC或编码长度+编码头数据
        /// </summary>
        private byte[] m_pc;
        /// <summary>
        /// 标签的UII或编码数据
        /// </summary>
        private byte[] m_code;
        /// <summary>
        /// RSSI，单位：0.1dBm
        /// </summary>
        private short m_rssi;
        /// <summary>
        /// 信道，值范围：1~4
        /// </summary>
        private byte m_channel;
        /// <summary>
        /// CRC
        /// </summary>
        private byte[] m_crc;
        /// <summary>
        /// codelength
        /// </summary>
        private byte m_len;

        /// <summary>
        /// 标签序号
        /// </summary>
        public ushort NO
        {
            get { return m_no; }
        }

        /// <summary>
        /// 标签的PC或编码长度+编码头数据
        /// </summary>
        public byte[] PC
        {
            get { return m_pc; }
        }

        /// <summary>
        /// 标签的UII或编码数据
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

        /// <summary>
        /// LEN
        /// </summary>
        public byte LEN
        {
            get { return m_len; }
        }

        /// <summary>
        /// 被盘点到的次数
        /// </summary>
        public int[] Counts
        {
            get { return m_counts; }
        }

        public ShowTagItem(TagItem item)
        {
            if (item.Code == null)
                throw new ArgumentNullException("item.Code");
            if (item.Antenna == 0 || item.Antenna > 4)
                throw new ArgumentOutOfRangeException("item.Antenna");

            m_counts = new int[4];
            m_counts[item.Antenna - 1] = 1;
            m_no = item.NO;
            m_pc = item.PC;
            m_code = item.Code;
            m_rssi = item.Rssi;
            m_channel = item.Channel;
            m_crc = item.CRC;
            m_len = item.LEN;
        }

        public ShowTagItem(ushort no, byte[] pc, byte[] code, short rssi, byte ant, byte channel, byte[] crc, byte len)
        {
            if (pc == null)
                throw new ArgumentNullException("pc");
            if (pc.Length != 2)
                throw new ArgumentException("PC must be 2 Byte");
            if (code == null)
                throw new ArgumentNullException("code");
            if (ant == 0 || ant > 4)
                throw new ArgumentOutOfRangeException("channel");

            m_counts = new int[4];
            m_counts[ant - 1] = 1;
            m_no = no;
            m_rssi = rssi;
            m_channel = channel;
            m_crc = crc;
            m_code = code;
            m_len = len;
        }

        /// <summary>
        /// 获取被盘点到的次数
        /// </summary>
        /// <returns></returns>
        public string CountsToString()
        {
            StringBuilder sb = new StringBuilder(64);
            for (int i = 0; i < m_counts.Length; i++)
            {
                sb.Append(m_counts[i]);
                sb.Append('/');
            }
            sb.Length -= 1;
            return sb.ToString();
        }

        public void IncCount(TagItem item)
        {
            if (item.Antenna > 0 && item.Antenna <= 4)
                m_counts[item.Antenna - 1]++;

            m_no = item.NO;
            m_pc = item.PC;
            m_code = item.Code;
            m_rssi = item.Rssi;
            m_channel = item.Channel;
            m_crc = item.CRC;
            m_len = item.LEN;
        }

        public bool CompareCode(byte[] code)
        {
            if (code == null)
                return false;
            if (m_code == code)
                return true;
            if (m_code.Length != code.Length)
                return false;
            for (int i = 0; i < code.Length; i++)
            {
                if (m_code[i] != code[i])
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            return Util.HexArrayToString(m_code);
        }
    }

}
