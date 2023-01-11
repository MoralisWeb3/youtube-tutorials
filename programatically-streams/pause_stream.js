const Moralis = require("moralis").default;
require("dotenv").config();

const status = "active";
var stream;

Moralis.start({
  apiKey: process.env.MORALIS_KEY,
}).then(async () => {
  const streams = await Moralis.Streams.getAll({
    limit: 100,
  });
  stream = streams.jsonResponse.result[0];
  await Moralis.Streams.updateStatus({
    id: stream.id,
    status: status,
  });
});
