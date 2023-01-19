const express = require("express");
const app = express();
const port = 5001;
const Moralis = require("moralis").default;
const cors = require("cors");

require("dotenv").config({ path: ".env" });

app.use(cors());
app.use(express.json());

const MORALIS_API_KEY = process.env.MORALIS_API_KEY;

app.get("/getwalletbalance", async (req, res) => {
  try {
    const { query } = req;

    const response = await Moralis.EvmApi.token.getWalletTokenBalances({
      address: query.address,
      chain: query.chain,
    });

    return res.status(200).json(response);
  } catch (e) {
    console.log(`Something went wrong ${e}`);
    return res.status(400).json();
  }
});

app.get("/getwalletnft", async (req, res) => {
  try {
    const { query } = req;

    const response = await Moralis.EvmApi.nft.getWalletNFTs({
      address: query.address,
      chain: query.chain,
    });

    return res.status(200).json(response);
  } catch (e) {
    console.log(`Something went wrong ${e}`);
    return res.status(400).json();
  }
});

app.get("/getwallettransactions", async (req, res) => {
  try {
    const { query } = req;

    const response = await Moralis.EvmApi.token.getWalletTokenTransfers({
      address: query.address,
      chain: query.chain,
    });

    return res.status(200).json(response);
  } catch (e) {
    console.log(`Something went wrong ${e}`);
    return res.status(400).json();
  }
});

app.get("/getstreams", async (req, res) => {
  try {
    const streams = await Moralis.Streams.getAll({
      limit: 100,
    });

    return res.status(200).json(streams);
  } catch (e) {
    console.log(`Something went wrong ${e}`);
    return res.status(400).json();
  }
});

app.get("/createstream", async (req, res) => {
  try {
    const { query } = req;
    const address = query.address;

    const stream = {
      chains: ["0x5"],
      description: `Monitor ${address}`,
      tag: address,
      webhookUrl:
        "https://us-central1-automatedweb3tracker.cloudfunctions.net/ext-moralis-streams-webhook", // webhook url to receive events,
      includeNativeTxs: true,
    };

    const newStream = await Moralis.Streams.add(stream);
    const { id } = newStream.toJSON();

    await Moralis.Streams.addAddress({ address, id });
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
