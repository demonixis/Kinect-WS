using Microsoft.Kinect;
using System;
using System.Collections.Generic;

namespace Demonixis.Kinect.Server
{
    class Program
    {
        private static WebSocketServer _server;
        private static KinectSensor _sensor;
        private static Skeleton[] _skeletons = null;
        private static List<Skeleton> _tracked = new List<Skeleton>(6);
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
            if (KinectSensor.KinectSensors.Count > 0)
            {
                _sensor = KinectSensor.KinectSensors[0];
                _sensor.SkeletonStream.Enable(new TransformSmoothParameters());
                _sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(OnSkeletonFrameReady);
                _sensor.Start();

                return true;
            }

            return false;
        }

        private static void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                // No player
                if (skeletonFrame == null)
                    return;

                var skeletonCount = skeletonFrame.SkeletonArrayLength;
                if (skeletonCount == 0)
                    return;

                _skeletons = new Skeleton[skeletonCount];
                skeletonFrame.CopySkeletonDataTo(_skeletons);

                if (_sendType != BodySendType.All)
                {
                    if (_sendType == BodySendType.AllTracked)
                        _tracked.Clear();

                    for (int i = 0; i < skeletonCount; i++)
                    {
                        if (_skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                        {
                            if (_sendType == BodySendType.FirstTracked)
                            {
                                _server.SendData(_skeletons[i].SerializeToArray());
                                return;
                            }

                            _tracked.Add(_skeletons[i]);
                        }
                    }

                    _server.SendData(_tracked.Serialize());
                }
                else
                    _server.SendData(_skeletons.Serialize());
            }
        }
    }
}
