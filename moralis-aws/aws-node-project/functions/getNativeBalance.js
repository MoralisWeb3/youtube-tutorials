"use strict";
const Moralis = require("moralis").default;

const startMoralis = async () => {
  await Moralis.start({
    apiKey: process.env.MORALIS_API_KEY,
  });
};

startMoralis();

module.exports.handler = async (event) => {
  // Get native balance
  const nativeBalance = await Moralis.EvmApi.balance.getNativeBalance({
    address: event.address,
    chain: event.chain,
  });

  // Format the native balance formatted in ether via the .ether getter
  const nativeBalanceEther = nativeBalance.result.balance.ether;

  return {
    result: nativeBalanceEther,
  };
};
