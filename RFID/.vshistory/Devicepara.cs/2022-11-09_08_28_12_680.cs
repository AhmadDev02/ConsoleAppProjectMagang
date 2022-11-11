using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp11.RFID
{
    /// <summary>
    /// 设备参数
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Devicepara
    {

        private byte DEVICEARRD;
        private byte RFIDPRO;
        private byte WORKMODE;
        private byte INTERFACE;
        private byte BAUDRATE;
        private byte WGSET;
        private byte ANT;
        private byte REGION;
        private ushort STRATFREI;
        private ushort STRATFRED;
        private ushort STEPFRE;
        private byte CN;
        private byte RFIDPOWER;
        private byte INVENTORYAREA;
        private byte QVALUE;
        private byte SESSION;
        private byte ACSADDR;
        private byte ACSDATALEN;
        private byte FILTERTIME;
        private byte TRIGGLETIME;
        private byte BUZZERTIME;
        private byte INTERNELTIME;
        public byte Addr
        {
            get { return DEVICEARRD; }
            set { DEVICEARRD = value; }
        }
        public byte Protocol
        {
            get { return RFIDPRO; }
            set { RFIDPRO = value; }
        }
        public byte Baud
        {
            get { return BAUDRATE; }
            set { BAUDRATE = value; }
        }
        public byte Workmode
        {
            get { return WORKMODE; }
            set { WORKMODE = value; }
        }
        public byte port
        {
            get { return INTERFACE; }
            set { INTERFACE = value; }
        }
        public byte wieggand
        {
            get { return WGSET; }
            set { WGSET = value; }
        }
        public byte Ant
        {
            get { return ANT; }
            set { ANT = value; }
        }
        public byte Region
        {
            get { return REGION; }
            set { REGION = value; }
        }
        public byte Channel
        {
            get { return CN; }
            set { CN = value; }
        }
        public byte Power
        {
            get { return RFIDPOWER; }
            set { RFIDPOWER = value; }
        }
        public byte Area
        {
            get { return INVENTORYAREA; }
            set { INVENTORYAREA = value; }
        }
        public byte Q
        {
            get { return QVALUE; }
            set { QVALUE = value; }
        }
        public byte Session
        {
            get { return SESSION; }
            set { SESSION = value; }
        }
        public byte Startaddr
        {
            get { return ACSADDR; }
            set { ACSADDR = value; }
        }
        public byte DataLen
        {
            get { return ACSDATALEN; }
            set { ACSDATALEN = value; }
        }
        public byte Filtertime
        {
            get { return FILTERTIME; }
            set { FILTERTIME = value; }
        }
        public byte Triggletime
        {
            get { return TRIGGLETIME; }
            set { TRIGGLETIME = value; }
        }
        public byte Buzzertime
        {
            get { return BUZZERTIME; }
            set { BUZZERTIME = value; }
        }
        public byte IntenelTime
        {
            get { return INTERNELTIME; }
            set { INTERNELTIME = value; }
        }

        public ushort StartFreq
        {
            get
            {

                return (ushort)(STRATFREI >> 8 | STRATFREI << 8);

            }
            set
            {
                STRATFREI = (ushort)(value >> 8 | value << 8);          //大小端转换
            }
        }
        public ushort StartFreqde
        {
            get { return (ushort)(STRATFRED >> 8 | STRATFRED << 8); }
            set
            {
                STRATFRED = (ushort)(value >> 8 | value << 8);          //大小端转换
            }
        }
        public ushort Stepfreq
        {
            get { return (ushort)(STEPFRE >> 8 | STEPFRE << 8); }
            set
            {
                STEPFRE = (ushort)(value >> 8 | value << 8);          //大小端转换 
            }
        }
    };

}
