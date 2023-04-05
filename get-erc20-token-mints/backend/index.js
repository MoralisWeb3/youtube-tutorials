import express from "express";
const app = express();
const port = 5001;
import Moralis from "moralis";
import cors from "cors";
import dotenv from "dotenv";
dotenv.config();

app.use(cors());
app.use(express.json());

const MORALIS_API_KEY = process.env.MORALIS_API_KEY;

app.get("/getmints", async (req, res) => {
  const { query } = req;

  try {
    const erc20mints = await Moralis.EvmApi.token.getErc20Mints({
      chain: query.chain,
      limit: 20,
      contractAddresses: [],
      excludeContracts: [],
      walletAddresses: [],
      excludeWallets: [],
    });

    let moreData = [];

    for (let i = 0; i < erc20mints.result.length; i++) {
      const response = await Moralis.EvmApi.token.getTokenMetadata({
        chain: query.chain,
        addresses: [erc20mints.result[i].contractAddress._value],
      });

      moreData.push({
        finalResponse: {
          response: erc20mints.result[i],
          tokenData: {
            name: response.jsonResponse[0].name,
            symbol: response.jsonResponse[0].symbol,
            decimals: response.jsonResponse[0].decimals,
          },
        },
      });
    }

    const response = moreData;
    return res.status(200).json(response);
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
