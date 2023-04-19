import { useState } from "react";
import axios from "axios";
import styles from "../styles/Home.module.css";

export default function Main() {
  const [showResult, setShowResult] = useState(false);
  const [result, setResult] = useState([]);
  const [walletAddress, setWalletAddress] = useState("");

  const handleChange = (e) => {
    setWalletAddress(e.target.value);
  };

  const handleSubmit = async () => {
    document.querySelector("#inputField").value = "";

    const response = await axios.get("http://localhost:5001/getcointransfers", {
      params: { address: walletAddress },
    });

    console.log("res", response);

    setResult(response.data.result);
    setShowResult(true);
  };

  return (
    <section className={styles.main}>
      <section className={styles.input_section}>
        <input
          className={styles.inputField}
          type="text"
          id="inputField"
          name="inputField"
          maxLength="120"
          required
          onChange={handleChange}
        />
        <button className={styles.submit_btn} onClick={handleSubmit}>
          Submit
        </button>
      </section>
      <section className={styles.result}>
        {showResult &&
          result.map((coin, i) => {
            return (
              <section className={styles.resultContainer} key={i}>
                <section className={styles.card}>
                  <section className={styles.card_details}>
                    <section>
                      <p className={styles.card_details__key}>Activity:</p>
                      <p className={styles.card_details__key}>Coin Type:</p>
                      <p className={styles.card_details__key}>Address: </p>
                      <p className={styles.card_details__key}>Time: </p>
                    </section>
                    <section>
                      <p className={styles.card_details__value}>
                        {coin.activity_type
                          .split("::")
                          .slice(-1)[0]
                          .replace(/([A-Z])/g, " $1")}
                      </p>
                      <p className={styles.card_details__value}>
                        {coin.coin_type
                          .split("::")
                          .slice(-1)[0]
                          .replace(/([A-Z])/g, " $1")}
                      </p>
                      <p className={styles.card_details__value}>
                        {coin.owner_address.slice(0, 8)}...
                      </p>
                      <p className={styles.card_details__value}>
                        {coin.transaction_timestamp.split("T")[0]}
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
