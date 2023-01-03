import { useState } from "react";
import axios from "axios";
import Select from "react-select";
import styles from "../styles/Home.module.css";

export default function Header() {
  const [showResult, setShowResult] = useState(false);
  const [reserve0, setReserve0] = useState([]);
  const [reserve1, setReserve1] = useState([]);
  const [chainValue, setChainValue] = useState("");
  let pairAddress;

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

  const changeChainHandler = (chainValue) => {
    setChainValue(chainValue);
  };

  const handleSubmit = async () => {
    pairAddress = document.querySelector("#inputField").value;
    const chain = chainValue.value;

    const response = await axios.get(`http://localhost:5001/getpairreserves`, {
      params: { pairAddress, chain },
    });

    console.log(response);
    setReserve0(response.data.reserve0);
    setReserve1(response.data.reserve1);
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
          Add Pair Address
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
              <span>Reserves</span>
            </section>
            <section className={styles.resultContainer_data}>
              <p>Reserve 0: {(reserve0 / 10 ** 18).toFixed(2)}</p>
              <p>Reserve 1: {(reserve1 / 10 ** 6).toFixed(2)}</p>
            </section>
          </section>
        )}
      </section>
    </section>
  );
}
