const express = require("express");
const Moralis = require("moralis").default;
const app = express();
const cors = require("cors");
const port = 3000;

app.use(cors());
app.use(express.json());

app.get("/allNft", async (req, res) => {
  try {
    const { query } = req;

    let NFTs;

    if (query.cursor) {
      NFTs = await Moralis.EvmApi.nft.getContractNFTs({
        address: query.address,
        chain: query.chain,
        cursor: query.cursor,
        limit: 10,
      });
    } else {
      NFTs = await Moralis.EvmApi.nft.getContractNFTs({
        address: query.address,
        chain: query.chain,
        limit: 10,
      });
    }

    const result = NFTs.raw;

    return res.status(200).json({ result });
  } catch (e) {

    console.log(e);
    console.log("something went wrong");
    return res.status(400).json();

  }
});


Moralis.start({
  apiKey: "xxxx",
}).then(() => {
  app.listen(port, () => {
    console.log(`Listening for API Calls`);
  });
});
