namespace Transmission.API.RPC.Arguments
{
    public static class RpcMethods
    {
        public static class Session
        {
            public const string Close = "session-close";
            public const string Get = "session-get";
            public const string Set = "session-set";
            public const string Stats = "session-stats";
        }

        public static class Torrent
        {
            public const string Add = "torrent-add";
            public const string Set = "torrent-set";
            public const string Get = "torrent-get";
            public const string Remove = "torrent-remove";
            public const string Start = "torrent-start";
            public const string StartNow = "torrent-start-now";
            public const string Stop = "torrent-stop";
            public const string Verify = "torrent-verify";
            public const string SetLocation = "torrent-set-location";
            public const string RenamePath = "torrent-rename-path";
        }

        public static class Queue
        {
            public const string MoveTop = "queue-move-top";
            public const string MoveUp = "queue-moce-up";
            public const string MoveDown = "queue-move-down";
            public const string MoveBottom = "queue-move-bottom";
        }

        public const string PortTest = "port-test";
        public const string BlocklistUpdate = "blocklist-update";
        public const string FreeSpace = "free-space";
    }
}
