using ConsoleApp11.RFID;
using EasyNetQ;
using EasyNetQ.Topology;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApp11
{
    public class RfidReader
    {
        private static readonly int FLAG_IN_INVENTORY = BitVector32.CreateMask();
        private static readonly int FLAG_STOP_INVENTORY = BitVector32.CreateMask(FLAG_IN_INVENTORY);
        IAdvancedBus _bus;
        IExchange exchange;
        public RfidReader()
        {
            _bus = RabbitHutch.CreateBus(string.Format("host={0};virtualhost={1};username={2};password={3}",
            "localhost", "IBC", "admin", "admin123")).Advanced;
            exchange = _bus.ExchangeDeclare("amq.topic", "topic");
        }
        public Reader Reader
        {
            get { return m_reader; }
        }

        // 标识集合
        BitVector32 m_flags = new BitVector32();

        // 停止盘点等待的时间
        private static readonly ushort s_usStopInventoryTimeout = 10000;

        // 标签数据，用于标签按接收次序排序
        List<ShowTagItem> m_tags2 = new List<ShowTagItem>(1024);
        // 标签盘点响应总个数
        int m_iInvTagCount = 0;
        // 标签盘点总时间
        int m_iInvTimeMs = 1;
        // 开始盘点的时间
        int m_iInvStartTick = 0;
        // 盘点线程
        Thread m_thInventory = null;
        volatile bool m_bClosed = false;
        Reader m_reader = new Reader();
        Devicepara devicepara = new Devicepara();

        protected bool InInventory
        {
            get { return m_flags[FLAG_IN_INVENTORY]; }
            set { m_flags[FLAG_IN_INVENTORY] = value; }
        }


        public void Connect(string ip, int port)
        {
            try
            {
                if (ip.Length == 0)
                {
                    MessageBox.Show("Connect Failed，please input IP address");
                    return;
                }
                System.Net.IPAddress ip2;
                if (!System.Net.IPAddress.TryParse(ip, out ip2))
                {
                    MessageBox.Show( "Error IPV4 Address");
                    return;
                }
                ushort port2 = (ushort)port;
 
                if (m_reader.IsOpened)
                {
                    MessageBox.Show("reader is already opened ，please close first");
                    return;
                }

                m_reader.Open(ip, port2, 3000, true);   // 3000ms等待时间

                WriteLog(MessageType.Info, "reader open success，IP address：" + ip2.ToString() + "，port：" + port2.ToString(), null);
            }
            catch(Exception e) {
                WriteLog(MessageType.Error, e.Message, e);
            }
            InitReader();
        }
        public bool StopInventory
        {
            get { return m_flags[FLAG_STOP_INVENTORY]; }
            set { m_flags[FLAG_STOP_INVENTORY] = value; }
        }

        public bool IsClosed
        {
            get { return m_bClosed; }
        }


        private delegate void WriteLogHandler(MessageType type, string msg, Exception ex);
        private void InitReader()
        {
            try
            {
                Reader reader = this.Reader;
                if (reader.IsOpened)
                {

                    devicepara = reader.GetDevicePara();
                }
            }
            catch (Exception ex)
            {
                WriteLog(MessageType.Error, " Failed to get Power", ex);
                MessageBox.Show("Failed to get power：");
                return;
            }
        }
        public void Start()
        {
            try
            {
                Reader reader = this.Reader;
                if (reader == null)
                    throw new Exception("Reader not connected");

                if (InInventory)
                {
                        StopInventory = true;
                        CloseInventoryThread();
                        return;
                }
                    devicepara.Workmode = 1;
                    reader.SetDevicePara(devicepara);
                    InInventory = true;
                    StopInventory = false;

                    m_thInventory = new Thread(InventoryThread);
                    m_thInventory.Start();

            }
            catch (Exception ex)
            {
                InInventory = false;
                StopInventory = true;

                WriteLog(MessageType.Error, "Inventory label failed：", ex);
                MessageBox.Show("Inventory label failed：" + ex.Message, "Tips");
            }
        }
        private void CloseInventoryThread()
        {
            try
            {
                StopInventory = true;
                if (!m_thInventory.Join(4000))
                    m_thInventory.Abort();
            }
            catch { }
        }

        /// <summary>
        /// 盘点线程主函数
        /// </summary>
        private void InventoryThread()
        {
            try
            {
                Reader reader = this.Reader;
                if (reader == null)
                {
                    DoStopInventory();
                    return;
                }
                m_iInvTagCount = 0;
                m_iInvStartTick = Environment.TickCount;

                while (!StopInventory)
                {
                    TagItem item;             //接收标签数据
                    try
                    {
                        item = reader.GetTagUii(1000);

                    }
                    catch (ReaderException ex)
                    {
                        if (ex.ErrorCode == ReaderException.ERROR_CMD_COMM_TIMEOUT ||
                            ex.ErrorCode == ReaderException.ERROR_CMD_RESP_FORMAT_ERROR)
                        {
                            if (reader != null && this.IsClosed)
                            {
                                DoStopInventory();
                                return;
                            }
                            continue;
                        }
                        throw ex;
                    }
                    if (item == null)     
                        // 为空 表示周围没有标签或者指令结束
                        break;
                    if (item.Antenna == 0 || item.Antenna > 4)
                        continue;

                    ShowTagItem sitem; 
                    sitem = new ShowTagItem(item);
                    sitem.tag = Util.HexArrayToString(sitem.Code).Trim().Replace(" ","");
                    IMessage<ShowTagItem> message = new Message<ShowTagItem>(sitem);
                    _bus.Publish<ShowTagItem>(exchange, "bean_scan", true, message);
                    //m_iShowRow = 0;
                    m_iInvTagCount++;
                    m_iInvTimeMs = Environment.TickCount - m_iInvStartTick + 1;
                }
                _= new ThreadStart(OnInventoryEnd);
            }
            catch (Exception ex)
            {
                try
                {
                    WriteLog(MessageType.Error, "Inventory label failed：", ex);
                }
                catch { }
                DoStopInventory();
            }
        }
        private void DoStopInventory()
        {
            try
            {
                InInventory = false;
                StopInventory = true;
                try
                {
                    Reader reader = this.Reader;
                    if (reader != null)
                    {
                        reader.InventoryStop(s_usStopInventoryTimeout);
                    }
                }
                catch (Exception) { };
            }
            catch { }
            try
            {
                _ = new ThreadStart(OnInventoryEnd);
            }
            catch { }
        }
        private void OnInventoryEnd()
        {
            InInventory = false;
            StopInventory = true;
            WriteLog(MessageType.Info, "Inventory completed", null);
        }

        public void WriteLog(MessageType type, string msg, Exception ex)
        {
            try
            {


                StringBuilder sb = new StringBuilder(128);
                sb.Append(DateTime.Now);
                sb.Append(", ");
                switch (type)
                {
                    case MessageType.Info:
                        sb.Append("info：");
                        break;
                    case MessageType.Warning:
                        sb.Append("warning：");
                        break;
                    case MessageType.Error:
                        sb.Append("error：");
                        break;
                }
                if (msg.Length > 0)
                    sb.Append(msg);
                if (ex != null)
                    sb.Append(ex.Message);
                sb.Append("\r\n");
                string msg2 = sb.ToString();
                Console.WriteLine(msg2);
            }
            catch { }
        }
    }
}
