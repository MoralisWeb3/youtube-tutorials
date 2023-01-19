import { useEffect, useState } from "react";
import axios from "axios";
import styles from "../styles/Home.module.css";
import { useAccount } from "wagmi";
import { Illustration } from "@web3uikit/core";

export default function Token() {
  const { address } = useAccount();
  const [result, setResult] = useState([]);
  const [showResult, setShowResult] = useState(false);

  useEffect(() => {
    if (address) {
      async function getWalletBalance() {
        const response = await axios.get(
          "http://localhost:5001/getwalletbalance",
          {
            params: { address, chain: "0x5" },
          }
        );
        console.log("r", response);
        setResult(response.data);
        setShowResult(true);
      }
      getWalletBalance();
    } else {
      console.log("Not Connected");
    }
  }, [address]);

  return (
    <section className={styles.result_token}>
      {showResult &&
        result.map((token) => {
          return (
            <section
              className={styles.tokenContainer}
              key={result.indexOf(token)}
            >
              {token.thumbnail ? (
                <img src={token.thumbnail} />
              ) : (
                <Illustration height="80px" width="80px" logo="token" />
              )}
              <p className={styles.name}>{token.name}</p>
              <p className={styles.amount}>
                {(token.balance / 10 ** token.decimals).toFixed(2)}
              </p>
            </section>
          );
        })}
    </section>
  );
}
