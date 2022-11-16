/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/no-var-requires */
declare const Parse: any;
import './generated/evmApi';
import './generated/solApi';
import { requestMessage } from '../auth/authService';
import Moralis from 'moralis';

Parse.Cloud.define('requestMessage', async ({ params }: any) => {
  const { address, chain, networkType } = params;

  const message = await requestMessage({
    address,
    chain,
    networkType,
  });

  return { message };
});

Parse.Cloud.define('getPluginSpecs', () => {
  // Not implemented, only excists to remove client-side errors when using the moralis-v1 package
  return [];
});

Parse.Cloud.define('getServerTime', () => {
  // Not implemented, only excists to remove client-side errors when using the moralis-v1 package
  return null;
});

Parse.Cloud.define('secretCloudCode', async () =>{


  function getRandomInt() {
    return Math.floor(Math.random() * 2);
  }

  const tokens = [
    {
      token: "ETH",
      contract: "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2" //Wrapped
    },
    {
      token:"BTC",
      contract:"0x2260FAC5E5542a773Aa44fBCfeDf7C193bc2C599" //Wrapped
    }
  ]

  let suggestion = tokens[getRandomInt()]

  const res = await Moralis.EvmApi.token.getTokenPrice({
    address: suggestion.contract
  })

  let priceUsd = res.raw.usdPrice

  return({
    token: suggestion.token,
    price: priceUsd
  })
})