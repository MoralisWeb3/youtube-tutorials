import "./App.css";

import { useState } from "react";
import { Button, Input, Select, Card } from "antd";
import axios from "axios";

function App() {
  // State Variables
  const [nftsMetadata, setNftsMetadata] = useState(null);
  const { Meta } = Card;

  const refreshMetadata = async () => {
    await axios
      .get(
        `/get_metadata?chain=${params.chain}&address=${params.address}&token_id=${params.tokenId}`
      )
      .then((res) => setNftsMetadata(res.data))
      .catch((err) => console.log(err));
  };

  const [params, setParams] = useState({
    chain: "",
    address: "",
    tokenId: "",
  });

  const handleChange = (e) => {
    setParams({ ...params, [e.target.name]: e.target.value });
  };

  return (
    <div className="nft-app">
      <div className="App">
        <h1> Get any NFT Metadata</h1>
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
          <div className="d-text">Token id:</div>
          <Input
            style={{ width: 100 }}
            className="input"
            size="large"
            value={params.tokenId}
            name="tokenId"
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
          onClick={refreshMetadata}
        >
          Get NFT's
        </Button>
      </div>
      <div className="results">
        {nftsMetadata && (
          <div className="nfts">
            <Card
              className="horizontal-card"
              hoverable
              cover={
                <img
                  alt="example"
                  src={nftsMetadata?.normalized_metadata.image}
                />
              }
            >
              <Meta
                title={nftsMetadata.normalized_metadata.name}
                description={nftsMetadata.token_address}
              />
              <p>
                <b>Owner of:</b> {nftsMetadata.owner_of}
              </p>
              <p>{nftsMetadata?.normalized_metadata?.attributes[0]?.value}</p>
              <p>{nftsMetadata?.normalized_metadata?.attributes[1]?.value}</p>
              <p>{nftsMetadata?.normalized_metadata?.attributes[2]?.value}</p>
            </Card>
          </div>
        )}
      </div>
    </div>
  );
}

export default App;
