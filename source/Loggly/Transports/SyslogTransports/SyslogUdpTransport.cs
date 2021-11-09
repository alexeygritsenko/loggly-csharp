using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Loggly.Config;

namespace Loggly.Transports.Syslog
{
    internal class SyslogUdpTransport : SyslogTransportBase
    {
        private readonly UdpClientEx _udpClient;
        public SyslogUdpTransport()
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 0);
            _udpClient = new UdpClientEx(localEP);
        }

        public bool IsActive
        {
            get { return _udpClient.IsActive; }
        }

        public void Close()
        {
            if (_udpClient.IsActive)
            {
#if NETSTANDARD
                _udpClient.Dispose();
#else
                _udpClient.Close();
#endif
            }
        }


        protected override async Task<LogResponse> Send(SyslogMessage syslogMessage)
        {
            try
            {
                var targetIp = IPAddress.Parse(LogglyConfig.Instance.Transport.EndpointHostname);
                var targetEndPoint = new IPEndPoint(targetIp, LogglyConfig.Instance.Transport.EndpointPort);

                var bytes = syslogMessage.GetBytes();
                await _udpClient.SendAsync(
                    bytes,
                    bytes.Length,
                    targetEndPoint).ConfigureAwait(false);
                return new LogResponse() { Code = ResponseCode.Success };
            }
            catch (Exception ex)
            {
                LogglyException.Throw(ex, "Error when sending data using Udp client.");
                return new LogResponse() { Code = ResponseCode.Error, Message = $"{ex.GetType()}: {ex.Message}" };
            }
            finally
            {
                Close();
            }
        }
    }
}
