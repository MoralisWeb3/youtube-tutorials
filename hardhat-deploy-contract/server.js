require("dotenv").config();
const express = require("express");
const Moralis = require("moralis").default;
const app = express();
const cors = require("cors");
const port = 3001;

app.use(cors());
app.use(express.json());

app.get("/get_owners", async (req, res) => {
  const contractAddress = req.query.address;

  try {
    const response = await Moralis.EvmApi.nft.getNFTOwners({
      chain: "0xaa36a7",
      format: "decimal",
      normalizeMetadata: true,
      address: contractAddress,
    });

    console.log(response);

    res.json(response.raw);
  } catch (e) {
    console.error(e);
    res.status(500).send("An error occurred while fetching NFT owners.");
  }
});

Moralis.start({
  apiKey: process.env.MORALIS_API_KEY,
}).then(() => {
  app.listen(port, () => {
    console.log(`Listening for API Calls at http://localhost:${port}`);
  });
});
