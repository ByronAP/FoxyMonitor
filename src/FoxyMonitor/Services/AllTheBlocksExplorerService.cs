using FoxyMonitor.Contracts.Services;
using FoxyMonitor.Models.AtbResponses;
using FoxyPoolApi;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FoxyMonitor.Services
{
    public class AllTheBlocksExplorerService : IPostChainExplorerService
    {
        public string Base_Url { get => "https://alltheblocks.net"; }
        private const string API_Base_Url = "https://api.alltheblocks.net";

        private readonly ILogger<AllTheBlocksExplorerService> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly RestClient _restClient;

        public AllTheBlocksExplorerService(IMemoryCache memoryCache, ILogger<AllTheBlocksExplorerService> logger)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _restClient = new RestClient(API_Base_Url);
        }

        public async Task<ulong> GetAddressBalanceAsync(PostPool pool, string address)
        {

            var apiPoolName = PostPoolToExplorerCoinName(pool);
            var resourcePath = Path.Combine(apiPoolName, "address", "name", address);

            var request = new RestRequest(resourcePath, Method.GET);
            request.AddParameter("balanceBeforeHours", 24);

            var restResponse = await PerformRequestAsync<AtbAddressResponse>(request);

            if (restResponse == null || restResponse.CreateTimestamp == 0ul) throw new Exception($"Get {apiPoolName} address balance failed");

            return restResponse.Balance;
        }

        public async Task<ulong> GetAddressBalanceOnDateAsync(PostPool pool, string address, ulong timestamp)
        {
            var hours = (DateTimeOffset.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(timestamp))).TotalHours;

            var apiPoolName = PostPoolToExplorerCoinName(pool);
            var resourcePath = Path.Combine(apiPoolName, "address", "name", address);

            var request = new RestRequest(resourcePath, Method.GET);
            request.AddParameter("balanceBeforeHours", hours);

            var restResponse = await PerformRequestAsync<AtbAddressResponse>(request);

            if (restResponse == null || restResponse.CreateTimestamp == 0ul) throw new Exception($"Get {apiPoolName} address balance on date failed");

            return restResponse.BalanceBefore;
        }

        public async Task<uint> GetDifficultyAsync(PostPool pool)
        {
            var apiPoolName = PostPoolToExplorerCoinName(pool);
            var resourcePath = Path.Combine(apiPoolName, "chain");

            var request = new RestRequest(resourcePath, Method.GET);

            var restResponse = await PerformRequestAsync<ATBChainResponse>(request);

            if (restResponse == null || restResponse.PeakHeight == 0ul) throw new Exception($"Get {apiPoolName} chain difficulty failed");

            return restResponse.Difficulty;
        }

        public string GetExplorerUrl(PostPool pool) => Path.Combine(Base_Url, PostPoolToExplorerCoinName(pool));

        public async Task<ulong> GetHeightAsync(PostPool pool)
        {
            var apiPoolName = PostPoolToExplorerCoinName(pool);
            var resourcePath = Path.Combine(apiPoolName, "chain");

            var request = new RestRequest(resourcePath, Method.GET);

            var restResponse = await PerformRequestAsync<ATBChainResponse>(request);

            if (restResponse == null || restResponse.PeakHeight == 0ul) throw new Exception($"Get {apiPoolName} chain height failed");

            return restResponse.PeakHeight;
        }

        public async Task<decimal> GetNetspaceAsync(PostPool pool)
        {
            var apiPoolName = PostPoolToExplorerCoinName(pool);
            var resourcePath = Path.Combine(apiPoolName, "chain");

            var request = new RestRequest(resourcePath, Method.GET);

            var restResponse = await PerformRequestAsync<ATBChainResponse>(request);

            if (restResponse == null || restResponse.PeakHeight == 0ul) throw new Exception($"Get {apiPoolName} chain netspace failed");

            return restResponse.NetspaceBytes;
        }

        private async Task<T> PerformRequestAsync<T>(RestRequest request)
        {
            var cacheKey = request.Resource;
            foreach (var parameter in request.Parameters)
            {
                cacheKey += parameter.Name + parameter.Value;
            }

            try
            {
                if (_memoryCache.TryGetValue(request.Resource, out T cacheItem))
                {
                    if (cacheItem != null)
                    {
                        return cacheItem;
                    }
                }

                var restResponse = await _restClient.ExecuteAsync<T>(request);

                if (restResponse == null || !restResponse.IsSuccessful) return default;

                _memoryCache.Set(request.Resource, restResponse.Data, DateTimeOffset.UtcNow.AddMinutes(2));

                return restResponse.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rest request failed, Base: {} Resource: {Resource}", API_Base_Url, request.Resource);
                return default;
            }
        }

        public static string PostPoolToExplorerCoinName(PostPool pool)
        {
            return pool switch
            {
                PostPool.Chia => "chia",
                PostPool.Chia_OG => "chia",
                PostPool.Flax_OG => "flax",
                PostPool.Chives_OG => "chives",
                PostPool.HddCoin_OG => "hddcoin",
                _ => throw new ArgumentOutOfRangeException(nameof(pool)),
            };
        }

        public async Task<ATBCoinsResponse> GetCoinsByAddress(PostPool pool, string address, uint pageNumber = 0, uint pageSize = 100)
        {
            var apiPoolName = PostPoolToExplorerCoinName(pool);
            var resourcePath = Path.Combine(apiPoolName, "coin", "address");

            var request = new RestRequest(resourcePath, Method.GET);
            request.AddParameter("pageNumber", pageNumber);
            request.AddParameter("pageSize", pageSize);

            var restResponse = await PerformRequestAsync<ATBCoinsResponse>(request);

            if (restResponse == null || !(restResponse.First || restResponse.Last)) throw new Exception($"Get {apiPoolName} coins for address failed");

            return restResponse;
        }
    }
}
