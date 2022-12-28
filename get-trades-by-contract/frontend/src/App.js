import axios from "axios";
import { useState } from "react";
import { Card } from "antd";
import styles from "./styles/Home.module.css";
import Logo from "./img/Moralis_logo.png";
import "./App.css";

function App() {
  const [nftTrades, setNftTrades] = useState(null);

  const refreshNftTrades = async () => {
    await axios
      .get(
        `/get_trades?address=${params.address}&chain=${params.chain}&limit=${params.limit}`
      )
      .then((res) => {
        setNftTrades(res.data.result);
      })
      .catch((err) => console.log(err));
  };

  const [params, setParams] = useState({
    address: "",
    chain: "eth",
    limit: 10,
  });

  const handleParamsChange = (e) => {
    setParams({ ...params, [e.target.name]: e.target.value });
  };

  const renderedPrice =
    nftTrades &&
    Object.values(nftTrades).map((trades) => {
      return (
        <Card
          hoverable
          className="result-card"
          title={`Block Number: ${trades.block_number}`}
          extra={
            <p>
              <b>Token ID:</b> {trades.token_ids[0]}
            </p>
          }
          style={{ width: 300 }}
        >
          <div className="result-element">
            <p className="result-begin">Price:</p>
            <p className="result-end">{trades.price}</p>
          </div>
          <div className="result-element">
            <p className="result-begin">Seller:</p>
            <p className="result-end">{trades.seller_address}</p>
          </div>
          <div className="result-element">
            <p className="result-begin">Buyer:</p>
            <p className="result-end">{trades.buyer_address}</p>
          </div>
          <p className="result-begin" style={{ fontSize: 16 }}>
            Transaction Hash:
          </p>
          <div className="result-element">
            <p className="result-end">{trades.transaction_hash}</p>
          </div>
        </Card>
      );
    });

  return (
    <div>
      <div className={styles.header}>
        <div className={styles.moralis_logo}>
          <img src={Logo} alt="Logo image" width="102" height="82" />
        </div>
        <h1 className={styles.title}>Get NFT Trades by Marketplace</h1>
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
        <div className={styles.getTokenForm}>
          <label className={styles.label} htmlFor="nftAddress">
            Limit
          </label>
          <input
            className={styles.walletAddress}
            type="text"
            size="large"
            value={params.limit}
            name="limit"
            onChange={handleParamsChange}
          />
        </div>
        <button className={styles.form_btn} onClick={refreshNftTrades}>
          Get trades
        </button>
        <div className="results">
          {nftTrades && <div className="nfts">{renderedPrice}</div>}
        </div>
      </section>
    </div>
  );
}

export default App;
