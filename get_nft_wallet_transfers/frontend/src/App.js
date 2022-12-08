import axios from "axios";
import { useState } from "react";
import { Card } from "antd";
import styles from "./styles/Home.module.css";
import Logo from "./img/Moralis_logo.png";
import "./App.css";

function App() {
  const [nftTransactions, setNftTransactions] = useState(null);

  const refreshNftTransactions = async () => {
    await axios
      .get(
        `/get_transfers?address=${params.address}&chain=${params.chain}&limit=${params.limit}`
      )
      .then((res) => {
        setNftTransactions(res.data.result);
      })
      .catch((err) => console.log(err));
  };

  const [params, setParams] = useState({
    chain: "eth",
    address: "",
    limit: 10,
  });

  const handleParamsChange = (e) => {
    setParams({ ...params, [e.target.name]: e.target.value });
  };

  const renderedTransactions =
    nftTransactions &&
    Object.values(nftTransactions).map((transaction) => {
      return (
        <Card
          hoverable
          className="result-card"
          title={`Token ID: ${transaction.token_id}`}
          extra={
            <p>
              <b>Block Number: {transaction.block_number}</b>
            </p>
          }
          style={{ width: 300 }}
        >
          <div className="result-element">
            <p className="result-begin">From:</p>
            <p className="result-end">{transaction.from_address}</p>
          </div>
          <div className="result-element">
            <p className="result-begin">To:</p>
            <p className="result-end">{transaction.to_address}</p>
          </div>
          <p className="result-begin result-element">Transaction Hash:</p>

          <div className="result-element">
            <p className="result-end">{transaction.transaction_hash}</p>
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
        <h1 className={styles.title}>Get any Wallet NFT Transfers </h1>
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
        <div className={styles.getTokenForm}>
          <label className={styles.label} htmlFor="limitRes">
            Limit
          </label>
          <input
            className={styles.walletAddress}
            id="limitRes"
            name="limit"
            value={params.limit}
            onChange={handleParamsChange}
            required
          />
        </div>

        <button className={styles.form_btn} onClick={refreshNftTransactions}>
          Get Transfers
        </button>
        <div className="results">
          {nftTransactions && (
            <div className="nfts">{renderedTransactions}</div>
          )}
        </div>
      </section>
    </div>
  );
}

export default App;
