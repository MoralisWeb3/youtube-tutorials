using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Moralis.Web3Api.Models;

namespace Moralis.Web3Api.Interfaces
{
	/// <summary>
	/// Represents a collection of functions to interact with the API endpoints
	/// </summary>
	public interface ITokenApi
	{
		/// <summary>
		/// Returns metadata (name, symbol, decimals, logo) for a given token contract address.
		/// </summary>
		/// <param name="addresses">The addresses to get metadata for</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="subdomain">The subdomain of the moralis server to use (Only use when selecting local devchain as chain)</param>
		/// <param name="providerUrl">web3 provider url to user when using local dev chain</param>
		/// <returns>Returns metadata (name, symbol, decimals, logo) for a given token contract address.</returns>
		Task<List<Erc20Metadata>> GetTokenMetadata (List<String> addresses, ChainList chain, string subdomain=null, string providerUrl=null);

		/// <summary>
		/// Get the nft trades for a given contracts and marketplace
		/// </summary>
		/// <param name="address">Address of the contract</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="fromBlock">The minimum block number from where to get the transfers
		/// * Provide the param 'from_block' or 'from_date'
		/// * If 'from_date' and 'from_block' are provided, 'from_block' will be used.
		/// </param>
		/// <param name="toBlock">To get the reserves at this block number</param>
		/// <param name="fromDate">The date from where to get the transfers (any format that is accepted by momentjs)
		/// * Provide the param 'from_block' or 'from_date'
		/// * If 'from_date' and 'from_block' are provided, 'from_block' will be used.
		/// </param>
		/// <param name="toDate">Get the reserves to this date (any format that is accepted by momentjs)
		/// * Provide the param 'to_block' or 'to_date'
		/// * If 'to_date' and 'to_block' are provided, 'to_block' will be used.
		/// </param>
		/// <param name="providerUrl">web3 provider url to user when using local dev chain</param>
		/// <param name="marketplace">marketplace from where to get the trades (only opensea is supported at the moment)</param>
		/// <param name="offset">offset</param>
		/// <param name="limit">limit</param>
		/// <returns>Returns the trades</returns>
		Task<TradeCollection> GetNFTTrades (string address, ChainList chain, int? fromBlock=null, string toBlock=null, string fromDate=null, string toDate=null, string providerUrl=null, string marketplace=null, int? offset=null, int? limit=null);

		/// <summary>
		/// Get the lowest price found for a nft token contract for the last x days (only trades paid in ETH)
		/// </summary>
		/// <param name="address">Address of the contract</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="days">The number of days to look back to find the lowest price
		/// If not provided 7 days will be the default
		/// </param>
		/// <param name="providerUrl">web3 provider url to user when using local dev chain</param>
		/// <param name="marketplace">marketplace from where to get the trades (only opensea is supported at the moment)</param>
		/// <returns>Returns the trade with the lowest price</returns>
		Task<Trade> GetNFTLowestPrice (string address, ChainList chain, int? days=null, string providerUrl=null, string marketplace=null);

		/// <summary>
		/// Returns metadata (name, symbol, decimals, logo) for a given token contract address.
		/// </summary>
		/// <param name="symbols">The symbols to get metadata for</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="subdomain">The subdomain of the moralis server to use (Only use when selecting local devchain as chain)</param>
		/// <returns>Returns metadata (name, symbol, decimals, logo) for a given token contract address.</returns>
		Task<List<Erc20Metadata>> GetTokenMetadataBySymbol (List<String> symbols, ChainList chain, string subdomain=null);

		/// <summary>
		/// Returns the price nominated in the native token and usd for a given token contract address.
		/// </summary>
		/// <param name="address">The address of the token contract</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="providerUrl">web3 provider url to user when using local dev chain</param>
		/// <param name="exchange">The factory name or address of the token exchange</param>
		/// <param name="toBlock">to_block</param>
		/// <returns>Returns the price nominated in the native token and usd for a given token contract address</returns>
		Task<Erc20Price> GetTokenPrice (string address, ChainList chain, string providerUrl=null, string exchange=null, int? toBlock=null);

		/// <summary>
		/// Gets ERC20 token contract transactions in descending order based on block number
		/// </summary>
		/// <param name="address">The address of the token contract</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="subdomain">The subdomain of the moralis server to use (Only use when selecting local devchain as chain)</param>
		/// <param name="fromBlock">The minimum block number from where to get the transfers
		/// * Provide the param 'from_block' or 'from_date'
		/// * If 'from_date' and 'from_block' are provided, 'from_block' will be used.
		/// </param>
		/// <param name="toBlock">The maximum block number from where to get the transfers.
		/// * Provide the param 'to_block' or 'to_date'
		/// * If 'to_date' and 'to_block' are provided, 'to_block' will be used.
		/// </param>
		/// <param name="fromDate">The date from where to get the transfers (any format that is accepted by momentjs)
		/// * Provide the param 'from_block' or 'from_date'
		/// * If 'from_date' and 'from_block' are provided, 'from_block' will be used.
		/// </param>
		/// <param name="toDate">Get the transfers to this date (any format that is accepted by momentjs)
		/// * Provide the param 'to_block' or 'to_date'
		/// * If 'to_date' and 'to_block' are provided, 'to_block' will be used.
		/// </param>
		/// <param name="offset">offset</param>
		/// <param name="limit">limit</param>
		/// <returns>Returns a collection of token contract transactions.</returns>
		Task<Erc20TransactionCollection> GetTokenAddressTransfers (string address, ChainList chain, string subdomain=null, int? fromBlock=null, int? toBlock=null, string fromDate=null, string toDate=null, int? offset=null, int? limit=null);

		/// <summary>
		/// Gets the amount which the spender is allowed to withdraw from the spender
		/// </summary>
		/// <param name="address">The address of the token contract</param>
		/// <param name="ownerAddress">The address of the token owner</param>
		/// <param name="spenderAddress">The address of the token spender</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="providerUrl">web3 provider url to user when using local dev chain</param>
		/// <returns>Returns the amount which the spender is allowed to withdraw from the owner..</returns>
		Task<Erc20Allowance> GetTokenAllowance (string address, string ownerAddress, string spenderAddress, ChainList chain, string providerUrl=null);

		/// <summary>
		/// Gets NFTs that match a given metadata search.
		/// </summary>
		/// <param name="q">The search string</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="format">The format of the token id</param>
		/// <param name="filter">What fields the search should match on. To look into the entire metadata set the value to 'global'. To have a better response time you can look into a specific field like name</param>
		/// <param name="fromBlock">The minimum block number from where to start the search
		/// * Provide the param 'from_block' or 'from_date'
		/// * If 'from_date' and 'from_block' are provided, 'from_block' will be used.
		/// </param>
		/// <param name="toBlock">The maximum block number from where to end the search
		/// * Provide the param 'to_block' or 'to_date'
		/// * If 'to_date' and 'to_block' are provided, 'to_block' will be used.
		/// </param>
		/// <param name="fromDate">The date from where to start the search (any format that is accepted by momentjs)
		/// * Provide the param 'from_block' or 'from_date'
		/// * If 'from_date' and 'from_block' are provided, 'from_block' will be used.
		/// </param>
		/// <param name="toDate">Get search results up until this date (any format that is accepted by momentjs)
		/// * Provide the param 'to_block' or 'to_date'
		/// * If 'to_date' and 'to_block' are provided, 'to_block' will be used.
		/// </param>
		/// <param name="offset">offset</param>
		/// <param name="limit">limit</param>
		/// <returns>Returns the matching NFTs</returns>
		Task<NftMetadataCollection> SearchNFTs (string q, ChainList chain, string format=null, string filter=null, int? fromBlock=null, int? toBlock=null, string fromDate=null, string toDate=null, int? offset=null, int? limit=null);

		/// <summary>
		/// Gets the transfers of the tokens from a block number to a block number
		/// </summary>
		/// <param name="chain">The chain to query</param>
		/// <param name="fromBlock">The minimum block number from where to get the transfers
		/// * Provide the param 'from_block' or 'from_date'
		/// * If 'from_date' and 'from_block' are provided, 'from_block' will be used.
		/// </param>
		/// <param name="toBlock">The maximum block number from where to get the transfers.
		/// * Provide the param 'to_block' or 'to_date'
		/// * If 'to_date' and 'to_block' are provided, 'to_block' will be used.
		/// </param>
		/// <param name="fromDate">The date from where to get the transfers (any format that is accepted by momentjs)
		/// * Provide the param 'from_block' or 'from_date'
		/// * If 'from_date' and 'from_block' are provided, 'from_block' will be used.
		/// </param>
		/// <param name="toDate">Get transfers up until this date (any format that is accepted by momentjs)
		/// * Provide the param 'to_block' or 'to_date'
		/// * If 'to_date' and 'to_block' are provided, 'to_block' will be used.
		/// </param>
		/// <param name="format">The format of the token id</param>
		/// <param name="offset">offset</param>
		/// <param name="limit">limit</param>
		/// <param name="cursor">The cursor returned in the last response (for getting the next page)
		/// </param>
		/// <returns>Returns a collection of NFT transfers</returns>
		Task<NftTransferCollection> GetNftTransfersFromToBlock (ChainList chain, int? fromBlock=null, int? toBlock=null, string fromDate=null, string toDate=null, string format=null, int? offset=null, int? limit=null, string cursor=null);

		/// <summary>
		/// Gets data, including metadata (where available), for all token ids for the given contract address.
		/// * Results are sorted by the block the token id was minted (descending) and limited to 100 per page by default
		/// * Requests for contract addresses not yet indexed will automatically start the indexing process for that NFT collection
		/// 
		/// </summary>
		/// <param name="address">Address of the contract</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="format">The format of the token id</param>
		/// <param name="offset">offset</param>
		/// <param name="limit">limit</param>
		/// <returns>Returns a collection of nfts</returns>
		Task<NftCollection> GetAllTokenIds (string address, ChainList chain, string format=null, int? offset=null, int? limit=null);

		/// <summary>
		/// Gets the transfers of the tokens matching the given parameters
		/// </summary>
		/// <param name="address">Address of the contract</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="format">The format of the token id</param>
		/// <param name="offset">offset</param>
		/// <param name="limit">limit</param>
		/// <param name="cursor">The cursor returned in the last response (for getting the next page)
		/// </param>
		/// <returns>Returns a collection of NFT transfers</returns>
		Task<NftTransferCollection> GetContractNFTTransfers (string address, ChainList chain, string format=null, int? offset=null, int? limit=null, string cursor=null);

		/// <summary>
		/// Gets all owners of NFT items within a given contract collection
		/// * Use after /nft/contract/{token_address} to find out who owns each token id in a collection
		/// * Make sure to include a sort parm on a column like block_number_minted for consistent pagination results
		/// * Requests for contract addresses not yet indexed will automatically start the indexing process for that NFT collection
		/// 
		/// </summary>
		/// <param name="address">Address of the contract</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="format">The format of the token id</param>
		/// <param name="offset">offset</param>
		/// <param name="limit">limit</param>
		/// <returns>Returns a collection of nft owners</returns>
		Task<NftOwnerCollection> GetNFTOwners (string address, ChainList chain, string format=null, int? offset=null, int? limit=null);

		/// <summary>
		/// Gets the contract level metadata (name, symbol, base token uri) for the given contract
		/// * Requests for contract addresses not yet indexed will automatically start the indexing process for that NFT collection
		/// 
		/// </summary>
		/// <param name="address">Address of the contract</param>
		/// <param name="chain">The chain to query</param>
		/// <returns>Returns a collection NFT collections.</returns>
		Task<NftContractMetadata> GetNFTMetadata (string address, ChainList chain);

		/// <summary>
		/// ReSync the metadata for an NFT
		/// * The metadata flag will request a resync of the metadata for an nft
		/// * The uri flag will request fetch the token_uri for an NFT and then fetch it's metadata. To be used when the token uri get updated
		/// 
		/// </summary>
		/// <param name="address">Address of the contract</param>
		/// <param name="tokenId">The id of the token</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="flag">the type of resync to operate</param>
		Task<Type> ReSyncMetadata (string address, string tokenId, ChainList chain, string flag=null);

		/// <summary>
		/// Sync a Contract for NFT Index
		/// 
		/// </summary>
		/// <param name="address">Address of the contract</param>
		/// <param name="chain">The chain to query</param>
		Task<Type> SyncNFTContract (string address, ChainList chain);

		/// <summary>
		/// Gets data, including metadata (where available), for the given token id of the given contract address.
		/// * Requests for contract addresses not yet indexed will automatically start the indexing process for that NFT collection
		/// 
		/// </summary>
		/// <param name="address">Address of the contract</param>
		/// <param name="tokenId">The id of the token</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="format">The format of the token id</param>
		/// <returns>Returns the specified NFT</returns>
		Task<Nft> GetTokenIdMetadata (string address, string tokenId, ChainList chain, string format=null);

		/// <summary>
		/// Gets all owners of NFT items within a given contract collection
		/// * Use after /nft/contract/{token_address} to find out who owns each token id in a collection
		/// * Make sure to include a sort parm on a column like block_number_minted for consistent pagination results
		/// * Requests for contract addresses not yet indexed will automatically start the indexing process for that NFT collection
		/// 
		/// </summary>
		/// <param name="address">Address of the contract</param>
		/// <param name="tokenId">The id of the token</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="format">The format of the token id</param>
		/// <param name="offset">offset</param>
		/// <param name="limit">limit</param>
		/// <returns>Returns a collection of NFTs with their respective owners</returns>
		Task<NftOwnerCollection> GetTokenIdOwners (string address, string tokenId, ChainList chain, string format=null, int? offset=null, int? limit=null);

		/// <summary>
		/// Gets the transfers of the tokens matching the given parameters
		/// </summary>
		/// <param name="address">Address of the contract</param>
		/// <param name="tokenId">The id of the token</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="format">The format of the token id</param>
		/// <param name="offset">offset</param>
		/// <param name="limit">limit</param>
		/// <param name="order">The field(s) to order on and if it should be ordered in ascending or descending order. Specified by: fieldName1.order,fieldName2.order. Example 1: "block_number", "block_number.ASC", "block_number.DESC", Example 2: "block_number and contract_type", "block_number.ASC,contract_type.DESC"</param>
		/// <param name="cursor">The cursor returned in the last response (for getting the next page)
		/// </param>
		/// <returns>Returns a collection of NFT transfers</returns>
		Task<NftTransferCollection> GetWalletTokenIdTransfers (string address, string tokenId, ChainList chain, string format=null, int? offset=null, int? limit=null, string order=null, string cursor=null);

	}
}
