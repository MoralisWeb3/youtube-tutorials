import { useState } from "react";
import axios from "axios";
import Select from "react-select";
import styles from "../styles/Home.module.css";

export default function Header() {
  const [showResult, setShowResult] = useState(false);
  const [result, setResult] = useState([]);
  const [chainValue, setChainValue] = useState("");

  const valueOptions = [
    { value: "0xa4b1", label: "Arbitrum" },
    { value: "0x38", label: "Binance" },
    { value: "0x1", label: "Ethereum" },
    { value: "0x5", label: "Goerli" },
  ];

  const customStyles = {
    option: (provided) => ({
      ...provided,
      color: "#000000",
      backgroundColor: "#ffffff",
      borderColor: "#00c3f8",
      width: "80",
    }),
  };

  const changeHandler = (chainValue) => {
    setChainValue(chainValue);
  };

  const handleSubmit = async () => {
    const chain = chainValue.value;

    const response = await axios.get(`http://localhost:5001/getburns`, {
      params: { chain },
    });

    console.log(response);
    setResult(response.data.result);
    setShowResult(true);
  };

  return (
    <section className={styles.main}>
      <section className={styles.main_form}>
        <Select
          styles={(customStyles, { borderColor: "#00c3f8" })}
          options={valueOptions}
          value={chainValue}
          instanceId="long-value-select"
          onChange={changeHandler}
          className={styles.dropdown}
        />
        <button className={styles.submit_btn} onClick={handleSubmit}>
          Submit
        </button>
      </section>
      <section className={styles.result}>
        {showResult &&
          result.map((burn, i) => {
            return (
              <section className={styles.resultContainer} key={i}>
                <section className={styles.card}>
                  <section className={styles.card_details}>
                    <section className={styles.fire}>
                      <section className={styles.fire_left}>
                        <section className={styles.main_fire}></section>
                        <section className={styles.particle_fire}></section>
                      </section>
                      <section className={styles.fire_center}>
                        <section className={styles.main_fire}></section>
                        <section className={styles.particle_fire}></section>
                      </section>
                      <section className={styles.fire_right}>
                        <section className={styles.main_fire}></section>
                        <section className={styles.particle_fire}></section>
                      </section>
                      <section className={styles.fire_bottom}>
                        <section className={styles.main_fire}></section>
                      </section>
                    </section>
                    <section>
                      <p className={styles.card_details__key}>Date:</p>
                      <p className={styles.card_details__key}>
                        Contract Address:
                      </p>
                      <p className={styles.card_details__key}>Txn Hash: </p>
                      <p className={styles.card_details__key}>Value: </p>
                      <p className={styles.card_details__key}>From Wallet: </p>
                    </section>
                    <section>
                      <p className={styles.card_details__value}>
                        {burn.block_timestamp.split("T")[0]}
                      </p>
                      <p className={styles.card_details__value}>
                        {burn.contract_address}
                      </p>
                      <p className={styles.card_details__value}>
                        {burn.transaction_hash.slice(0, 40)}...
                      </p>
                      <p className={styles.card_details__value}>{burn.value}</p>
                      <p className={styles.card_details__value}>
                        {burn.from_wallet}
                      </p>
                    </section>
                  </section>
                </section>
              </section>
            );
          })}
      </section>
    </section>
  );
}
