const { id } = require("@ethersproject/hash");

const Moralis = require("moralis").default;
require("dotenv").config();

const url = "Set you WebHook url here";
var stream;

Moralis.start({
  apiKey: process.env.MORALIS_KEY,
}).then(async () => {
  const streams = await Moralis.Streams.getAll({
    limit: 100,
  });
  stream = streams.jsonResponse.result[0];
  const streamId = stream.id;

  await Moralis.Streams.update({
    id: streamId,
    webhookUrl: url,
    chains: ["0x1"],
  });

  const oldUrl = stream.webhookUrl;
  console.log(`old url: ${oldUrl}`);
  console.log(`new url: ${url}`);
});
