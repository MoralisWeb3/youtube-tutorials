const express = require("express");
const app = express();
const dotenv = require("dotenv");
const port = 3000;

app.use(express.json());
dotenv.config();

app.post("/webhook", (req, res) => {
  const webhook = req.body;
  console.log(webhook);
  return res.status(200).json();
});

app.listen(port, () => {
  console.log(`Listening to Streams on port ${port}!`);
});
