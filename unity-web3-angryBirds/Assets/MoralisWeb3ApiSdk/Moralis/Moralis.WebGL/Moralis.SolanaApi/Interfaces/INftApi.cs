using Moralis.WebGL.SolanaApi.Models;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.SolanaApi.Interfaces
{
    public interface INftApi
    {
        UniTask<NftMetadata> GetNFTMetadata(NetworkTypes network, string address);
    }
}
