import axios from "axios";
import { useState } from "react";
import { Card } from "antd";
import styles from "./styles/Home.module.css";
import Logo from "./img/Moralis_logo.png";
import "./App.css";

function App() {
  const [unsDomain, setUnsDomain] = useState(null);

  const refreshDomain = async () => {
    await axios
      .get(`/resolve_domain?domain=${params.address}&currency=${params.chain}`)
      .then((res) => {
        setUnsDomain(res.data);
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

  const renderedDomain = unsDomain && (
    <Card
      hoverable
      className="result-card"
      title="Your Unstoppable Domain"
      style={{ width: 300 }}
    >
      <div className="result-element">
        <p className="result-begin">domain address:</p>
        <p className="result-end">{unsDomain.address}</p>
      </div>
    </Card>
  );

  return (
    <div>
      <div className={styles.header}>
        <div className={styles.moralis_logo}>
          <img src={Logo} alt="Logo image" width="102" height="82" />
        </div>
        <h1 className={styles.title}>Resolve Unstoppable Domain </h1>
      </div>
      <section className={styles.main}>
        <div className={styles.getTokenForm}>
          <label className={styles.label} htmlFor="nftAddress">
            Domain
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
            Currency
          </label>
          <select className={styles.walletAddress} name="chain">
            <option value="eth">Ethereum</option>
            <option value="goerli">Goerli</option>
            <option value="polygon">Polygon</option>
            <option value="mumbai">Mumbai</option>
            <option value="bsc">Binance</option>
          </select>
        </div>

        <button className={styles.form_btn} onClick={refreshDomain}>
          Resolve Domain
        </button>
        <div className="results">
          {unsDomain && <div className="nfts">{renderedDomain}</div>}
        </div>
      </section>
    </div>
  );
}

export default App;
