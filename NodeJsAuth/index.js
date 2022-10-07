const express = require("express");
const Moralis = require("moralis").default;

const app = express();
const port = 3000;

const MORALIS_API_KEY = "replace_me";
const config = {
  network: "evm",
  domain: "http://localhost:3000",
  statement: "Web3 Login",
  uri: "auth.app",
  timeout: 60,
};

app.get("/challenge", async (req, res) => {
  try {
    const { address, chain } = req.body;

    const message = await Moralis.Auth.requestMessage({
      address,
      chain,
      ...config,
    });

    res.status(200);
    res.json(message);
  } catch (error) {
    // Handle errors
    console.error(error);
    res.status(500);
    res.json({ error: error.message });
  }
});

app.get("/verify", async (req, res) => {
  try {
    const { message, signature } = req.body;

    const { profileId } = (
      await Moralis.Auth.verify({ message, signature, network: "evm" })
    ).raw;

    res.status(200);
    res.json(profileId);
  } catch (error) {
    // Handle errors
    console.error(error);
    res.status(500);
    res.json({ error: error.message });
  }
});

const startServer = async () => {
  await Moralis.start({
    apiKey: MORALIS_API_KEY,
  });

  app.listen(port, () => {
    console.log(`Example app listening on port ${port}`);
  });
};

startServer();
