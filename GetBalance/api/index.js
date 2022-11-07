const express = require("express");
const Moralis = require("moralis").default;
const app = express();
const cors = require("cors");
const port = 3000;

app.use(cors());
app.use(express.json());

app.get("/balance", async (req, res) => {
  try {
    const { query } = req;
    let balance;

    if (query.toBlock) {
      balance = await Moralis.EvmApi.balance.getNativeBalance({
        address: query.address,
        chain: query.chain,
        toBlock: query.toBlock
      });
    }else{
      balance = await Moralis.EvmApi.balance.getNativeBalance({
        address: query.address,
        chain: query.chain,
      });
    }

    const result = balance.raw;

    return res.status(200).json({ result });
  } catch (e) {
    console.log(e);
    console.log("something went wrong");
    return res.status(400).json();
  }
});

Moralis.start({
  apiKey: "xxx",
}).then(() => {
  app.listen(port, () => {
    console.log(`Listening for API Calls`);
  });
});
