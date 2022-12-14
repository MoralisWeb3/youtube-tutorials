import axios from "axios";
import { useEffect, useState } from "react";
import { Card } from "antd";
import styles from "./styles/Home.module.css";
import Logo from "./img/Moralis_logo.png";
import "./App.css";

function App() {
  const [nftData, setNftData] = useState(null);
  const { Meta } = Card;

  const refreshNftData = async () => {
    await axios({
      method: "POST",
      url: "/get_nfts/",
      data: {
        chain: params.chain,
        tokens: params.tokens,
      },
    })
      .then((res) => {
        setNftData(res.data);
      })
      .catch((err) => {
        console.log(err);
      });
  };

  const [nftvalues, setNftValues] = useState({
    token_address: "",
    token_id: "",
  });

  const [params, setParams] = useState({
    chain: "eth",
    tokens: [],
  });

  const handleParamsChange = (e) => {
    setParams({ ...params, [e.target.name]: e.target.value });
  };

  const handleNftChange = (e) => {
    setNftValues({ ...nftvalues, [e.target.name]: e.target.value });
  };

  const addItemToTokens = () => {
    setParams((values) => ({
      ...values,
      tokens: [...values.tokens, nftvalues],
    }));
    setNftValues({ token_address: "", token_id: "" });
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

            <p>
              <b>Token URI:</b> {nft?.token_uri}
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
        <h1 className={styles.title}>Get Multiple NFTS </h1>
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
            name="token_address"
            value={nftvalues.token_address}
            onChange={handleNftChange}
            maxLength="120"
            required
          />
        </div>
        <div className={styles.getTokenForm}>
          <label className={styles.label} htmlFor="nftAddress">
            Token Id
          </label>
          <input
            className={styles.walletAddress}
            type="text"
            id="token_id"
            name="token_id"
            value={nftvalues.token_id}
            onChange={handleNftChange}
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

        <button className={styles.form_btn_add} onClick={addItemToTokens}>
          Add Item
        </button>
        <button className={styles.form_btn} onClick={refreshNftData}>
          Get NFTs
        </button>

        <div className="results">
          {nftData && <div className="nfts">{renderedData}</div>}
        </div>
      </section>
    </div>
  );
}

export default App;
