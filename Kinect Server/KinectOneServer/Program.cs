using Microsoft.Kinect;
using System;
using System.Collections.Generic;

namespace Demonixis.Kinect.Server
{
    class Program
    {
        private static WebSocketServer _server;
        private static KinectSensor _kinectSensor;
        private static Body[] _bodies;
        private static List<Body> _tracked = new List<Body>(6);
        private static MultiSourceFrameReader _reader;
        private static BodySendType _sendType = BodySendType.AllTracked;

        public static void Main(string[] args)
        {
            if (InitilizeKinect())
            {
                var parameters = new ServerParameters(args);
                _sendType = parameters.SendType;
                _server = new WebSocketServer();
                _server.PrintServerHeader(_sendType, true);
                _server.Start(parameters);
            }
            else
            {
                Console.WriteLine("The Kinect sensor is not connected. Please connect it and restart the server.");
                Console.ReadKey();
            }
        }

        private static bool InitilizeKinect()
        {
            _kinectSensor = KinectSensor.GetDefault();

            if (_kinectSensor != null)
            {
                _kinectSensor.Open();
                _reader = _kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += OnSkeletonFrameReady;
            }

            return _kinectSensor != null;
        }

        private static void OnSkeletonFrameReady(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame == null)
                    return;

                var bodyCount = frame.BodyFrameSource.BodyCount;
                if (bodyCount == 0)
                    return;

                _bodies = new Body[bodyCount];
                frame.GetAndRefreshBodyData(_bodies);

                if (_sendType != BodySendType.All)
                {
                    if (_sendType == BodySendType.AllTracked)
                        _tracked.Clear();

                    for (int i = 0; i < bodyCount; i++)
                    {
                        if (_bodies[i] != null && _bodies[i].IsTracked)
                        {
                            if (_sendType == BodySendType.FirstTracked)
                            {
                                _server.SendData(_bodies[i].SerializeToArray());
                                return;
                            }

                            _tracked.Add(_bodies[i]);
                        }
                    }

                    _server.SendData(_tracked.Serialize());
                }
                else
                    _server.SendData(_bodies.Serialize());
            }
        }
    }
}
