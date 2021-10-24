using FoxyMonitor.Models.AtbResponses;
using FoxyPoolApi;
using System.Threading.Tasks;

namespace FoxyMonitor.Contracts.Services
{
    public interface IPostChainExplorerService
    {
        string Base_Url { get; }
        Task<ulong> GetAddressBalanceAsync(PostPool pool, string address);

        Task<ulong> GetAddressBalanceOnDateAsync(PostPool pool, string address, ulong timestamp);

        Task<decimal> GetNetspaceAsync(PostPool pool);

        Task<uint> GetDifficultyAsync(PostPool pool);

        Task<ulong> GetHeightAsync(PostPool pool);

        string GetExplorerUrl(PostPool pool);

        Task<ATBCoinsResponse> GetCoinsByAddress(PostPool pool, string address, uint pageNumber, uint pageSize);

    }
}
