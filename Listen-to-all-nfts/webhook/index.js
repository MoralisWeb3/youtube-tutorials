const express = require("express");
const Moralis = require("moralis").default;
const app = express();
const port = 3000;
require("dotenv").config();
/* let blocknum = 0; */

app.use(express.json());

app.post("/webhook", async (req, res) => {
  const { headers, body } = req;

  try {
    Moralis.Streams.verifySignature({
      body,
      signature: headers["x-signature"],
    });

/* 
    if(body.block.number && (Number(body.block.number) > blocknum)){
        blocknum = Number(body.block.number);
      }else{
        return res.status(200).json();
      } 

    console.log("\n**** Block Number " + body.block.number + " ****\n");

    for (nftTransfer of body.nftTransfers) {
      
        console.log(
            nftTransfer.to.slice(0, 4) +
              "..." +
              nftTransfer.to.slice(38) +
              " just received " +
              nftTransfer.tokenName +
              " (" +
              nftTransfer.tokenId +
              ")"
          );

    } */


    console.log(body);
    
    return res.status(200).json();
  } catch (e) {
    console.log("Not Moralis");
    return res.status(400).json();
  }
});

Moralis.start({
  apiKey: "xxx",
}).then(() => {
  app.listen(port, () => {
    console.log(`Listening to streams`);
  });
});
