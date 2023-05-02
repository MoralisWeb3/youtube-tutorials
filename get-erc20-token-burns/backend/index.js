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

app.get("/getburns", async (req, res) => {
  try {
    const { query } = req;
    const response = await Moralis.EvmApi.token.getErc20Burns({
      chain: query.chain,
      limit: 15,
      walletAddresses: ["0x0000000000000000000000000000000000000000"],
    });

    return res.status(200).json(response);
  } catch (e) {
    console.log(`Somthing went wrong ${e}`);
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
