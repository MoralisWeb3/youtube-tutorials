

const Moralis = require("moralis").default;
require("dotenv").config();

var stream;

const abi = {
    "constant": true,
    "inputs": [{ "name": "who", "type": "address" }],
    "name": "balanceOf",
    "outputs": [{ "name": "", "type": "uint256" }],
    "payable": false,
    "stateMutability": "view",
    "type": "function"
};

const trigger = {
    contractAddress: "$contract",
    functionAbi: abi,
    inputs: ["$from"],
    type: "erc20transfer",

}

const triggers = [trigger];

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
        chains: ["0x1"],
        triggers: triggers,
    });


});