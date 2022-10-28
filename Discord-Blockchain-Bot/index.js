const express = require("express");
const Moralis = require("moralis").default;
const discord = require("discord.js");
require("dotenv").config();
const app = express();
const port = 3000;

const client = new discord.Client({
    intents: [],
  });

client.login(process.env.PASS);

app.use(express.json());

app.post("/webhook", async (req, res) => {
  const { body, headers } = req;

  try {
    Moralis.Streams.verifySignature({
      body,
      signature: headers["x-signature"],
    });


    let from = body.txs[0].fromAddress;
    let amount = Number(body.txs[0].value / 1E18);

    const channel = await client.channels.fetch(process.env.CHANNEL);
    channel.send(`New Donation submitted by ${from}, for ${amount.toFixed(2)} MATIC!!!!`);

    return res.status(200).json();
  } catch (e) {
    console.log("Not Moralis");
    return res.status(400).json();
  }
});

Moralis.start({
  apiKey: process.env.APIKEY,
}).then(() => {
  app.listen(port, () => {
    console.log(`Listening to streams`);
  });
});
