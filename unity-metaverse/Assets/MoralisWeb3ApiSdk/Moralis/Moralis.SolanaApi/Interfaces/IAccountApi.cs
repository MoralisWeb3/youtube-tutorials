using Moralis.SolanaApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moralis.SolanaApi.Interfaces
{
    public interface IAccountApi
    {
        Task<NativeBalance> Balance(NetworkTypes network, string address);

        Task<List<SplTokenBalanace>> GetSplTokens(NetworkTypes network, string address);

        Task<List<SplNft>> GetNFTs(NetworkTypes network, string address);

        Task<Portfolio> GetPortfolio(NetworkTypes network, string address);
    }
}
