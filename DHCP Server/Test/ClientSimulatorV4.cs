using PIXIS.DHCP.Option.V4;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Test
{
    public class ClientSimulatorV4
    {
        //private static Logger log = LoggerFactory.getLogger(ClientSimulatorV4.class);

        protected Random random = new Random();
        //protected Options options = new Options();
        //protected CommandLineParser parser = new BasicParser();
        //protected HelpFormatter formatter;

        protected IPAddress DEFAULT_ADDR;
        protected IPAddress serverAddr;
        protected int serverPort = DhcpConstants.V4_SERVER_PORT;
        protected IPAddress clientAddr;
        protected int clientPort = DhcpConstants.V4_SERVER_PORT;    // the test client acts as a relay
        protected bool rapidCommit = false;
        protected int numRequests = 100;
        //protected AtomicInteger discoversSent = new AtomicInteger();
        //protected AtomicInteger offersReceived = new AtomicInteger();
        //protected AtomicInteger requestsSent = new AtomicInteger();
        //protected AtomicInteger acksReceived = new AtomicInteger();
        //protected AtomicInteger releasesSent = new AtomicInteger();
        protected int successCnt = 0;
        protected long startTime = 0;
        protected long endTime = 0;
        protected long timeout = 0;
        protected int poolSize = 0;
        protected Object syncDone = new Object();

        protected IPEndPoint server = null;
        protected IPEndPoint client = null;

        //protected DatagramChannel channel = null;
        //protected ExecutorService executor = Executors.newCachedThreadPool();

        protected Dictionary<BigInteger, ClientMachine> clientMap = new Dictionary<BigInteger, ClientMachine>();


        /**
         * Instantiates a new test client.
         *
         * @param args the args
         * @throws Exception the exception
         */
        public ClientSimulatorV4()
        {
            DEFAULT_ADDR = IPAddress.Parse(GetLocalIPAddress());


            //Start();
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        protected IPAddress ParseIpAddressOption(string opt, string str, IPAddress defaddr)
        {
            IPAddress addr = defaddr;
            try
            {
                addr = IPAddress.Parse(str);
            }
            catch
            {
                addr = defaddr;
            }
            return addr;
        }

        /**
         * Start sending DHCPv4 DISCOVERs.
         */
        public void Start()
        {
            //DatagramChannelFactory factory =
            //    new NioDatagramChannelFactory(Executors.newCachedThreadPool());
            serverAddr = DEFAULT_ADDR;
            clientAddr = IPAddress.Parse("0.0.0.0");
            server = new IPEndPoint(serverAddr, serverPort);
            client = new IPEndPoint(clientAddr, clientPort);

            //ChannelPipeline pipeline = Channels.pipeline();
            //pipeline.addLast("logger", new LoggingHandler());
            //pipeline.addLast("encoder", new DhcpV4ChannelEncoder());
            //pipeline.addLast("decoder", new DhcpV4ChannelDecoder(client, false));
            //pipeline.addLast("executor", new ExecutionHandler(
            //        new OrderedMemoryAwareThreadPoolExecutor(16, 1048576, 1048576)));
            //pipeline.addLast("handler", this);

            //channel = factory.newChannel(pipeline);
            //channel.bind(client);

            //for (int i = 1; i <= numRequests; i++)
            //{
            //    executor.execute(new ClientMachine(i));
            //}
            new ClientMachine(1, serverAddr, clientAddr).Run();
            //synchronized(syncDone) {
            //    long ms = timeout * 1000;
            //    try
            //    {
            //        log.info("Waiting total of " + timeout + " milliseconds for completion");
            //        syncDone.wait(ms);
            //    }
            //    catch (InterruptedException ex)
            //    {
            //        log.error("Interrupted", ex);
            //    }
            //}

            //log.info("Complete: discoversSent=" + discoversSent +
            //        " offersReceived=" + offersReceived +
            //        " requestsSent=" + requestsSent +
            //        " acksReceived=" + acksReceived +
            //        " releasesSent=" + releasesSent +
            //        " elapsedTime=" + (endTime - startTime) + "ms");

            //log.info("Shutting down executor...");
            //executor.shutdownNow();
            //log.info("Closing channel...");
            //channel.close();
            //log.info("Done.");
            //if ((discoversSent.get() == offersReceived.get()) &&
            //        (requestsSent.get() == acksReceived.get()) &&
            //        (releasesSent.get() == numRequests))
            //{

            //    System.exit(0);
            //}
            //else
            //{
            //    System.exit(1);
            //}
        }

        /**
         * The Class ClientMachine.
         */
        protected class ClientMachine
        {

            DhcpV4Message msg;
            int id;
            byte[] mac;
            BigInteger key;
            protected int serverPort = DhcpConstants.V4_SERVER_PORT;
            private IPAddress serverAddr;
            private IPAddress clientAddr;

            public ClientMachine(int id)
            {
                this.id = id;
                this.mac = BuildChAddr(id);
                this.key = new BigInteger(mac);
            }

            public ClientMachine(int id, IPAddress serverAddr, IPAddress clientAddr) : this(id)
            {
                this.id = id;
                this.mac = BuildChAddr(id);
                this.key = new BigInteger(mac);
                this.serverAddr = serverAddr;
                this.clientAddr = clientAddr;
            }

            /* (non-Javadoc)
             * @see java.lang.Runnable#run()
             */
            public void Run()
            {
                //if (poolSize > 0)
                //{
                //    synchronized(clientMap) {
                //        if (poolSize <= clientMap.size())
                //        {
                //            try
                //            {
                //                log.info("Waiting for release...");
                //                clientMap.wait();
                //            }
                //            catch (InterruptedException ex)
                //            {
                //                log.error("Interrupted", ex);
                //            }
                //        }
                //        clientMap.put(key, this);
                //    }
                //}
                //else
                //{
                //    clientMap.put(key, this);
                //}
                Discover();
            }

            public void Discover()
            {
                msg = BuildDiscoverMessage(mac);
                //ChannelFuture future = channel.write(msg, server);
                //future.addListener(this);
                Console.WriteLine(msg.ToString());
                Request(msg);
            }

            public void Request(DhcpV4Message offerMsg)
            {
                msg = BuildRequestMessage(offerMsg);
                //ChannelFuture future = channel.write(msg, server);
                //future.addListener(this);
                Console.WriteLine(msg.ToString());
                Release(msg);
            }

            public void Release(DhcpV4Message ackMsg)
            {
                msg = BuildReleaseMessage(ackMsg);
                //ChannelFuture future = channel.write(msg, server);
                //future.addListener(this);
                Console.WriteLine(msg.ToString());
            }

            /* (non-Javadoc)
             * @see org.jboss.netty.channel.ChannelFutureListener#operationComplete(org.jboss.netty.channel.ChannelFuture)
             */
            //    public void operationComplete(ChannelFuture future)
            //    {
            //        if (future.isSuccess())
            //        {
            //            if (startTime == 0)
            //            {
            //                startTime = System.currentTimeMillis();
            //                log.info("Starting at: " + startTime);
            //            }
            //            if (msg.getMessageType() == DhcpConstants.V4MESSAGE_TYPE_DISCOVER)
            //            {
            //                discoversSent.getAndIncrement();
            //                log.info("Succesfully sent discover message mac=" + Util.toHexString(mac) +
            //                        " cnt=" + discoversSent);
            //            }
            //            else if (msg.getMessageType() == DhcpConstants.V4MESSAGE_TYPE_REQUEST)
            //            {
            //                requestsSent.getAndIncrement();
            //                log.info("Succesfully sent request message mac=" + Util.toHexString(mac) +
            //                        " cnt=" + requestsSent);
            //            }
            //            else if (msg.getMessageType() == DhcpConstants.V4MESSAGE_TYPE_RELEASE)
            //            {
            //                clientMap.remove(key);
            //                releasesSent.getAndIncrement();
            //                log.info("Succesfully sent release message mac=" + Util.toHexString(mac) +
            //                        " cnt=" + releasesSent);
            //                if (releasesSent.get() == numRequests)
            //                {
            //                    endTime = System.currentTimeMillis();
            //                    log.info("Ending at: " + endTime);
            //                    synchronized(syncDone) {
            //                        syncDone.notifyAll();
            //                    }
            //                }
            //                else if (poolSize > 0)
            //                {
            //                    synchronized(clientMap) {
            //                        clientMap.notify();
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            log.warn("Failed to send message id=" + msg.getTransactionId());
            //        }
            //    }
            //}
            private byte[] BuildChAddr(long id)
            {
                byte[] bid = new BigInteger(id).GetBytes();
                byte[] chAddr = new byte[6];
                chAddr[0] = (byte)0xde;
                chAddr[1] = (byte)0xb1;
                if (bid.Length == 4)
                {
                    chAddr[2] = bid[0];
                    chAddr[3] = bid[1];
                    chAddr[4] = bid[2];
                    chAddr[5] = bid[3];
                }
                else if (bid.Length == 3)
                {
                    chAddr[2] = 0;
                    chAddr[3] = bid[0];
                    chAddr[4] = bid[1];
                    chAddr[5] = bid[2];
                }
                else if (bid.Length == 2)
                {
                    chAddr[2] = 0;
                    chAddr[3] = 0;
                    chAddr[4] = bid[0];
                    chAddr[5] = bid[1];
                }
                else if (bid.Length == 1)
                {
                    chAddr[2] = 0;
                    chAddr[3] = 0;
                    chAddr[4] = 0;
                    chAddr[5] = bid[0];
                }
                return chAddr;
            }
            /**
         * Builds the discover message.
         * 
         * @return the  dhcp message
         */
            private DhcpV4Message BuildDiscoverMessage(byte[] chAddr)
            {
                DhcpV4Message msg = new DhcpV4Message(IPAddress.Parse("0.0.0.0"), new IPEndPoint(serverAddr, serverPort));

                msg.SetOp((short)DhcpConstants.V4_OP_REQUEST);
                msg.SetTransactionId(GetRandomLong());
                msg.SetHtype((short)1); // ethernet
                msg.SetHlen((byte)6);
                msg.SetChAddr(chAddr);
                msg.SetGiAddr(clientAddr);  // look like a relay to the DHCP server

                DhcpV4MsgTypeOption msgTypeOption = new DhcpV4MsgTypeOption();
                msgTypeOption.SetUnsignedByte((short)DhcpConstants.V4MESSAGE_TYPE_DISCOVER);

                msg.PutDhcpOption(msgTypeOption);

                return msg;
            }

           

            private DhcpV4Message BuildRequestMessage(DhcpV4Message offer)
            {

                DhcpV4Message msg = new DhcpV4Message(IPAddress.Parse("0.0.0.0"), new IPEndPoint(serverAddr, serverPort));

                msg.SetOp((short)DhcpConstants.V4_OP_REQUEST);
                msg.SetTransactionId(offer.GetTransactionId());
                msg.SetHtype((short)1); // ethernet
                msg.SetHlen((byte)6);
                msg.SetChAddr(offer.GetChAddr());
                msg.SetGiAddr(clientAddr);  // look like a relay to the DHCP server

                DhcpV4MsgTypeOption msgTypeOption = new DhcpV4MsgTypeOption();
                msgTypeOption.SetUnsignedByte((short)DhcpConstants.V4MESSAGE_TYPE_REQUEST);

                msg.PutDhcpOption(msgTypeOption);

                DhcpV4RequestedIpAddressOption reqIpOption = new DhcpV4RequestedIpAddressOption();
                reqIpOption.SetIpAddress(offer.GetYiAddr().ToString());
                msg.PutDhcpOption(reqIpOption);

                return msg;
            }

            private DhcpV4Message BuildReleaseMessage(DhcpV4Message ack)
            {

                DhcpV4Message msg = new DhcpV4Message(null, new IPEndPoint(serverAddr, serverPort));

                msg.SetOp((short)DhcpConstants.V4_OP_REQUEST);
                msg.SetTransactionId(ack.GetTransactionId());
                msg.SetHtype((short)1); // ethernet
                msg.SetHlen((byte)6);
                msg.SetChAddr(ack.GetChAddr());
                msg.SetGiAddr(clientAddr);  // look like a relay to the DHCP server
                msg.SetCiAddr(ack.GetYiAddr());

                DhcpV4MsgTypeOption msgTypeOption = new DhcpV4MsgTypeOption();
                msgTypeOption.SetUnsignedByte((short)DhcpConstants.V4MESSAGE_TYPE_RELEASE);

                msg.PutDhcpOption(msgTypeOption);

                return msg;
            }

            private static long GetRandomLong()
            {
                Random rnd = new Random();

                byte[] buf = new byte[8];
                rnd.NextBytes(buf);
                long longRand = BitConverter.ToInt64(buf, 0);

                long result = (Math.Abs(longRand % (2000000000000000 - 1000000000000000)) + 1000000000000000);

                long random_seed = (long)rnd.Next(1000, 5000);
                random_seed = random_seed * result + rnd.Next(1000, 5000);
                return ((long)(random_seed / 655) % 10000000000000001);
            }
        }



        



        /**
         * The main method.
         * 
         * @param args the arguments
         */
        //public static void main(String[] args)
        //{
        //    try
        //    {
        //        new ClientSimulatorV4(args);
        //    }
        //    catch (Exception e)
        //    {
        //        //e.printStackTrace();
        //    }
        //}
    }
}