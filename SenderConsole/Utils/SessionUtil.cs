using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MaxP.Arpro.Probe
{
    internal class SessionUtil
    {
        private static Dictionary<int, ProbeConnectionSession> _sessions = new Dictionary<int, ProbeConnectionSession>();
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ProbeConnectionSession GetLocalSession(string serverAddress, int port, int agentHashCode, IPAddress agentIp)
        {
            if (!_sessions.ContainsKey(agentHashCode))
                _sessions.Add(agentHashCode, new ProbeConnectionSession(serverAddress, port, agentIp));
            else
            {
                _sessions.Remove(agentHashCode);
                _sessions.Add(agentHashCode, new ProbeConnectionSession(serverAddress, port, agentIp));
            }
            return _sessions[agentHashCode];
        }


        public static ProbeConnectionSession GetLocalSession(int agentHashCode)
        {
            if (_sessions.ContainsKey(agentHashCode))
            {
                return _sessions[agentHashCode];
            }
            return null;
        }
    }
}
