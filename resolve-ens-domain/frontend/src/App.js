import axios from "axios";
import { useState } from "react";
import { Card } from "antd";
import styles from "./styles/Home.module.css";
import Logo from "./img/Moralis_logo.png";
import "./App.css";

function App() {
  const [ensDomain, setEnsDomain] = useState(null);

  const refreshDomain = async () => {
    await axios
      .get(`/resolve_domain?address=${params.address}`)
      .then((res) => {
        setEnsDomain(res.data);
      })
      .catch((err) => console.log(err));
  };

  const [params, setParams] = useState({
    address: "",
  });

  const handleParamsChange = (e) => {
    setParams({ ...params, [e.target.name]: e.target.value });
  };

  const renderedDomain = ensDomain && (
    <Card
      hoverable
      className="result-card"
      title="Your Ens Domain"
      style={{ width: 300 }}
    >
      <div className="result-element">
        <p className="result-begin">domain name:</p>
        <p className="result-end">{ensDomain.name}</p>
      </div>
    </Card>
  );

  return (
    <div>
      <div className={styles.header}>
        <div className={styles.moralis_logo}>
          <img src={Logo} alt="Logo image" width="102" height="82" />
        </div>
        <h1 className={styles.title}>Resolve any ENS domain </h1>
      </div>
      <section className={styles.main}>
        <div className={styles.getTokenForm}>
          <label className={styles.label} htmlFor="nftAddress">
            Domain Address
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

        <button className={styles.form_btn} onClick={refreshDomain}>
          Resolve Domain
        </button>
        <div className="results">
          {ensDomain && <div className="nfts">{renderedDomain}</div>}
        </div>
      </section>
    </div>
  );
}

export default App;
