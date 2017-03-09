using MaxP.Arpro.Probe;
using MaxP.PacketDotNet;
using SharpPcap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SenderConsole.Tester
{
    interface ITestDevice : IObservable<CaptureEventArgs>
    {
        void SendPacket(Packet packet);
        IPAddress GetSourceIPAddress();
        PhysicalAddress GetPhysicalAddress();
    }

    class TestDevice : ITestDevice
    {
        private ICaptureDevice _ncard;
        private List<IObserver<CaptureEventArgs>> observers = new List<IObserver<CaptureEventArgs>>();
        private Object locker = new Object();
        private PhysicalAddress _sourceMAC;
        private IPAddress _sourceIPAddress;

        public TestDevice(PhysicalAddress sourceMAC, IPAddress sourceIPAddress, NICHelper nicHelper)
        {
            _sourceIPAddress = sourceIPAddress;
            _sourceMAC = sourceMAC;

            _ncard = nicHelper.GetRawNIC(sourceMAC);
            _ncard.Open();
            _ncard.OnPacketArrival += new PacketArrivalEventHandler(onPacketArrival);
            _ncard.StartCapture();
        }

        public void SendPacket(Packet packet)
        {
            _ncard.SendPacket(packet);
        }

        private void onPacketArrival(object sender, CaptureEventArgs e)
        {
            lock (locker)
            {
                foreach (var observer in observers)
                    observer.OnNext(e);
            }
            
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<CaptureEventArgs>> _observers;
            private IObserver<CaptureEventArgs> _observer;
            private Object _locker;

            public Unsubscriber(List<IObserver<CaptureEventArgs>> observers, IObserver<CaptureEventArgs> observer, Object locker)
            {
                _observers = observers;
                _observer = observer;
                _locker = locker;
            }

            public void Dispose()
            {
                lock (_locker)
                {
                    if (!(_observer == null)) _observers.Remove(_observer);
                }
                
            }
        }

        public IDisposable Subscribe(IObserver<CaptureEventArgs> observer)
        {
            lock(locker)
            {
                if (!observers.Contains(observer))
                    observers.Add(observer);
            }
            

            return new Unsubscriber(observers, observer, locker);
        }

        public IPAddress GetSourceIPAddress()
        {
            return _sourceIPAddress;
        }

        public PhysicalAddress GetPhysicalAddress()
        {
            return _sourceMAC;
        }
    }
}
