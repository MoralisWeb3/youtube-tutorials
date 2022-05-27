import React, { useState } from "react";
import { useMoralisSolanaApi } from "react-moralis";
import "./App.css";

const App = () => {

  const [address, setAddress] = useState();
  const [name, setName] = useState();
  const [image, setImage] = useState();
  const SolanaApi = useMoralisSolanaApi();

  async function NFTsearch(address){

    const options = {
      network: "mainnet",
      address: address,
    };

    const nftResult = await SolanaApi.nft.getNFTMetadata(options);
    let uri = nftResult.metaplex.metadataUri;
    setName(nftResult.name)

    try {
      await fetch(uri)
        .then((response) => response.json())
        .then((data) => {
          setImage(data.image)
        });
    }catch{
      console.log("couldnt get image")
    }

  }

  return (
    <>
     <div>Solana NFT</div>
     <input type="text" onChange={(e)=>setAddress(e.target.value)}></input>
     <button onClick={()=> NFTsearch(address)}>Get NFTs</button>
     {image && <img src={image} alt="nft" width="200px" height="200px"/>}
     {name && <div>{name}</div>}
    </>
  );
};

export default App;
