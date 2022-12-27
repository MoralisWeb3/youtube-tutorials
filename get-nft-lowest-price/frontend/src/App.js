import axios from "axios";
import { useState } from "react";
import { Card } from "antd";
import styles from "./styles/Home.module.css";
import Logo from "./img/Moralis_logo.png";
import "./App.css";

function App() {
  const [nftPrice, setNftPrice] = useState(null);

  const refreshNftPrice = async () => {
    await axios
      .get(`/get_lowest?address=${params.address}&chain=${params.chain}`)
      .then((res) => {
        setNftPrice(res.data);
      })
      .catch((err) => console.log(err));
  };

  const [params, setParams] = useState({
    address: "",
    chain: "eth",
  });

  const handleParamsChange = (e) => {
    setParams({ ...params, [e.target.name]: e.target.value });
  };

  const renderedPrice = nftPrice && (
    <Card
      hoverable
      className="result-card"
      title={nftPrice.marketplace_address}
      extra={
        <p>
          <b>Token Id:</b> {nftPrice.token_ids[0]}
        </p>
      }
      style={{ width: 300 }}
    >
      <div className="result-element">
        <p className="result-begin">Seller:</p>
        <p className="result-end">{nftPrice.seller_address}</p>
      </div>
      <div className="result-element">
        <p className="result-begin">Buyer:</p>
        <p className="result-end">{nftPrice.buyer_address}</p>
      </div>
      <p className="result-begin" style={{ fontSize: 16 }}>
        Transaction Hash:
      </p>
      <div className="result-element">
        <p className="result-end">{nftPrice.transaction_hash}</p>
      </div>
    </Card>
  );

  return (
    <div>
      <div className={styles.header}>
        <div className={styles.moralis_logo}>
          <img src={Logo} alt="Logo image" width="102" height="82" />
        </div>
        <h1 className={styles.title}>Get NFT Transfers by Block </h1>
      </div>
      <section className={styles.main}>
        <div className={styles.getTokenForm}>
          <label className={styles.label} htmlFor="nftAddress">
            Contract Address
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

        <button className={styles.form_btn} onClick={refreshNftPrice}>
          Get Lowest Price
        </button>
        <div className="results">
          {nftPrice && <div className="nfts">{renderedPrice}</div>}
        </div>
      </section>
    </div>
  );
}

export default App;
