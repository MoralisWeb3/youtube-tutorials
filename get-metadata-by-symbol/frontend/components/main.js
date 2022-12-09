import { useState } from "react";
import axios from "axios";
import Select from "react-select";
import styles from "../styles/Home.module.css";

export default function Header() {
  const [showResult, setShowResult] = useState(false);
  const [result, setResult] = useState([]);
  const [chainValue, setChainValue] = useState("");
  let symbol = [];

  const valueOptions = [
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

  const changeHandler = (chainValue) => {
    setChainValue(chainValue);
  };

  const handleSubmit = async () => {
    symbol.push(document.querySelector("#symbol").value);
    const chain = chainValue.value;

    const response = await axios.get(`http://localhost:5001/symbol`, {
      params: { symbol, chain },
    });

    console.log(response);
    setResult(response.data);
    setShowResult(true);
    document.querySelector("#symbol").value = "";
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
        <label className={styles.label} htmlFor="symbol">
          Add Symbol
        </label>
        <input
          className={styles.symbolInput}
          type="text"
          id="symbol"
          name="symbol"
          maxLength="120"
          required
        />
        <label className={styles.label} htmlFor="symbol">
          Select Chain
        </label>
        <Select
          styles={(customStyles, { borderColor: "#00c3f8" })}
          options={valueOptions}
          value={chainValue}
          instanceId="long-value-select"
          onChange={changeHandler}
        />
      </form>
      <button className={styles.form_btn} onClick={handleSubmit}>
        Submit
      </button>
      <section className={styles.result}>
        {showResult &&
          result.map((symbol) => {
            return (
              <section
                className={styles.resultContainer}
                key={result.indexOf(symbol)}
              >
                <img src={symbol.thumbnail} />
                <p className={styles.name}>{symbol.name}</p>
                <p className={styles.symbol}>{symbol.symbol}</p>
              </section>
            );
          })}
      </section>
    </section>
  );
}
