import { BigNumber } from "@moralisweb3/core";
import Moralis from "moralis";
const notifier = require("node-notifier");

const express = require("express");

const app = express();
const port = 3000;

app.use(express.json());

interface ProposalAdded {
  proposalId: BigNumber;
}

app.post("/webhook", (req, res) => {
  const webhook = req.body;
  const decodedLogs = Moralis.Streams.parsedLogs<ProposalAdded>(webhook);

  notifier.notify({
    title: "New Proposal Launched",
    message: `Proposal ID: ${decodedLogs[0].proposalId.toString()}`,
  });

  return res.status(200).json();
});

app.listen(port, () => {
  console.log(`Listening to streams`);
});
