const express = require("express");
const app = express();
const port = 5001;
const Moralis = require("moralis").default;
const cors = require("cors");

require("dotenv").config({ path: ".env" });

app.use(cors());
app.use(express.json());

const MORALIS_API_KEY = process.env.MORALIS_API_KEY;

app.get("/gettokens", async (req, res) => {
  try {
    let modifiedResponse = [];
    let totalWalletUsdValue = 0;
    const { query } = req;

    const response = await Moralis.EvmApi.token.getWalletTokenBalances({
      address: query.address,
      chain: "0x1",
    });

    for (let i = 0; i < response.toJSON().length; i++) {
      const tokenPriceResponse = await Moralis.EvmApi.token.getTokenPrice({
        address: response.toJSON()[i].token_address,
        chain: "0x1",
      });
      modifiedResponse.push({
        walletBalance: response.toJSON()[i],
        calculatedBalance: (
          response.toJSON()[i].balance /
          10 ** response.toJSON()[i].decimals
        ).toFixed(2),
        usdPrice: tokenPriceResponse.toJSON().usdPrice,
      });
      totalWalletUsdValue +=
        (response.toJSON()[i].balance / 10 ** response.toJSON()[i].decimals) *
        tokenPriceResponse.toJSON().usdPrice;
    }

    modifiedResponse.push(totalWalletUsdValue);

    return res.status(200).json(modifiedResponse);
  } catch (e) {
    console.log(`Something went wrong ${e}`);
    return res.status(400).json();
  }
});

Moralis.start({
  apiKey: MORALIS_API_KEY,
}).then(() => {
  app.listen(port, () => {
    console.log(`Listening for API Calls`);
  });
});
