using ConsoleApp11.RFID;
using EasyNetQ;
using EasyNetQ.Topology;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp11
{
    class Program
    {
        static void Main(string[] args)
        {
            RfidReader rfidReader = new RfidReader();
            rfidReader.Connect("10.171.50.153", 2022);
            rfidReader.Start();

        }

    }
}
