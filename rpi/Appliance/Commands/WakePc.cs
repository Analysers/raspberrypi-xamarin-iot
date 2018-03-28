using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using MediatR;
using Serilog;

namespace Appliance.Commands
{
    /// <summary>
    /// Wake a sleeping PC via UDP magic packet
    /// </summary>
    public class WakePcHandler : AsyncRequestHandler<WakePcCommand>
    {
        private const string MacAddressOfDestinationPc = "AA:BB:CC:11:22:33";

        protected override async Task HandleCore(WakePcCommand command)
        {
            var mac = MacAddressOfDestinationPc.Split(':').Select(x => Convert.ToByte(x, 16)).ToArray();
            var counter = 0;

            var bytes = new byte[6 * 17];
            for (var i = 0; i < 6; i++)
            {
                bytes[counter++] = 0xFF;
            }

            //16x MAC
            for (var i = 0; i < 16; i++)
            {
                mac.CopyTo(bytes, 6 + i * 6);
            }

            try
            {
                using (var client = new UdpClient())
                {
                    client.EnableBroadcast = true;
                    await client.SendAsync(bytes, bytes.Length, new IPEndPoint(new IPAddress(0xffffffff), 9));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception during wake on lan request.");
            }
        }
    }

    public class WakePcCommand : IRequest
    {
    }
}
