using System;

namespace AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Utils
{
    public class RpcPayloadId
    {
        private static readonly Random rng = new Random();
        private static readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        public static long Generate()
        {
            var date = (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds * 1000L;
            var extra = (long)Math.Floor(rng.NextDouble() * 1000.0);
            return date + extra;
        }
    }
}