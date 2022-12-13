"use strict";
const Moralis = require("moralis").default;

const startMoralis = async () => {
  await Moralis.start({
    apiKey: process.env.MORALIS_API_KEY,
  });
};

startMoralis();

module.exports.handler = async (event) => {
  // Get wallet NFTs
  const nfts = await Moralis.EvmApi.nft.getWalletNFTs({
    address: event.address,
    chain: event.chain,
    limit: 10,
  });

  return {
    result: nfts,
  };
};
