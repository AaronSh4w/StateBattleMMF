﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NetLib.Discover
{
    public class DiscoverClient
    {
        public const bool LocalOnly = false;

        Dictionary<string, ServerItem> _knownSevers = new Dictionary<string, ServerItem>();

        ushort _port;
        Socket _sck;
        byte[] _recvBuffer;
        byte[] _sendBuffer;
        DateTime _stamp;

        public event Action<ServerItem> OnNewResponse = delegate { };

        public DiscoverClient(Guid id, ushort port)
        {
            _port = port;
            _sendBuffer = id.ToByteArray();

            _recvBuffer = new byte[4096];
            _sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

            _sck.Bind(new IPEndPoint(IPAddress.Any, 0));

            try
            {
                EndPoint rep = new IPEndPoint(IPAddress.Any, 0);
                _sck.BeginReceiveFrom(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, ref rep, RecvProc, null);
            }
            catch (SocketException ex)
            {
                System.Threading.Thread.Sleep(1000);
                EndPoint rep = new IPEndPoint(IPAddress.Any, 0);
                _sck.BeginReceiveFrom(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, ref rep, RecvProc, null);
            }
        }

        private void RecvProc(IAsyncResult ar)
        {
            try
            {
                EndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                var l = _sck.EndReceiveFrom(ar, ref ep);

                if (ep.AddressFamily == AddressFamily.InterNetwork)
                {
                    var ipep = (IPEndPoint)ep;

                    lock (_knownSevers)
                    {
                        var addr = ipep.Address.ToString();
                        var port = (ushort)ipep.Port;
                        var key = addr + ":" + port;

                        if (!_knownSevers.ContainsKey(key))
                        {
                            var ping = DateTime.Now - _stamp;

                            var item = new ServerItem(Encoding.UTF8.GetString(_recvBuffer, 0, l), addr, ping.TotalSeconds.ToString(), port);

                            _knownSevers.Add(key, item);

                            OnNewResponse(item);
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
            }

            EndPoint rep = new IPEndPoint(IPAddress.Any, 0);
            _sck.BeginReceiveFrom(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, ref rep, RecvProc, null);
        }

        public void Reset()
        {
            lock (_knownSevers)
            {
                _knownSevers.Clear();
                _stamp = DateTime.Now;
                _sck.SendTo(_sendBuffer, new IPEndPoint(LocalOnly ? IPAddress.Loopback : IPAddress.Broadcast, _port));
            }
        }
    }
}
