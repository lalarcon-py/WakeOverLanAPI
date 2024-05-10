using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Net;


namespace WakeOnLanController
{
    [ApiController]
    [Route("[controller]")]
    public class WakeOnLanController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> TriggerWakeOnLanAsync([FromBody] string macAddress, [FromQuery] string ipAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(ipAddress))
                {
                    return BadRequest("IP address is required.");
                }

                await SendWakeOnLanPacketAsync(macAddress, ipAddress);
                return Ok($"Wake-On-LAN packet sent successfully to {macAddress} using IP address {ipAddress}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to send Wake-On-LAN packet: {ex.Message}");
            }
        }

        private async Task SendWakeOnLanPacketAsync(string macAddress, string ipAddress)
        {
            byte[] magicPacket = BuildMagicPacketBytes(macAddress);
            await TransmitWakeOnLanPacketAsync(ipAddress, magicPacket);
        }

        private byte[] BuildMagicPacketBytes(string macAddress)
        {
            macAddress = Regex.Replace(macAddress, "[: -]", "");
            byte[] macBytes = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                macBytes[i] = Convert.ToByte(macAddress.Substring(i * 2, 2), 16);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < 6; i++)  //First 6 times 0xff
                    {
                        bw.Write((byte)0xff);
                    }
                    for (int i = 0; i < 16; i++) // then 16 times MacAddress
                    {
                        bw.Write(macBytes);
                    }
                }
                return ms.ToArray(); // 102 bytes magic packet
            }
        }

        private async Task TransmitWakeOnLanPacketAsync(string ipAddress, byte[] magicPacket)
        {
            using (UdpClient client = new UdpClient())
            {
                IPAddress multicastAddress = IPAddress.Parse(ipAddress);
                client.Connect(new IPEndPoint(multicastAddress, 9));
                await client.SendAsync(magicPacket, magicPacket.Length);
            }
        }
        
        private byte[] MacAddressToBytes(string macAddress)
        {
            macAddress = Regex.Replace(macAddress, "[: -]", "");
            byte[] macBytes = new byte[6];
            for (int i = 0; i < 6; i++)
                macBytes[i] = Convert.ToByte(macAddress.Substring(i * 2, 2), 16);
            return macBytes;
        }
        
        private bool IsValidMacAddress(string macAddress)
        {
            Regex macRegex = new Regex("^([0-9A-F]{2}[:-]){5}([0-9A-F]{2})$", RegexOptions.IgnoreCase);
            return macRegex.IsMatch(macAddress);
        }
    }
}