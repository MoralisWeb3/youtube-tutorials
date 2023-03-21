import { useState } from "react";
import axios from "axios";
import Image from "next/image";
import { Illustration } from "@web3uikit/core";
import { Copy } from "@web3uikit/icons";
import styles from "@/styles/Home.module.css";
import MoralisLogo from "../public/assets/moralis.png";
import AptosLogo from "../public/assets/aptos_white.png";

export default function Main() {
  const [creatorAddress, setCreatorAddress] = useState("");
  const [result, setResult] = useState([]);
  const [showResult, setShowResult] = useState(false);

  const handleChange = (e) => {
    setCreatorAddress(e.target.value);
  };

  const handleSubmit = async () => {
    document.querySelector("#inputField").value = "";

    const response = await axios.get(`http://localhost:5001/getcoins`, {
      params: { address: creatorAddress },
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
          <button className={styles.form_btn} onClick={handleSubmit}>
            Submit
          </button>
        </section>
      </section>
      <section className={styles.resultContainer}>
        {showResult &&
          (result?.length ? (
            result.map((coin, i) => {
              return (
                <section
                  className={styles.resultSection}
                  key={coin.coin_type_hash}
                >
                  <section className={styles.card}>
                    <Illustration logo="token" />
                    <h4 className={styles.card_title}>{coin.name}</h4>
                    <p className={styles.card_symbol}>{coin.symbol}</p>
                    <section className={styles.card_details}>
                      <section>
                        <p className={styles.card_details__key}>
                          Creator Address:{" "}
                        </p>
                        <p className={styles.card_details__key}>Symbol: </p>
                        <p className={styles.card_details__key}>Decimal: </p>
                        <p className={styles.card_details__key}>
                          Time Created:{" "}
                        </p>
                      </section>
                      <section>
                        <p
                          className={`${styles.card_details__value} ${styles.blueText}`}
                        >
                          {`${coin.creator_address.slice(
                            0,
                            4
                          )}...${coin.creator_address.slice(62)}`}
                          <Copy fontSize="20px" />
                        </p>
                        <p className={styles.card_details__value}>
                          {coin.symbol}
                        </p>
                        <p className={styles.card_details__value}>
                          {coin.decimals}
                        </p>
                        <p className={styles.card_details__value}>
                          {coin.transaction_created_timestamp.split("T")[0]}
                        </p>
                      </section>
                    </section>
                  </section>
                </section>
              );
            })
          ) : (
            <section className={styles.beanBoy}>
              <Illustration logo="beanBoyStepOne" />
            </section>
          ))}
      </section>
    </section>
  );
}
