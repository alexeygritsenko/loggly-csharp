using Loggly;
using Loggly.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTester
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var config = LogglyConfig.Instance;
            config.Transport.EndpointHostname = "127.0.0.1";
            config.Transport.EndpointPort = 514;
            config.Transport.LogTransport = LogTransport.SyslogTcp;
            var syslog = new LogglyClient();

            var logEvent = new LogglyEvent();
            logEvent.Syslog.Level = Loggly.Transports.Syslog.Level.Warning;
            logEvent.Data.Add("message", "Oct 12 04:16:11 localhost CEF:0|nxlog.org|nxlog|2.7.1243|Executable Code was Detected|Advanced exploit detected|100|src=192.168.255.110 spt=46117 dst=172.25.212.204 dpt=80");
            var res = await syslog.Log(logEvent);
            Console.WriteLine(res);
            Console.ReadKey();
        }
    }
}
