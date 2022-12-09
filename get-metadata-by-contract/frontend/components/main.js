import { useState } from "react";
import axios from "axios";
import Select from "react-select";
import styles from "../styles/Home.module.css";

export default function Header() {
  const [showResult, setShowResult] = useState(false);
  const [result, setResult] = useState([]);
  const [chainValue, setChainValue] = useState("");
  let address = [];

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
    address.push(document.querySelector("#contractAddress").value);
    const chain = chainValue.value;

    const response = await axios.get(`http://localhost:5001/contract`, {
      params: { address, chain },
    });

    console.log(response);
    setResult(response.data);
    setShowResult(true);
    document.querySelector("#contractAddress").value = "";
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
        <label className={styles.label} htmlFor="contractAddress">
          Add Contract Address
        </label>
        <input
          className={styles.contractAddress}
          type="text"
          id="contractAddress"
          name="contractAddress"
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
          result.map((contract) => {
            return (
              <section
                className={styles.resultContainer}
                key={result.indexOf(contract)}
              >
                <img src={contract.thumbnail} />
                <p className={styles.name}>{contract.name}</p>
                <p className={styles.symbol}>{contract.symbol}</p>
              </section>
            );
          })}
      </section>
    </section>
  );
}
