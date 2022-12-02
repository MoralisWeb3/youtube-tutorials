import "./App.css";
import { useState } from "react";
import { Button, Input, Select, Card } from "antd";
import axios from "axios";

function App() {
  // State Variables
  const [nftsCollections, setNftsCollections] = useState("");

  const refreshNfts = async () => {
    await axios
      .get(
        `/get_collections?chain=${params.chain}&address=${params.address}&limit=${params.limit}`
      )
      .then((res) => setNftsCollections(res.data.result))
      .catch((err) => console.log(err));
  };

  const renderedNFTS =
    nftsCollections &&
    Object.values(nftsCollections).map((collection) => {
      return (
        <Card
          className="result-card"
          title={collection.name}
          extra={
            <p>
              <b>contract type:</b> {collection.contract_type}
            </p>
          }
          style={{ width: 300 }}
        >
          <div className="result-element">
            <p className="result-begin">Token Address:</p>
            <p className="result-end">{collection.token_address}</p>
          </div>
        </Card>
      );
    });

  const [params, setParams] = useState({
    chain: "",
    address: "",
    limit: 10,
  });

  const handleChange = (e) => {
    setParams({ ...params, [e.target.name]: e.target.value });
  };

  return (
    <div className="nft-app">
      <div className="App">
        <h1> Get Any Wallet’s NFT Collections</h1>
      </div>
      <div className="wallet">
        <div className="d-text">Wallet:</div>
        <Input
          className="input"
          size="large"
          value={params.address}
          name="address"
          onChange={handleChange}
        />
      </div>
      <div className="chains">
        <div className="d-text">Chains:</div>
        <Select
          className="input"
          style={{ width: 120 }}
          name="chain"
          value={params.chain}
          onChange={(e) =>
            handleChange({ target: { name: "chain", value: e } })
          }
        >
          <option value="eth">Ethereum</option>
          <option value="goerli">Goerli</option>
          <option value="polygon">Polygon</option>
          <option value="mumbai">Mumbai</option>
          <option value="bsc">Binance</option>
        </Select>
        <div className="chains">
          <div className="d-text">Limit:</div>
          <Input
            style={{ width: 50 }}
            className="input"
            size="large"
            value={params.limit}
            name="limit"
            onChange={handleChange}
          />
        </div>
      </div>
      <div className="bu">
        <Button
          type="primary"
          className="butt"
          shape="round"
          size="large"
          onClick={refreshNfts}
        >
          Get NFT's
        </Button>
      </div>
      <div className="results">
        {nftsCollections && <div className="nfts">{renderedNFTS}</div>}
      </div>
    </div>
  );
}

export default App;
