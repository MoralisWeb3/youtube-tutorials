import { useState } from "react";
import axios from "axios";
import Image from "next/image";
import styles from "@/styles/Home.module.css";

import CardComp from "./cardComp.js";
import MoralisLogo from "../public/assets/moralis.png";
import AptosLogo from "../public/assets/aptos1.png";

export default function Main() {
  const [creatorAddressInput, setCreatorAddressInput] = useState("");
  const [showResult, setShowResult] = useState(false);
  const [result, setResult] = useState([]);

  const changeHandler = (e) => {
    setCreatorAddressInput(e.target.value);
  };

  const handleTestnetSubmit = async () => {
    document.querySelector("#inputFieldTestnet").value = "";

    const response = await axios.get(`http://localhost:5001/gettestnet`, {
      params: { creatorAddress: creatorAddressInput },
    });

    setResult(response.data);
    setShowResult(true);
  };

  const handleMainnetSubmit = async () => {
    document.querySelector("#inputFieldMainnet").value = "";

    const response = await axios.get(`http://localhost:5001/getmainnet`, {
      params: { creatorAddress: creatorAddressInput },
    });

    setResult(response.data);
    setShowResult(true);
  };

  return (
    <main className={styles.main}>
      <section className={styles.left}>
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
          <label className={styles.label} htmlFor="contractAddress">
            Add Creator Address - Testnet
          </label>
          <input
            className={styles.inputField}
            type="text"
            id="inputFieldTestnet"
            name="inputFieldTestnet"
            maxLength="120"
            required
            onChange={changeHandler}
          />
          <button className={styles.form_btn} onClick={handleTestnetSubmit}>
            Submit
          </button>
        </section>
        <section className={styles.input_section}>
          <label className={styles.label} htmlFor="contractAddress">
            Add Creator Address - Mainnet
          </label>
          <input
            className={styles.inputField}
            type="text"
            id="inputFieldMainnet"
            name="inputFieldMainnet"
            maxLength="120"
            required
            onChange={changeHandler}
          />
          <button className={styles.form_btn} onClick={handleMainnetSubmit}>
            Submit
          </button>
        </section>
      </section>
      <section className={styles.right}>
        {showResult &&
          result.map((nft) => {
            return <CardComp nft={nft} key={nft.token_data_id_hash} />;
          })}
      </section>
    </main>
  );
}
