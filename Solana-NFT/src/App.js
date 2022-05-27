import React, { useState } from "react";
import { useMoralisSolanaApi } from "react-moralis";
import "./App.css";
import bg from "./bg.png";
import logo from "./logo.png";
import moralis from "./moralis.png";

const App = () => {
  const [address, setAddress] = useState();
  const [name, setName] = useState();
  const [image, setImage] = useState();
  const SolanaApi = useMoralisSolanaApi();

  async function NFTsearch(address) {
    const options = {
      network: "mainnet",
      address: address,
    };

    const nftResult = await SolanaApi.nft.getNFTMetadata(options);
    let uri = nftResult.metaplex.metadataUri;
    setName(nftResult.name);

    try {
      await fetch(uri)
        .then((response) => response.json())
        .then((data) => {
          setImage(data.image);
        });
    } catch {
      console.log("couldnt get image");
    }
  }

  return (
    <>
      <div
        style={{
          backgroundImage: `url(${bg})`,
          backgroundSize: "cover",
          height: "100vh",
        }}
      >
        <div className="header">
          <img src={logo} alt="nft" style={{ height: "40px" }} />
        </div>
        <div className="content">
          <input
            style={{width: "350px", height: "20px"}}
            type="text"
            onChange={(e) => setAddress(e.target.value)}
          ></input>
          <div className="search" onClick={() => NFTsearch(address)}>Get NFT</div>
          {image && <img src={image} alt="nft" />}
          {name && <div className="name">{name}</div>}
        </div>
        <img className="pwr" src={moralis} alt="powered"/>
      </div>
    </>
  );
};

export default App;
