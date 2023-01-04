const express = require("express");
const app = express();
const dotenv = require("dotenv");
const web3 = require("web3");
const port = 3000;

app.use(express.json());
dotenv.config();
const apiKey = process.env.MORALIS_API_KEY;

app.post("/webhook", (req, res) => {
  verifySignature(req, apiKey);
  const webhook = req.body;
  console.log(webhook);
  return res.status(200).json();
});

app.listen(port, () => {
  console.log(`Listening to streams`);
});

const verifySignature = (req, secret) => {
  const providedSignature = req.headers["x-signature"];
  if (!providedSignature) throw new Error("Signature not provided");
  const generatedSignature = web3.utils.sha3(JSON.stringify(req.body) + secret);
  if (generatedSignature !== providedSignature)
    throw new Error("Invalid Signature");
};
