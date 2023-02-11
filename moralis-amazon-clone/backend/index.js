const express = require("express");
const app = express();
const port = 5001;
const Moralis = require("moralis").default;
const cors = require("cors");

require("dotenv").config();

app.use(cors());
app.use(express.json());

const MORALIS_API_KEY = process.env.MORALIS_API;

app.get("/ethtoken", async (req, res) => {
  const address = "0xc02aaa39b223fe8d0a0e5c4f27ead9083c756cc2";
  const chain = "0x1";

  try {
    const response = await Moralis.EvmApi.token.getTokenPrice({
      address,
      chain,
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
    console.log(`Listening for API Calls on port ${port}`);
  });
});
