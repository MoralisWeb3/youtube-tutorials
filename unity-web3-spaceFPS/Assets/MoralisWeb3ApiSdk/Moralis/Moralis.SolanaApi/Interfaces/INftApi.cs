using Moralis.SolanaApi.Models;
using System.Threading.Tasks;

namespace Moralis.SolanaApi.Interfaces
{
    public interface INftApi
    {
        Task<NftMetadata> GetNFTMetadata(NetworkTypes network, string address);
    }
}
