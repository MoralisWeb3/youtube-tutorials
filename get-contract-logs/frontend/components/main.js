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
    address = document.querySelector("#inputField").value;
    const chain = chainValue.value;

    const response = await axios.get(`http://localhost:5001/getlogs`, {
      params: { address, chain },
    });

    console.log(response.data);
    setResult(response.data.result);
    setShowResult(true);
    document.querySelector("#inputField").value = "";
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
          Add Contract Address
        </label>
        <input
          className={styles.inputField}
          type="text"
          id="inputField"
          name="inputField"
          maxLength="120"
          required
        />
        <label className={styles.label} htmlFor="inputField">
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
          result.map((log, i) => {
            return (
              <section className={styles.resultContainer} key={i}>
                <section className={styles.resultContainer_header}>
                  <span>Time: {log.block_timestamp.split("T", 1)[0]}</span>
                  <span>Block Number: {log.block_number}</span>
                </section>
                <section className={styles.resultContainer_data}>
                  <p>Address: {log.address}</p>
                  <p>Transaction Hash: {log.transaction_hash}</p>
                </section>
              </section>
            );
          })}
      </section>
    </section>
  );
}
