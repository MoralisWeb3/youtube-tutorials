const Moralis = require("moralis").default;
const express = require("express");
const cors = require("cors");
require("dotenv").config();

const app = express();
const port = 8080;

app.use(cors());

app.listen(port, () => {
  console.log(`Example app listening on port ${port}`);
});

//GET AMOUNT AND VALUE OF NATIVE TOKENS

app.get("/nativeBalance", async (req, res) => {
  await Moralis.start({ apiKey: process.env.MORALIS_API_KEY });

  try {
    const { address, chain } = req.query;

    const response = await Moralis.EvmApi.balance.getNativeBalance({
      address: address,
      chain: chain,
    });

    const nativeBalance = response.data;

    let nativeCurrency;
    if (chain === "0x1") {
      nativeCurrency = "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2";
    } else if (chain === "0x89") {
      nativeCurrency = "0x0d500B1d8E8eF31E21C99d1Db9A6444d3ADf1270";
    }

    const nativePrice = await Moralis.EvmApi.token.getTokenPrice({
      address: nativeCurrency, //WETH Contract
      chain: chain,
    });

    nativeBalance.usd = nativePrice.data.usdPrice;

    res.send(nativeBalance);
  } catch (e) {
    res.send(e);
  }
});

//GET AMOUNT AND VALUE OF ERC20 TOKENS

app.get("/tokenBalances", async (req, res) => {
  await Moralis.start({ apiKey: process.env.MORALIS_API_KEY });

  try {
    const { address, chain } = req.query;

    const response = await Moralis.EvmApi.token.getWalletTokenBalances({
      address: address,
      chain: chain,
    });

    let tokens = response.data;
    let legitTokens = [];
    for (let i = 0; i < tokens.length; i++) {
      try {
        const priceResponse = await Moralis.EvmApi.token.getTokenPrice({
          address: tokens[i].token_address,
          chain: chain,
        });
        if (priceResponse.data.usdPrice > 0.01) {
          tokens[i].usd = priceResponse.data.usdPrice;
          legitTokens.push(tokens[i]);
        } else {
          console.log("💩 coin");
        }
      } catch (e) {
        console.log(e);
      }
    }


    res.send(legitTokens);
  } catch (e) {
    res.send(e);
  }
});

//GET Users NFT's

app.get("/nftBalance", async (req, res) => {
  await Moralis.start({ apiKey: process.env.MORALIS_API_KEY });

  try {
    const { address, chain } = req.query;

    const response = await Moralis.EvmApi.nft.getWalletNFTs({
      address: address,
      chain: chain,
    });

    const userNFTs = response.data;

    res.send(userNFTs);
  } catch (e) {
    res.send(e);
  }
});

//GET USERS TOKEN TRANSFERS

app.get("/tokenTransfers", async (req, res) => {
  await Moralis.start({ apiKey: process.env.MORALIS_API_KEY });

  try {
    const { address, chain } = req.query;

    const response = await Moralis.EvmApi.token.getWalletTokenTransfers({
      address: address,
      chain: chain,
    });
    
    const userTrans = response.data.result;

    let userTransDetails = [];
    
    for (let i = 0; i < userTrans.length; i++) {
      
      try {
        const metaResponse = await Moralis.EvmApi.token.getTokenMetadata({
          addresses: [userTrans[i].address],
          chain: chain,
        });
        if (metaResponse.data) {
          userTrans[i].decimals = metaResponse.data[0].decimals;
          userTrans[i].symbol = metaResponse.data[0].symbol;
          userTransDetails.push(userTrans[i]);
        } else {
          console.log("no details for coin");
        }
      } catch (e) {
        console.log(e);
      }

    }



    res.send(userTransDetails);
  } catch (e) {
    res.send(e);
  }
});
