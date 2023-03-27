import { useState } from "react";
import axios from "axios";
import Image from "next/image";
import styles from "@/styles/Home.module.css";
import MoralisLogo from "../public/assets/moralis.png";
import AptosLogo from "../public/assets/aptos_white.png";

export default function Main() {
  const [coinTypeHash, setCoinTypeHash] = useState("");
  const [result, setResult] = useState([]);
  const [showResult, setShowResult] = useState(false);

  const handleChange = (e) => {
    setCoinTypeHash(e.target.value);
  };

  const handleSubmit = async () => {
    document.querySelector("#inputField").value = "";

    const response = await axios.get(`http://localhost:5001/gettopholders`, {
      params: { hash: coinTypeHash },
    });

    console.log("res", response);

    setResult(response.data.result);
    setShowResult(true);
  };

  return (
    <section className={styles.container}>
      <section className={styles.header}>
        <section className={styles.logo_section}>
          <Image src={MoralisLogo} alt="Logo image" width="102" height="82" />
          <svg
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            strokeWidth="1.5"
            stroke="currentColor"
            className={styles.mxa}
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M6 18L18 6M6 6l12 12"
            />
          </svg>
          <Image src={AptosLogo} alt="Logo image" width="82" height="82" />
        </section>
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
      </section>
      <section className={styles.resultContainer}>
        {showResult &&
          result.map((holder, i) => {
            return (
              <section className={styles.resultSection} key={i}>
                <section className={styles.card}>
                  <section className={styles.card_details}>
                    <section>
                      <p className={styles.card_details__key}>Amount:</p>
                      <p className={styles.card_details__key}>Owner:</p>
                      <p className={styles.card_details__key}>Token: </p>
                      <p className={styles.card_details__key}>Latest Txn: </p>
                    </section>
                    <section>
                      <h4 className={styles.card_details__value}>
                        {Number(holder.amount / 10 ** 6).toLocaleString(
                          "sv-SE"
                        )}
                      </h4>
                      <p className={styles.card_details__value}>
                        {holder.owner_address.slice(0, 8)}
                      </p>
                      <p className={styles.card_details__value}>
                        {holder.coin_type
                          .split("::")
                          .slice(-1)[0]
                          .replace(/([A-Z])/g, " $1")}
                      </p>
                      <p className={styles.card_details__value}>
                        {holder.last_transaction_timestamp.split("T")[0]}
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
