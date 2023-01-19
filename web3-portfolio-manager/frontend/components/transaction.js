import { useEffect, useState } from "react";
import axios from "axios";
import styles from "../styles/Home.module.css";
import { useAccount } from "wagmi";

export default function Transaction() {
  const { address } = useAccount();
  const [result, setResult] = useState([]);
  const [showResult, setShowResult] = useState(false);

  useEffect(() => {
    if (address) {
      async function getWalletNft() {
        const response = await axios.get(
          "http://localhost:5001/getwallettransactions",
          {
            params: { address, chain: "0x1" },
          }
        );
        console.log("r", response);
        setResult(response.data.result);
        setShowResult(true);
      }
      getWalletNft();
    } else {
      console.log("Not Connected");
    }
  }, [address]);

  return (
    <section className={styles.result_transaction}>
      {showResult &&
        result.map((transaction) => {
          return (
            <section
              className={styles.transactionContainer}
              key={result.indexOf(transaction)}
            >
              <p>From: {transaction.from_address}</p>
              <p>To: {transaction.to_address}</p>
              <p>Transaction Hash: {transaction.transaction_hash}</p>
            </section>
          );
        })}
    </section>
  );
}
