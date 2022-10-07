const functions = require("firebase-functions");
const Moralis = require("moralis").default;
const { EvmChain } = require("@moralisweb3/evm-utils");

exports.getPrice = functions.https.onRequest(async (req, res) => {
    
    await Moralis.start(
        {
            apiKey: "Moralis API KEY"
        }
    );

    
    const address = req.query.address;


    const response = await Moralis.EvmApi.token.getTokenPrice({
        address: address,
        chain: EvmChain.ETHEREUM
    });

    const usd = response.raw.usdPrice;

    res.json({usd});

});
