namespace Demonixis.Kinect.Server
{
    public sealed class ServerParameters
    {
        public string Ip { get; set; } = "127.0.0.1";
        public string Port { get; set; } = "8181";
        public BodySendType SendType { get; set; } = BodySendType.AllTracked;

        public ServerParameters(string[] args)
        {
            if (args != null)
            {
                string[] tmp;

                for (int i = 0, l = args.Length; i < l; i++)
                {
                    tmp = args[i].Split('=');

                    if (tmp.Length == 2)
                    {
                        switch (tmp[0])
                        {
                            case "ip": Ip = tmp[1]; break;
                            case "port": Port = tmp[1]; break;
                            case "sendType":
                                int sendType;
                                if (int.TryParse(tmp[1], out sendType))
                                    SendType = (BodySendType)sendType;
                                break;
                        }
                    }
                }
            }
        }

        public string GetConnectionString()
        {
            return string.Format("ws://{0}:{1}", Ip, Port);
        }
    }
}
