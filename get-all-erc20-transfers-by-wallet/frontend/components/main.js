import { useState } from "react";
import axios from "axios";
import Select from "react-select";
import styles from "../styles/Home.module.css";

export default function Header() {
  const [showResult, setShowResult] = useState(false);
  const [result, setResult] = useState([]);
  const [chainValue, setChainValue] = useState("");
  let address;

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
    address = document.querySelector("#walletAddress").value;
    const chain = chainValue.value;

    const response = await axios.get(`http://localhost:5001/transfers`, {
      params: { address: address, chain: chain },
    });

    console.log(response);
    setResult(response.data.result);
    setShowResult(true);
    document.querySelector("#walletAddress").value = "";
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
        <label className={styles.label} htmlFor="walletAddress">
          Add ERC20 Wallet Address
        </label>
        <input
          className={styles.walletAddress}
          type="text"
          id="walletAddress"
          name="walletAddress"
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
          result.map((transfer) => {
            return (
              <section
                className={styles.resultContainer}
                key={result.indexOf(transfer)}
              >
                <section className={styles.resultContainer_header}>
                  <span>Time: {transfer.block_timestamp.split("T", 1)[0]}</span>
                  <span>Block Number: {transfer.block_number}</span>
                </section>
                <section className={styles.resultContainer_data}>
                  <p>From: {transfer.from_address}</p>
                  <p>To: {transfer.to_address}</p>
                  <p>Transaction Hash: {transfer.transaction_hash}</p>
                </section>
              </section>
            );
          })}
      </section>
    </section>
  );
}
