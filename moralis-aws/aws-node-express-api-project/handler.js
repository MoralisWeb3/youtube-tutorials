const serverless = require("serverless-http");
const express = require("express");
const bodyParser = require("body-parser");
const Moralis = require("moralis").default;

const app = express();

// Accept all type of request body format
app.use(bodyParser.json());
app.use(bodyParser.raw());
app.use(bodyParser.text());
app.use(bodyParser.urlencoded({ extended: true }));

// Start Moralis
const startMoralis = async () => {
  await Moralis.start({
    apiKey: process.env.MORALIS_API_KEY,
  });
};

startMoralis();

app.get("/", (req, res, next) => {
  return res.status(200).json({
    message: "Hello from root!",
  });
});

app.post("/getNativeBalance", async (req, res, next) => {
  try {
    // Get native balance
    const nativeBalance = await Moralis.EvmApi.balance.getNativeBalance({
      address: req.body.address,
      chain: req.body.chain,
    });

    // Format the native balance formatted in ether via the .ether getter
    const nativeBalanceEther = nativeBalance.result.balance.ether;

    res.status(200);
    res.send(nativeBalanceEther);
  } catch (error) {
    // Handle errors
    console.error(error);
    res.status(500);
    res.json({ error: error.message });
  }
});

app.post("/getWalletNfts", async (req, res, next) => {
  try {
    // Get wallet NFTs
    const nfts = await Moralis.EvmApi.nft.getWalletNFTs({
      address: req.body.address,
      chain: req.body.chain,
      limit: 10,
    });

    res.status(200);
    res.json(nfts);
  } catch (error) {
    // Handle errors
    console.error(error);
    res.status(500);
    res.json({ error: error.message });
  }
});

module.exports.handler = serverless(app);
