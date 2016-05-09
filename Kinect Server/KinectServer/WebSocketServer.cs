using Fleck;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Demonixis.Kinect.Server
{
    public enum BodySendType
    {
        FirstTracked = 0, AllTracked, All
    }

    public class WebSocketServer
    {
        private List<IWebSocketConnection> _sockets;
        private int _socketsCount;

        public void Start(ServerParameters serverParameters)
        {
            _sockets = new List<IWebSocketConnection>();

            var server = new Fleck.WebSocketServer(serverParameters.GetConnectionString());

            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    _sockets.Add(socket);
                    _socketsCount = _sockets.Count;
                    Console.WriteLine(string.Format("* -> Connected to {0}", socket.ConnectionInfo.ClientIpAddress));
                };

                socket.OnClose = () =>
                {
                    _sockets.Remove(socket);
                    _socketsCount = _sockets.Count;
                    Console.WriteLine(string.Format("* -> Disconnected from {0}", socket.ConnectionInfo.ClientIpAddress));
                };

                socket.OnMessage = OnMessageReceived;
            });

            Console.ReadKey();
        }

        protected virtual void OnMessageReceived(string message)
        {
            Console.WriteLine(message);
        }

        public void SendData(string json)
        {
            for (int i = 0; i < _socketsCount; i++)
                _sockets[i].Send(json);
        }

        public static string Serialize(object target)
        {
            return JsonConvert.SerializeObject(target);
        }

        #region Static CLI Helpers

        public void PrintServerHeader(BodySendType sendType, bool xboxOne)
        {
            Console.WriteLine("************************************");
            Console.WriteLine(string.Format("* Kinect Sensor for Xbox {0} Ready *", xboxOne ? "One" : "360"));
            Console.WriteLine("************************************");

            if (sendType == BodySendType.FirstTracked)
                Console.WriteLine("* Send Type: First Tracked Body    *");
            else if (sendType == BodySendType.AllTracked)
                Console.WriteLine("* Send Type: All Tracked Bodies    *");
            else
                Console.WriteLine("* Send Type: All Bodies            *");

            Console.WriteLine("************************************");
            Console.WriteLine("");
        }

        #endregion
    }
}
