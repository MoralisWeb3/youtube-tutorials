import cors from "cors";
import fetch from "node-fetch";
import express from "express";
const app = express();
const port = 5001;

app.use(cors());
app.use(express.json());

const options = {
  method: "GET",
  headers: {
    accept: "application/json",
    "X-API-Key": "YOUR-API-KEY-HERE",
  },
};

app.get("/gettestnet", async (req, res) => {
  const { query } = req;

  try {
    fetch(
      `https://testnet-aptos-api.moralis.io/nfts/creators?limit=10&offset=0&creator_addresses=${query.creatorAddress}`,
      options
    )
      .then((response) => response.json())
      .then((response) => {
        return res.status(200).json(response.result);
      });
  } catch (e) {
    console.log("Something went wrong", e);
    return res.status(400).json();
  }
});

app.get("/getmainnet", async (req, res) => {
  const { query } = req;

  try {
    fetch(
      `https://mainnet-aptos-api.moralis.io/nfts/creators?limit=6&creator_addresses%5B0%5D=${query.creatorAddress}`,
      options
    )
      .then((response) => response.json())
      .then((response) => {
        return res.status(200).json(response.result);
      });
  } catch (e) {
    console.log("Something went wrong", e);
    return res.status(400).json();
  }
});

app.listen(port, () => {
  console.log(`Listening for API Calls`);
});
