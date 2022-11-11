import axios from "axios";
import "./App.css";
import { useState } from "react";
import moralis from "./moralis.png";

function App() {
  const [address, setAddress] = useState("");
  const [chain, setChain] = useState("0x1");
  const [txs, setTxs] = useState(null);

  async function fetchTxs() {
    let res = await axios.get(`http://localhost:3000/txs`, {
      params: { address: address, chain: chain },
    });

    console.log(res);

    setTxs(res.data.result);
  }

  function addressChange(e) {
    setAddress(e.target.value);
    setTxs(null);
  }

  function chainChange(e) {
    setChain(e.target.value);
    setTxs(null);
  }

  return (
    <>
      <img src={moralis} alt="moralis" className="moralis" />
      <div className="App">
        <div style={{ fontSize: "23px", fontWeight: "700" }}>
          Get Wallet Native Transactions
        </div>
        <button className="bu" onClick={fetchTxs}>
          Get Txs
        </button>
        <div className="inputs">
          <div style={{ display: "flex" }}>
            <div style={{ width: "80px" }}>Wallet:</div>

            {/****  WALLET INPUT ****/}
            <input
              className="input"
              value={address}
              onChange={(e) => addressChange(e)}
            ></input>
          </div>
          <div style={{ display: "flex" }}>
            <div style={{ width: "80px" }}>Chain:</div>

            {/**** CHAIN SELECTION ****/}
            <select className="input" onChange={(e) => chainChange(e)}>
              <option value="0x1">Ethereum</option>
              <option value="0x13881">Mumbai</option>
            </select>
          </div>
        </div>

        {/**** Results ****/}

        {txs && (
          <div className="results">
            {txs.result?.map((e, i) => {
              return (
              <a href={`https://mumbai.polygonscan.com/tx/${e.hash}`} key={i} className="tx" target="_blank">
                [{e.block_timestamp.slice(0,10)}]
                {e.from_address.toLowerCase() === address.toLowerCase() ?
                <span> Send {(e.value / 1E18).toFixed(2)} MATIC</span>  :
                <span> Receive {(e.value / 1E18).toFixed(2)} MATIC</span>
              }
              </a>
              )
            })}
          </div>
        )}
      </div>
    </>
  );
}

export default App;
