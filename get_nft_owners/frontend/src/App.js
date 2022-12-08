import axios from "axios";
import { useState } from "react";
import { Card } from "antd";
import styles from "./styles/Home.module.css";
import Logo from "./img/Moralis_logo.png";
import "./App.css";

function App() {
  const [nftOwners, setNftOwners] = useState(null);

  const refreshNftOwners = async () => {
    await axios
      .get(
        `/get_owners?address=${params.address}&chain=${params.chain}&limit=${params.limit}`
      )
      .then((res) => {
        setNftOwners(res.data.result);
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

  const renderedOwners =
    nftOwners &&
    Object.values(nftOwners).map((owner) => {
      return (
        <Card
          hoverable
          className="result-card"
          title={`name: ${owner.name}`}
          extra={
            <p>
              <b>Block Number:</b> {owner.block_number}
            </p>
          }
          style={{ width: 300 }}
        >
          <div className="result-element">
            <p className="result-begin">Token id:</p>
            <p className="result-end">{owner.token_id}</p>
          </div>
          <div className="result-element">
            <p className="result-begin">Owner:</p>
            <p className="result-end">{owner.owner_of}</p>
          </div>
          <div className="result-element">
            <p className="result-begin">Token address:</p>
            <p className="result-end">{owner.token_address}</p>
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
        <h1 className={styles.title}>Get NFT Owners of a Contract </h1>
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

        <button className={styles.form_btn} onClick={refreshNftOwners}>
          Get Owners
        </button>
        <div className="results">
          {nftOwners && <div className="nfts">{renderedOwners}</div>}
        </div>
      </section>
    </div>
  );
}

export default App;
