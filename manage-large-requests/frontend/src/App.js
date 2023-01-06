import axios from "axios";
import { useEffect, useState } from "react";
import { Card } from "antd";
import styles from "./styles/Home.module.css";
import Logo from "./img/Moralis_logo.png";
import "./App.css";

function App() {
  const [nftData, setNftData] = useState(null);
  const { Meta } = Card;

  const refreshNFTS = async () => {
    await axios
      .get(
        `/get_nfts/?address=${params.address}&chain=${params.chain}&cursor=${params.cursor}`
      )
      .then((res) => {
        setNftData(res.data.result);
        setParams({ ...params, cursor: res.data.cursor });
      })
      .catch((err) => console.log(err));
  };

  const [params, setParams] = useState({
    address: "",
    chain: "eth",
    cursor: "",
  });

  const handleParamsChange = (e) => {
    setParams({ ...params, [e.target.name]: e.target.value });
  };

  const renderedData =
    nftData &&
    Object.values(nftData).map((nft) => {
      return (
        <div className="nfts">
          <Card
            className="horizontal-card result-card"
            hoverable
            cover={<img alt="example" src={nft?.normalized_metadata?.image} />}
          >
            <Meta
              title={nft?.normalized_metadata?.name}
              description={nft?.token_address}
            />
            <p>
              <b>Owner of:</b> {nft?.owner_of}
            </p>
            <p>
              <b>Token id:</b> {nft?.token_id}
            </p>
            <p>
              <b>Block Number:</b> {nft?.block_number}
            </p>
          </Card>
        </div>
      );
    });

  return (
    <div>
      <div className={styles.header}>
        <div className={styles.moralis_logo}>
          <img src={Logo} alt="Logo image" width="102" height="82" />
        </div>
        <h1 className={styles.title}>Get Wallet NFTs </h1>
      </div>
      <section className={styles.main}>
        <div className={styles.getTokenForm}>
          <label className={styles.label} htmlFor="nftAddress">
            Wallet Address
          </label>
          <input
            className={styles.walletAddress}
            type="text"
            id="nftAddress"
            name="address"
            value={params.address}
            onChange={handleParamsChange}
            maxLength="120"
            required
          />
        </div>

        <div className={styles.getTokenForm}>
          <label
            className={styles.label}
            value={params.chain}
            onChange={handleParamsChange}
          >
            Chain
          </label>
          <select className={styles.walletAddress} name="chain">
            <option value="eth">Ethereum</option>
            <option value="goerli">Goerli</option>
            <option value="polygon">Polygon</option>
            <option value="mumbai">Mumbai</option>
            <option value="bsc">Binance</option>
          </select>
        </div>

        <div>
          {!nftData ? (
            <button className={styles.form_btn} onClick={refreshNFTS}>
              Get NFTs
            </button>
          ) : (
            <button className={styles.form_btn} onClick={refreshNFTS}>
              Next Page
            </button>
          )}
        </div>

        <div className="results">
          {nftData && <div className="nfts">{renderedData}</div>}
        </div>
      </section>
    </div>
  );
}

export default App;
