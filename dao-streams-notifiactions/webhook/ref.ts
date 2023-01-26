const express = require("express");
const notifier = require("node-notifier");
const app = express();
const port = 3000;
import { BigNumber } from "@moralisweb3/core";
import Moralis from "moralis";

app.use(express.json());

app.post("/webhook", (req, res) => {
  const webhook = req.body;

  interface ProposalAdded {
    proposalId: BigNumber;
  }

  const decodedLogs = Moralis.Streams.parsedLogs<ProposalAdded>(webhook);

  notifier.notify({
    title: "NEW PROPOSAL LAUNCHED",
    message: ` New Proposal Launched: [${decodedLogs[0].proposalId.toString()}]`,
  });
  console.log(
    `New Proposal Launched: [${decodedLogs[0].proposalId.toString()}]`
  );

  return res.status(200).json();
});

app.listen(port, () => {
  console.log(`Listening to streams`);
});
