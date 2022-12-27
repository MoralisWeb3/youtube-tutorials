require("dotenv").config();
const express = require("express");
const TelegramBot = require("node-telegram-bot-api");
const TelgramBot = require("node-telegram-bot-api");
const app = express();
const port = 5001;

const TELEGRAM_BOT_TOKEN = process.env.TELEGRAM_BOT_TOKEN;

const bot = new TelegramBot(TELEGRAM_BOT_TOKEN, { polling: true });

app.use(express.json());

app.post("/webhook", async (req, res) => {
  const webhook = req.body;

  for (const nftTransfer of webhook.nftTransfers) {
    const fromAddress = `From address: ${nftTransfer.from.slice(
      0,
      4
    )}...${nftTransfer.from.slice(38)}`;
    const toAddress = `To address: ${nftTransfer.to.slice(
      0,
      4
    )}...${nftTransfer.to.slice(38)}`;
    const tokenItem = `Token Item: ${nftTransfer.tokenName} #${nftTransfer.tokenId}`;
    const transactionHash = `Transaction Hash: ${nftTransfer.transactionHash}`;

    const chatId = -1001698721495;
    const text = `${fromAddress}, ${toAddress}, ${tokenItem}, ${transactionHash}`;

    bot.sendMessage(chatId, text);
  }

  return res.status(200).json();
});

app.listen(port, () => {
  console.log(`Listening for NFT Transfers`);
});
