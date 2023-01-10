import axios from "axios";
import { useEffect, useState } from "react";
import styles from "../styles/Home.module.css";
import { useFirebase } from "./FirebaseInitializer";

import CreateStream from "./createStream";

export default function GetTransactions() {
  const { auth } = useFirebase();
  const [address, setAddress] = useState(() => auth.currentUser?.displayName);
  const [showTxs, setShowTsx] = useState(false);
  const [txs, setTxs] = useState([]);
  const chain = "0x1";

  useEffect(() => {
    async function getData() {
      const response = await axios.get(`http://localhost:3001/gettxs`, {
        params: { address, chain },
      });
      console.log(response);
      setTxs(response.data.result);
      setShowTsx(true);
    }
    getData();
  }, []);

  return (
    <section className={styles.loggedInMain}>
      <section className={styles.loggedInAccount}>
        <section className={styles.result}>
          {showTxs &&
            txs.map((transactions, i) => {
              return (
                <section className={styles.resultContainer} key={i}>
                  <section className={styles.resultContainer_data}>
                    <p>From: {transactions.from_address}</p>
                    <p>To: {transactions.to_address}</p>
                    <p>Transaction Hash: {transactions.transaction_hash}</p>
                  </section>
                </section>
              );
            })}
        </section>
        <CreateStream />
      </section>
    </section>
  );
}
