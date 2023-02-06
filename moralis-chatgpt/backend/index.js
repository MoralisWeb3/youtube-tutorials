const express = require("express");
const app = express();
const port = 5001;
const Moralis = require("moralis").default;
const cors = require("cors");
require("dotenv").config();

app.use(cors());
app.use(express.json());

const MORALIS_API_KEY = process.env.MORALIS_API_KEY;

const { Configuration, OpenAIApi } = require("openai");
const configuration = new Configuration({
  apiKey: process.env.OPENAI_API_KEY,
});
const openai = new OpenAIApi(configuration);

app.get("/", async (req, res) => {
  const { query } = req;
  try {
    const response = await openai.createCompletion({
      model: "text-davinci-003",
      prompt: query.message,
      max_tokens: 30,
      temperature: 0,
    });
    return res.status(200).json(response.data);
  } catch (e) {
    console.log(`Something went wrong ${e}`);
    return res.status(400).json();
  }
});

app.get("/uploadtoipfs", async (req, res) => {
  const { query } = req;
  try {
    const response = await Moralis.EvmApi.ipfs.uploadFolder({
      abi: [
        {
          path: "conversation.json",
          content: { chat: query.pair },
        },
      ],
    });
    console.log(response.result);
    return res.status(200).json(response.result);
  } catch (e) {
    console.log(`Something went wrong ${e}`);
    return res.status(400).json();
  }
});

Moralis.start({
  apiKey: MORALIS_API_KEY,
}).then(() => {
  app.listen(port, () => {
    console.log(`Listening for API Calls`);
  });
});
