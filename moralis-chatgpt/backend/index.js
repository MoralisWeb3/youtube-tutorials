const express = require("express");
const app = express();
const port = 5001;
const Moralis = require("moralis").default;
const cors = require("cors");
require("dotenv").config();

app.use(cors());
app.use(express.json());

const { Configuration, OpenAIApi } = require("openai");
const configuration = new Configuration({
  apiKey: process.env.OPENAI_API_KEY,
});
const openai = new OpenAIApi(configuration);

app.get("/", async (req, res) => {
  const { query } = req;
  console.log("qq", query.message);
  try {
    const response = await openai.createCompletion({
      model: "text-davinci-003",
      prompt: query.message,
      max_tokens: 30,
      temperature: 0,
    });
    console.log("before return", response.data);
    return res.status(200).json(response.data);
  } catch (e) {
    console.log(`Something went wrong ${e}`);
    return res.status(400).json();
  }
});

app.listen(port, () => {
  console.log(`Listening on port ${port}`);
});
