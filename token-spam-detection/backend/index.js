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

app.get("/gettokens", async (req, res) => {
  const { query } = req;

  try {
    const response = await Moralis.EvmApi.token.getWalletTokenBalances({
      chain: "0x1",
      address: query.address,
    });

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
