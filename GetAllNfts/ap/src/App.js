import axios from "axios";
import "./App.css";
import { useState } from "react";
import logo from "./logo.svg";
import moralis from "./moralis.png";

function App() {
  const [address, setAddress] = useState("");
  const [chain, setChain] = useState("0x1");
  const [cursor, setCursor] = useState(null);
  const [NFTs, setNFTs] = useState([]);

  function getImgUrl(metadata) {
    if (!metadata) return logo;

    let meta = JSON.parse(metadata);

    if (!meta.image) return logo;

    if (!meta.image.includes("ipfs://")) {
      return meta.image;
    } else {
      return "https://ipfs.io/ipfs/" + meta.image.substring(7);
    }
  }

  async function fetchNFTs() {
    let res;
    if (cursor) {
      res = await axios.get(`http://localhost:3000/allNft`, {
        params: { address: address, chain: chain, cursor: cursor },
      });
    } else {
      res = await axios.get(`http://localhost:3000/allNft`, {
        params: { address: address, chain: chain },
      });
    }

    console.log(res);

    let n = NFTs;
    setNFTs(n.concat(res.data.result.result));
    setCursor(res.data.result.cursor);
    console.log(res);
  }

  function addressChange(e) {
    setAddress(e.target.value);
    setCursor(null);
    setNFTs([]);
  }

  function chainChange(e) {
    setChain(e.target.value);
    setCursor(null);
    setNFTs([]);
  }

  return (
    <>
      <img src={moralis} alt="moralis" className="moralis" />
      <div className="App">
        <div style={{ fontSize: "23px", fontWeight: "700" }}>
          Get NFTs by contract
        </div>
        <button className="bu" onClick={fetchNFTs}>
          Get NFT's
        </button>
        <div className="inputs">
          <div style={{ display: "flex" }}>
            <div style={{ width: "80px" }}>Contract:</div>
            <input
              className="input"
              value={address}
              onChange={(e) => addressChange(e)}
            ></input>
          </div>
          <div style={{ display: "flex" }}>
            <div style={{ width: "80px" }}>Chain:</div>
            <select className="input" onChange={(e) => chainChange(e)}>
              <option value="0x1">Ethereum</option>
              <option value="0x38">Bsc</option>
              <option value="0x89">Polygon</option>
              <option value="0xa86a">Avalanche</option>
            </select>
          </div>
        </div>
        {NFTs.length > 0 && (
          <>
            <div className="results">
              {NFTs?.map((e, i) => {
                return (
                  <>
                    <div style={{ width: "70px" }}>
                      <img
                        loading="lazy"
                        width={70}
                        src={getImgUrl(e.metadata)}
                        alt={`${i}image`}
                        style={{ borderRadius: "5px", marginTop: "10px" }}
                      />
                      <div key={i} style={{ fontSize: "10px" }}>
                        {`${e.name}\n${e.token_id}`}
                      </div>
                    </div>
                  </>
                );
              })}
            </div>
            {cursor && (
              <>
                <button className="bu" onClick={fetchNFTs}>
                  Load More
                </button>
              </>
            )}
          </>
        )}
      </div>
    </>
  );
}

export default App;
