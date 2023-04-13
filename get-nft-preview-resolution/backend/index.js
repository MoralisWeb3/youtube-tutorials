import fetch from "node-fetch";
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

const options = {
  method: "GET",
  headers: {
    accept: "application/json",
    "X-API-Key": MORALIS_API_KEY,
  },
};

app.get("/getpreview", async (req, res) => {
  try {
    fetch(
      "https://deep-index.moralis.io/api/v2/0x738b928AF015bDD2Ed3aa3755D0ecf42dB7868C0/nft?chain=eth&format=decimal&disable_total=true&normalizeMetadata=false&media_items=true",
      options
    )
      .then((response) => response.json())
      .then((response) => {
        return res.status(200).json(response);
      });
  } catch (e) {
    console.log(`Something went wrong ${e}`);
    return res.status(400).json();
  }
});

app.get("/getmetadata", async (req, res) => {
  try {
    const response = await Moralis.EvmApi.nft.getWalletNFTs({
      chain: "0x1",
      format: "decimal",
      tokenAddresses: [],
      address: "0x738b928AF015bDD2Ed3aa3755D0ecf42dB7868C0",
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
