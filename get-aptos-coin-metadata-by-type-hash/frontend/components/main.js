import { useState } from "react";
import axios from "axios";
import styles from "../styles/Home.module.css";

export default function Main() {
  const [showResult, setShowResult] = useState(false);
  const [result, setResult] = useState([]);
  const [inputValue, setInputValue] = useState("");

  const handleChange = (e) => {
    setInputValue(e.target.value);
  };

  const handleSubmit = async () => {
    document.querySelector("#inputField").value = "";

    const response = await axios.get("http://localhost:5001/getcoinmetadata", {
      params: { typeHash: inputValue },
    });

    setResult(response.data[0]);
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
        {showResult && (
          <section className={styles.resultContainer}>
            <section className={styles.card}>
              <section className={styles.card_details}>
                <section>
                  <p className={styles.card_details__key}>Creator Address:</p>
                  <p className={styles.card_details__key}>Name:</p>
                  <p className={styles.card_details__key}>Symbol: </p>
                  <p className={styles.card_details__key}>Decimals: </p>
                  <p className={styles.card_details__key}>Time Created: </p>
                </section>
                <section>
                  <p className={styles.card_details__value}>
                    {result.creator_address.slice(0, 8)}
                  </p>
                  <p className={styles.card_details__value}>{result.name}</p>
                  <p className={styles.card_details__value}>{result.symbol}</p>
                  <p className={styles.card_details__value}>
                    {result.decimals}
                  </p>
                  <p className={styles.card_details__value}>
                    {result.transaction_created_timestamp.split("T")[0]}
                  </p>
                </section>
              </section>
            </section>
          </section>
        )}
      </section>
    </section>
  );
}
