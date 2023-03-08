import { useState } from "react";
import axios from "axios";
import Image from "next/image";
import { Card, Illustration } from "@web3uikit/core";
import styles from "@/styles/Home.module.css";
import MoralisLogo from "../public/assets/moralis.png";
import AptosLogo from "../public/assets/aptos_white.png";

export default function Main() {
  const [walletAddress, setWalletAddress] = useState("");
  const [result, setResult] = useState([]);
  const [showResult, setShowResult] = useState(false);

  const handleChange = (e) => {
    setWalletAddress(e.target.value);
  };

  const handleSubmit = async () => {
    document.querySelector("#inputField").value = "";

    const response = await axios.get(`http://localhost:5001/getwalletcoins`, {
      params: { address: walletAddress },
    });

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
      <section className={styles.tokenContainer}>
        {showResult &&
          result.map((token) => {
            return (
              <section className={styles.tokenSection} key={token.coin_type}>
                <Card
                  onClick={function noRefCheck() {}}
                  setIsSelected={function noRefCheck() {}}
                  title={`${token.amount / 10 ** 8} ${token.coin_type
                    .split("::")
                    .slice(-1)[0]
                    .replace(/([A-Z])/g, " $1")}`}
                  description={`Latest Transaction: ${token.last_transaction_timestamp.split(
                    "T",
                    1
                  )}`}
                  className={styles.card}
                >
                  <Illustration height="180px" logo="token" width="100%" />
                </Card>
              </section>
            );
          })}
      </section>
    </section>
  );
}
