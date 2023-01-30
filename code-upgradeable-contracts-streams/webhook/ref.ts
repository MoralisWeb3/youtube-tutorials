import { BigNumber } from "@moralisweb3/core";
import Moralis from "moralis";
const notifier = require("node-notifier");

const express = require("express");

const app = express();
const port = 3000;

app.use(express.json());

interface Upgraded {
  implementation: string;
}

app.post("/webhook", (req, res) => {
  const webhook = req.body;

  try {
    const decodedLogs = Moralis.Streams.parsedLogs<Upgraded>(webhook);
    notifier.notify({
      title: "Box Contract Has been Upgraded",
      message: `New Version Address: ${decodedLogs[0].implementation}`,
    });
  } catch (e) {
    return res.status(200).json();
  }

  return res.status(200).json();
});

app.listen(port, () => {
  console.log(`Listening to streams`);
});
