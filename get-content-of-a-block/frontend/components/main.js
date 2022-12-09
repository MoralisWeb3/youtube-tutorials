import { useState } from "react";
import axios from "axios";
import Select from "react-select";
import styles from "../styles/Home.module.css";

export default function Header() {
  const [showResult, setShowResult] = useState(false);
  const [result, setResult] = useState([]);
  const [chainValue, setChainValue] = useState("");
  let blockNumber;

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
    blockNumber = document.querySelector("#blockNumber").value;
    const chain = chainValue.value;

    const response = await axios.get(`http://localhost:5001/getblock`, {
      params: { blockNumber, chain },
    });

    console.log(response);
    setResult(response.data.transactions);
    setShowResult(true);
    document.querySelector("#blockNumber").value = "";
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
        <label className={styles.label} htmlFor="blockNumber">
          Add Block Number
        </label>
        <input
          className={styles.blockNumber}
          type="text"
          id="blockNumber"
          name="blockNumber"
          maxLength="120"
          required
        />
        <label className={styles.label} htmlFor="contractAddress">
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
          result.map((block) => {
            return (
              <section
                className={styles.resultContainer}
                key={result.indexOf(block)}
              >
                <section className={styles.resultContainer_header}>
                  <span>Time: {block.block_timestamp.split("T", 1)[0]}</span>
                  <span>Block Number: {block.block_number}</span>
                </section>
                <section className={styles.resultContainer_data}>
                  <p>From: {block.from_address}</p>
                  <p>Gas Price: {block.gas_price / 10 ** 9} Gwei</p>
                  <p>Transaction Hash: {block.hash}</p>
                </section>
              </section>
            );
          })}
      </section>
    </section>
  );
}
