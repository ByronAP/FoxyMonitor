using FoxyPoolApi;
using System;

namespace FoxyMonitor.Helpers
{
    public static class PostPoolBalanceConverter
    {
        public static decimal ConvertBalance(PostPool pool, ulong amount)
        {
#pragma warning disable IDE0066 // Convert switch statement to expression
            switch (pool)

            {
                case PostPool.Chia:
                case PostPool.Chia_OG:
                case PostPool.Flax_OG:
                case PostPool.Chives_OG:
                case PostPool.HddCoin_OG:
                default:
                    return Convert.ToDecimal(amount / Math.Pow(10, 12));
            }
#pragma warning restore IDE0066 // Convert switch statement to expression
        }
    }
}
