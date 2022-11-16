/* eslint-disable @typescript-eslint/no-var-requires */
/* eslint-disable @typescript-eslint/no-explicit-any */
import Moralis from 'moralis'
import { MoralisError } from '@moralisweb3/core';
import { handleRateLimit } from '../../rateLimit'
import { AxiosError } from 'axios'
declare const Parse: any;

const getErrorMessage = (error: Error, name: string) => {
  // Resolve Axios data inside the MoralisError
  if (
    error instanceof MoralisError &&
    error.cause &&
    error.cause instanceof AxiosError &&
    error.cause.response &&
    error.cause.response.data
  ) {
    return JSON.stringify(error.cause.response.data);
  }

  if (error instanceof Error) {
    return error.message;
  } 

  return `API error while calling ${name}`
}

const beforeApiRequest = async (user: any, ip: any, name: string) => {
  if (!(await handleRateLimit(user, ip))) {
    throw new Error(
      `Too many requests to ${name} API from this particular client, the clients needs to wait before sending more requests.`
    );
  }
}

Parse.Cloud.define("getBlock", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getBlock');
    const result = await Moralis.EvmApi.native.getBlock(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getBlock'));
  }
})

Parse.Cloud.define("getDateToBlock", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getDateToBlock');
    const result = await Moralis.EvmApi.native.getDateToBlock(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getDateToBlock'));
  }
})

Parse.Cloud.define("getLogsByAddress", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getLogsByAddress');
    const result = await Moralis.EvmApi.native.getLogsByAddress(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getLogsByAddress'));
  }
})

Parse.Cloud.define("getNFTTransfersByBlock", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getNFTTransfersByBlock');
    const result = await Moralis.EvmApi.native.getNFTTransfersByBlock(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getNFTTransfersByBlock'));
  }
})

Parse.Cloud.define("getTransaction", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getTransaction');
    const result = await Moralis.EvmApi.native.getTransaction(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getTransaction'));
  }
})

Parse.Cloud.define("getContractEvents", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getContractEvents');
    const result = await Moralis.EvmApi.native.getContractEvents(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getContractEvents'));
  }
})

Parse.Cloud.define("runContractFunction", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'runContractFunction');
    const result = await Moralis.EvmApi.native.runContractFunction(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'runContractFunction'));
  }
})

Parse.Cloud.define("getTransactions", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getTransactions');
    const result = await Moralis.EvmApi.account.getTransactions(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getTransactions'));
  }
})

Parse.Cloud.define("getNativeBalance", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getNativeBalance');
    const result = await Moralis.EvmApi.account.getNativeBalance(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getNativeBalance'));
  }
})

Parse.Cloud.define("getTokenBalances", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getTokenBalances');
    const result = await Moralis.EvmApi.account.getTokenBalances(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getTokenBalances'));
  }
})

Parse.Cloud.define("getTokenTransfers", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getTokenTransfers');
    const result = await Moralis.EvmApi.account.getTokenTransfers(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getTokenTransfers'));
  }
})

Parse.Cloud.define("getNFTs", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getNFTs');
    const result = await Moralis.EvmApi.account.getNFTs(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getNFTs'));
  }
})

Parse.Cloud.define("getNFTTransfers", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getNFTTransfers');
    const result = await Moralis.EvmApi.account.getNFTTransfers(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getNFTTransfers'));
  }
})

Parse.Cloud.define("getWalletNFTCollections", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getWalletNFTCollections');
    //@ts-ignore
    const result = await Moralis.EvmApi.account.getWalletNFTCollections(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getWalletNFTCollections'));
  }
})

Parse.Cloud.define("getNFTsForContract", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getNFTsForContract');
    const result = await Moralis.EvmApi.account.getNFTsForContract(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getNFTsForContract'));
  }
})

Parse.Cloud.define("getTokenMetadata", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getTokenMetadata');
    const result = await Moralis.EvmApi.token.getTokenMetadata(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getTokenMetadata'));
  }
})

Parse.Cloud.define("getNFTTrades", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getNFTTrades');
    const result = await Moralis.EvmApi.token.getNFTTrades(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getNFTTrades'));
  }
})

Parse.Cloud.define("getNFTLowestPrice", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getNFTLowestPrice');
    const result = await Moralis.EvmApi.token.getNFTLowestPrice(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getNFTLowestPrice'));
  }
})

Parse.Cloud.define("getTokenMetadataBySymbol", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getTokenMetadataBySymbol');
    const result = await Moralis.EvmApi.token.getTokenMetadataBySymbol(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getTokenMetadataBySymbol'));
  }
})

Parse.Cloud.define("getTokenPrice", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getTokenPrice');
    const result = await Moralis.EvmApi.token.getTokenPrice(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getTokenPrice'));
  }
})

Parse.Cloud.define("getTokenAddressTransfers", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getTokenAddressTransfers');
    const result = await Moralis.EvmApi.token.getTokenAddressTransfers(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getTokenAddressTransfers'));
  }
})

Parse.Cloud.define("getTokenAllowance", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getTokenAllowance');
    const result = await Moralis.EvmApi.token.getTokenAllowance(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getTokenAllowance'));
  }
})

Parse.Cloud.define("searchNFTs", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'searchNFTs');
    const result = await Moralis.EvmApi.token.searchNFTs(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'searchNFTs'));
  }
})

Parse.Cloud.define("getNftTransfersFromToBlock", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getNftTransfersFromToBlock');
    const result = await Moralis.EvmApi.token.getNftTransfersFromToBlock(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getNftTransfersFromToBlock'));
  }
})

Parse.Cloud.define("getAllTokenIds", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getAllTokenIds');
    const result = await Moralis.EvmApi.token.getAllTokenIds(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getAllTokenIds'));
  }
})

Parse.Cloud.define("getContractNFTTransfers", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getContractNFTTransfers');
    const result = await Moralis.EvmApi.token.getContractNFTTransfers(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getContractNFTTransfers'));
  }
})

Parse.Cloud.define("getNFTOwners", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getNFTOwners');
    const result = await Moralis.EvmApi.token.getNFTOwners(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getNFTOwners'));
  }
})

Parse.Cloud.define("getNFTMetadata", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getNFTMetadata');
    const result = await Moralis.EvmApi.token.getNFTMetadata(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getNFTMetadata'));
  }
})

Parse.Cloud.define("reSyncMetadata", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'reSyncMetadata');
    const result = await Moralis.EvmApi.token.reSyncMetadata(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'reSyncMetadata'));
  }
})

Parse.Cloud.define("syncNFTContract", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'syncNFTContract');
    //@ts-ignore
    const result = await Moralis.EvmApi.contract.syncNFTContract(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'syncNFTContract'));
  }
})

Parse.Cloud.define("getTokenIdMetadata", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getTokenIdMetadata');
    const result = await Moralis.EvmApi.token.getTokenIdMetadata(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getTokenIdMetadata'));
  }
})

Parse.Cloud.define("getTokenIdOwners", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getTokenIdOwners');
    const result = await Moralis.EvmApi.token.getTokenIdOwners(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getTokenIdOwners'));
  }
})

Parse.Cloud.define("getWalletTokenIdTransfers", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getWalletTokenIdTransfers');
    const result = await Moralis.EvmApi.token.getWalletTokenIdTransfers(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getWalletTokenIdTransfers'));
  }
})

Parse.Cloud.define("resolveDomain", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'resolveDomain');
    const result = await Moralis.EvmApi.resolve.resolveDomain(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'resolveDomain'));
  }
})

Parse.Cloud.define("resolveAddress", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'resolveAddress');
    const result = await Moralis.EvmApi.resolve.resolveAddress(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'resolveAddress'));
  }
})

Parse.Cloud.define("getPairReserves", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getPairReserves');
    const result = await Moralis.EvmApi.defi.getPairReserves(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getPairReserves'));
  }
})

Parse.Cloud.define("getPairAddress", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'getPairAddress');
    const result = await Moralis.EvmApi.defi.getPairAddress(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'getPairAddress'));
  }
})

Parse.Cloud.define("uploadFolder", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'uploadFolder');
    const result = await Moralis.EvmApi.storage.uploadFolder(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'uploadFolder'));
  }
})

Parse.Cloud.define("web3ApiVersion", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'web3ApiVersion');
    //@ts-ignore
    const result = await Moralis.EvmApi.info.web3ApiVersion(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'web3ApiVersion'));
  }
})

Parse.Cloud.define("endpointWeights", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, 'endpointWeights');
    //@ts-ignore
    const result = await Moralis.EvmApi.info.endpointWeights(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, 'endpointWeights'));
  }
})

