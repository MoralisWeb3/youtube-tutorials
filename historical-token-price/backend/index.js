import express from "express";
import Moralis from "moralis";
import cors from "cors";
import dotenv from "dotenv";
const app = express();
const port = 5001;
dotenv.config();

app.use(cors());
app.use(express.json());

const MORALIS_API_KEY = process.env.MORALIS_API_KEY;

app.get("/getdateblock", async (req, res) => {
  const { query } = req;
  const blockNumbers = [];

  try {
    for (let i = 0; i < query.dates.length; i++) {
      const response = await Moralis.EvmApi.block.getDateToBlock({
        chain: "0x1",
        date: Number(query.dates[i]),
      });

      blockNumbers.push(response.toJSON().block);
    }

    const response = { blockNumbers };

    return res.status(200).json(response);
  } catch (e) {
    console.log(`Something went wrong ${e}`);
    return res.status(400).json();
  }
});

app.get("/gettokenprice", async (req, res) => {
  const { query } = req;
  const tokenPrices = [];

  try {
    for (let i = 0; i < query.blockNumbers.length; i++) {
      const response = await Moralis.EvmApi.token.getTokenPrice({
        chain: "0x1",
        exchange: "uniswap-v2",
        address: query.address,
        toBlock: query.blockNumbers[i],
      });

      tokenPrices.push(response.toJSON().usdPrice);
    }

    const response = { tokenPrices };

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
