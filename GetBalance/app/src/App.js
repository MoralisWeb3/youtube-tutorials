import axios from "axios";
import "./App.css";
import { useState } from "react";
import moralis from "./moralis.png";

function App() {
  const [address, setAddress] = useState("");
  const [chain, setChain] = useState("0x1");
  const [toBlock, setToBlock] = useState("");
  const [balance, setBalance] = useState(null)

  async function fetchBalance() {

    let res;

    if(toBlock){
      res = await axios.get(`http://localhost:3000/balance`, {
        params: { address: address, chain: chain, toBlock: toBlock },
      });
    }else{
      res = await axios.get(`http://localhost:3000/balance`, {
        params: { address: address, chain: chain },
      });
    }

    console.log(res);

    setBalance((res.data.result.balance / 1E18).toFixed(2))
  }

  function addressChange(e) {
    setAddress(e.target.value);
    setBalance(null);
  }

  function chainChange(e) {
    setChain(e.target.value);
    setBalance(null);
  }

  function blockChange(e) {
    setToBlock(e.target.value);
    setBalance(null);
  }

  return (
    <>
      <img src={moralis} alt="moralis" className="moralis" />
      <div className="App">
        <div style={{ fontSize: "23px", fontWeight: "700" }}>
          Get Wallet Native Balance
        </div>
        <button className="bu" onClick={fetchBalance}>
          Get Balance
        </button>
        <div className="inputs">
          <div style={{ display: "flex" }}>
            <div style={{ width: "80px" }}>Wallet:</div>
            <input
              className="input"
              value={address}
              onChange={(e) => addressChange(e)}
            ></input>
           </div>
           <div style={{ display: "flex" }}>
            <div style={{ width: "80px" }}>To Block:</div>
            <input
              className="input"
              value={toBlock}
              type="number"
              onChange={(e) => blockChange(e)}
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
        {balance &&
          <div className="results">
            {balance} 
          </div>
        }
        
      </div>
    </>
  );
}

export default App;
