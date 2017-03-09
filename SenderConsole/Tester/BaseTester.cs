using MaxP.PacketDotNet;
using SharpPcap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SenderConsole.Tester
{

    interface ISendPacket
    {
        bool IsTcp();
        ushort GetSourcePort();
        ushort GetDestnationPort();
        Packet GetSendPacket(PhysicalAddress sourceMAC, IPAddress sourceIPAddress);
    }

    interface IExpectedPacket
    {
        bool IsPass(byte[] data);
        byte[] GetData();
    }

    abstract class BaseTester : IObserver<CaptureEventArgs>
    {
        protected BaseTester next;
        protected ISendPacket sendPacket;
        protected IExpectedPacket expectedPacket;
        private IDisposable unsubscriber;
        bool passed = false;
        private ITestDevice _device;
        private const int WAIT_REPLY_TIMEOUT = 10000;

        BlockingCollection<CaptureEventArgs> queue = new BlockingCollection<CaptureEventArgs>();
        CancellationTokenSource cts ;
       
        public BaseTester(ISendPacket sendPacket, IExpectedPacket expectedPacket, BaseTester next)
        {
            this.next = next;
            this.sendPacket = sendPacket;
            this.expectedPacket = expectedPacket;
            
        }
        
        private void ReadPacketTask(CancellationToken token)
        {
            while (token.IsCancellationRequested == false)
            {
                CaptureEventArgs value = queue.Take();
                if (value == null)
                    continue;

                var dotnetpacket = Packet.ParsePacket(value.Packet.LinkLayerType, value.Packet.Data);
                if (sendPacket.IsTcp())
                {
                    TCPHandler(dotnetpacket);
                }
                else
                {
                    UDPHandler(dotnetpacket);
                }
            }

            unsubscriber.Dispose();
            if (next != null && passed == true)
            {
                next.Start(_device);
            }
        }
        
        private void UDPHandler(Packet dotnetPacket)
        {
            var udpPacket = UdpPacket.GetEncapsulated(dotnetPacket);
            if(udpPacket != null && udpPacket.DestinationPort == sendPacket.GetSourcePort())
                AssertReplyPacketData(udpPacket.PayloadData);
        }

        private void AssertReplyPacketData(byte[] data)
        {
            passed = expectedPacket.IsPass(data);
            if (passed == true)
                cts.Cancel();
        }

        private void TCPHandler(Packet dotnetPacket)
        {
            var tcpPacket = TcpPacket.GetEncapsulated(dotnetPacket);
            if (tcpPacket != null && tcpPacket.DestinationPort == sendPacket.GetSourcePort())
                AssertReplyPacketData(tcpPacket.PayloadData);
        }

        protected void DoNext(ITestDevice device)
        {
            cts = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(state => ReadPacketTask(cts.Token));

            _device = device;
            unsubscriber = device.Subscribe(this);
            _device.SendPacket(sendPacket.GetSendPacket(_device.GetPhysicalAddress(),_device.GetSourceIPAddress()));
        }


        public void OnNext(CaptureEventArgs value)
        {
            queue.Add(value);
        }

        public void OnError(Exception error)
        {

        }

        public void OnCompleted()
        {

        }
        


        abstract public void Start(ITestDevice device);
    }

    
}
