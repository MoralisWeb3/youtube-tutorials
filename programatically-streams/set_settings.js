const Moralis = require("moralis").default;

require("dotenv").config();
const options = ["us-east-1", "us-west-2", "eu-central-1", "ap-southeast-1"];

Moralis.start({
  apiKey: process.env.MORALIS_KEY,
}).then(async () => {
  await Moralis.Streams.setSettings({
    region: options[2],
  });
  const settingsResponse = await Moralis.Streams.readSettings();
  console.log(settingsResponse);
});
