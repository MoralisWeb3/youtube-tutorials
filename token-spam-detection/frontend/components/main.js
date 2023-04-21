import { useState } from "react";
import axios from "axios";
import { Illustration } from "@web3uikit/core";
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

    const response = await axios.get("http://localhost:5001/gettokens", {
      params: { address: inputValue },
    });

    setResult(response.data);
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
      <section className={styles.resultContainer}>
        {showResult &&
          result.map((token, i) => {
            return (
              <section className={styles.resultSection} key={i}>
                <section className={styles.card}>
                  <Illustration logo="token" className={styles.illustration} />
                  <h2>{token.name}</h2>
                  <p
                    className={`${
                      token.possible_spam ? styles.spam : styles.noSpam
                    }`}
                  >
                    {token.possible_spam ? "SPAM 👎" : "NO SPAM 👍"}
                  </p>
                </section>
              </section>
            );
          })}
      </section>
    </section>
  );
}
