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

    const response = await axios.get(
      "http://localhost:5001/getwallettransfers",
      {
        params: { address: walletAddress },
      }
    );

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
          result.map((nft, i) => {
            return (
              <section className={styles.resultContainer} key={i}>
                <section className={styles.card}>
                  <section className={styles.card_details}>
                    <section>
                      <p className={styles.card_details__key}>
                        Collection Name:
                      </p>
                      <p className={styles.card_details__key}>Name:</p>
                      <p className={styles.card_details__key}>To: </p>
                      <p className={styles.card_details__key}>Event: </p>
                      <p className={styles.card_details__key}>Time: </p>
                    </section>
                    <section>
                      <p className={styles.card_details__value}>
                        {nft.collection_name}
                      </p>
                      <p className={styles.card_details__value}>{nft.name}</p>
                      <p className={styles.card_details__value}>
                        {nft.to_address
                          ? `${nft.to_address.slice(0, 8)}...`
                          : "no address"}
                      </p>
                      <p className={styles.card_details__value}>
                        {nft.transfer_type
                          .split("::")
                          .slice(-1)[0]
                          .replace(/([A-Z])/g, " $1")}
                      </p>
                      <p className={styles.card_details__value}>
                        {nft.transaction_timestamp.split("T")[0]}
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
