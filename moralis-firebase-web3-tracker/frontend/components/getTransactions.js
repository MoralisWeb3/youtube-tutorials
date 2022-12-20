import { useAccount } from "wagmi";
import axios from "axios";
import { useEffect, useState } from "react";
import styles from "../styles/Home.module.css";

export default function getTransactions() {
  const [txs, setTxs] = useState([]);
  const { address } = useAccount();
  const chain = "0x1";

  useEffect(() => {
    let response;
    async function getData() {
      response = await axios.get(`http://localhost:5001/gettxs`, {
        params: { address, chain },
      });
      console.log(response);
      setTxs(response.data.result);
    }
    getData();
  }, []);

  return (
    <section>
      {txs.map((transactions, i) => {
        return (
          <section className={styles.resultContainer} key={i}>
            <section className={styles.resultContainer_header}>
              <span>Time: {transactions.block_timestamp.split("T", 1)[0]}</span>
              <span>Block Number: {transactions.block_number}</span>
            </section>
            <section className={styles.resultContainer_data}>
              <p>From: {transactions.from_address}</p>
              <p>To: {transactions.to_address}</p>
              <p>Transaction Hash: {transactions.transaction_hash}</p>
            </section>
          </section>
        );
      })}
    </section>
  );
}
