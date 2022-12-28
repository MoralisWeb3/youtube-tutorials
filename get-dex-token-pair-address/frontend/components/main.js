import { useState } from "react";
import axios from "axios";
import Select from "react-select";
import styles from "../styles/Home.module.css";

export default function Header() {
  const [showResult, setShowResult] = useState(false);
  const [pairAddress, setPairADdress] = useState("");
  const [token0, setToken0] = useState([]);
  const [token1, setToken1] = useState([]);
  const [exchangeValue, setExchangeValue] = useState("");
  const [chainValue, setChainValue] = useState("");
  let token0Address;
  let token1Address;

  const exchangeValueOptions = [
    { value: "uniswapv2", label: "uniswapv2" },
    { value: "uniswapv3", label: "uniswapv3" },
    { value: "sushiswapv2", label: "sushiswapv2" },
  ];

  const chainValueOptions = [
    { value: "0x1", label: "Ethereum" },
    { value: "0x5", label: "Goerli" },
    { value: "0x13881", label: "Mumbai" },
  ];

  const customStyles = {
    option: (provided) => ({
      ...provided,
      color: "#000000",
      backgroundColor: "#ffffff",
      borderColor: "#00c3f8",
    }),
  };

  const changeExchangeHandler = (exchangeValue) => {
    setExchangeValue(exchangeValue);
  };

  const changeChainHandler = (chainValue) => {
    setChainValue(chainValue);
  };

  const handleSubmit = async () => {
    token0Address = document.querySelector("#inputField0").value;
    token1Address = document.querySelector("#inputField1").value;
    const exchange = exchangeValue.value;
    const chain = chainValue.value;

    const response = await axios.get(`http://localhost:5001/getdexpair`, {
      params: { token0Address, token1Address, exchange, chain },
    });

    console.log(response);
    setPairADdress(response.data.pairAddress);
    setToken0(response.data.token0);
    setToken1(response.data.token1);
    setShowResult(true);
    document.querySelector("#inputField0").value = "";
    document.querySelector("#inputField1").value = "";
    setExchangeValue("");
    setChainValue("");
  };

  return (
    <section className={styles.main}>
      <form
        className={styles.getForm}
        name="create-profile-form"
        method="POST"
        action="#"
      >
        <label className={styles.label} htmlFor="inputField">
          Add Token0Address
        </label>
        <input
          className={styles.inputField}
          type="text"
          id="inputField0"
          name="inputField"
          maxLength="120"
          required
        />
        <label className={styles.label} htmlFor="inputField">
          Add Token1Address
        </label>
        <input
          className={styles.inputField}
          type="text"
          id="inputField1"
          name="inputField"
          maxLength="120"
          required
        />
        <label className={styles.label} htmlFor="inputField">
          Select Exchange
        </label>
        <Select
          styles={(customStyles, { borderColor: "#00c3f8" })}
          options={exchangeValueOptions}
          value={exchangeValue}
          instanceId="long-value-select"
          onChange={changeExchangeHandler}
        />
        <label className={styles.label} htmlFor="inputField">
          Select Chain
        </label>
        <Select
          styles={(customStyles, { borderColor: "#00c3f8" })}
          options={chainValueOptions}
          value={chainValue}
          instanceId="long-value-select"
          onChange={changeChainHandler}
        />
      </form>
      <button className={styles.form_btn} onClick={handleSubmit}>
        Submit
      </button>
      <section className={styles.result}>
        {showResult && (
          <section className={styles.resultContainer}>
            <section className={styles.resultContainer_header}>
              <span>Pair Address: {pairAddress}</span>
            </section>
            <h4>Token 0</h4>
            <section className={styles.resultContainer_data}>
              <img src={token0.thumbnail} />
              <p className={styles.name}>Name: {token0.name}</p>
              <p className={styles.symbol}>Symbol: {token0.symbol}</p>
            </section>
            <h4>Token 1</h4>
            <section className={styles.resultContainer_data}>
              <img src={token1.thumbnail} />
              <p className={styles.name}>Name: {token1.name}</p>
              <p className={styles.symbol}>Symbol: {token1.symbol}</p>
            </section>
          </section>
        )}
      </section>
    </section>
  );
}
