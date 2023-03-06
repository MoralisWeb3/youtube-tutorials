import axios from "axios";
import { useState } from "react";
import { Card } from "antd";
import styles from "./styles/Home.module.css";
import Logo from "./img/Moralis_logo.png";
import "./App.css";


function App() {
  const [latestCoins, setLatestCoins] = useState(null);

  const [params, setParams] = useState({
    limit: "2",
  });

  const refreshCoins = async () => {
    await axios
      .get(`/get_coins/?limit=${params.limit}`)
      .then((res) => {
        setLatestCoins(res.data.result);

      })
      .catch((err) => console.log(err));
  };

  const handleParamsChange = (e) => {
    setParams({ ...params, [e.target.name]: e.target.value });
  };

  const renderedCoins = latestCoins && Object.values(latestCoins).map((coin) => {
    return (
      <Card
        hoverable
        className="result-card"
        title={`Coin name: ${coin.name}`}
        extra={
          <p>
            <b>Symbol:</b> {coin.symbol}
          </p>
        }
        style={{ width: 300 }}
      >
        <p className="result-begin">Creator address:</p>
        <div className="result-element">
          <p className="result-end">{coin.creator_address}</p>
        </div>
        <p className="result-begin">Coin Type:</p>
        <div className="coin-type">
          <p className="result-end">{coin.coin_type}</p>
        </div>
        <div className="result-element">
          <p className="result-begin">Decimals:</p>
          <p className="result-end">{coin.decimals}</p>
        </div>



      </Card>
    )
  })



  return (
    <div>
      <div className={styles.header}>
        <div className={styles.moralis_logo}>
          <img src={Logo} alt="Logo image" width="102" height="82" />
        </div>
        <h1 className={styles.title}>Get Aptos Latest Coins </h1>
      </div>
      <section className={styles.main}>
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

        <button className={styles.form_btn} onClick={refreshCoins}>
          Get Coins
        </button>
        <div className="results">
          {renderedCoins && <div className="nfts">{renderedCoins}</div>}
        </div>
      </section>
    </div>
  );
}

export default App;
