const functions = require("firebase-functions");
const Moralis = require("moralis").default;

exports.newPunkTransfer = functions.firestore
    .document(`moralis/events/Cryptopunktransferred/{id}`)
    .onCreate(async (snap) => {
    
        const tokenId = snap.data().punkIndex;

        await Moralis.start({
            apiKey: 'get from Moralis Admin Dashboard',
        });

        const response = await Moralis.EvmApi.nft.getNFTTokenIdOwners({
            address:"0xb47e3cd837dDF8e4c57F05d70Ab865de6e193BBB",
            tokenId:tokenId,
        });

        const metaData = JSON.parse(response.raw.result[0].metadata);

        return snap.ref.set({
            image: metaData.image
          }, {merge: true});


    });

