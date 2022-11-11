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
    public class TagItem
    {
        private static readonly byte[] s_arrEmpty = new byte[0];

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
        private byte[] m_crc;
        /// <summary>
        /// codelengh
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

        /// <summary>
        /// len
        /// </summary>
        public byte LEN
        {
            get { return m_len; }
        }

        public TagItem(ushort no, byte[] pc, byte codeLen, byte[] code, short rssi, byte ant, byte channel, byte[] crc, byte len)
        {
            if (pc == null)
                throw new ArgumentNullException("pc");
            if (crc == null)
                throw new ArgumentNullException("crc");
            if (pc.Length != 2)
                throw new ArgumentException("PC must be 2 Byte");
            if (codeLen > 0 && code == null)
                throw new ArgumentNullException("code");
            if (codeLen > code.Length)
                throw new ArgumentOutOfRangeException("codeLen", "codelen is over");

            m_no = no;
            m_pc = pc;
            m_rssi = rssi;
            m_ant = ant;
            m_channel = channel;
            m_crc = crc;
            m_len = len;
            if (codeLen > 0)
            {
                m_code = new byte[codeLen];
                Array.Copy(code, m_code, codeLen);
            }
            else
                m_code = s_arrEmpty;
        }

        internal TagItem(TagInfo info)
        {
            //if (info.PC == null)
            //    throw new ArgumentNullException("pc");
            //if (info.CRC == null)
            //    throw new ArgumentNullException("crc");
            //if (info.PC.Length != 2)
            //    throw new ArgumentException("PC must be 2 Byte");
            byte codeLen = info.CodeLength;
            if (codeLen > 0 && info.Code == null)
                throw new ArgumentNullException("code");
            if (codeLen > info.Code.Length)
                throw new ArgumentOutOfRangeException("codeLen", "codelen is over");

            //m_no = info.NO;
            //m_pc = info.PC;
            m_len = info.CodeLength;         // 数据长度
            m_rssi = info.Rssi;
            m_ant = info.Antenna;
            m_channel = info.Channel;
            //m_crc = info.CRC;
            if (codeLen > 0)
            {
                m_code = new byte[codeLen];
                Array.Copy(info.Code, m_code, codeLen);
            }
            else
                m_code = s_arrEmpty;
        }

        public override int GetHashCode()
        {
            int hash = add(0, m_pc);
            hash = add(hash, m_crc);
            hash = add(hash, m_code);
            return hash;
        }

        public override bool Equals(object obj)
        {
            TagItem item = obj as TagItem;
            if (item == null)
                return false;
            return compare(m_crc, item.m_crc) &&
                compare(m_code, item.m_code) &&
                compare(m_pc, item.m_pc);
        }

        private bool compare(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }

        private int add(int hash, byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if ((hash & 0x80000000) != 0)
                    hash = (hash << 1) + 1;
                else
                    hash <<= 1;
            }
            return hash;
        }
    }

}
