namespace FoxyMonitor.Utils
{
    internal static class CoinInfo
    {
        public static string GetCoinCurrencyCode(string poolName)
        {
            return poolName.ToLowerInvariant() switch
            {
                "chives_og" => "XCC",
                "hddcoin_og" => "HDD",
                "flax_og" => "XFX",
                _ => "XCH",
            };
        }
    }
}
