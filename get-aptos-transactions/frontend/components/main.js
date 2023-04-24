import { useEffect, useState } from "react";
import axios from "axios";
import styles from "../styles/Home.module.css";

export default function Main() {
  const [showResults, setShowResults] = useState(false);
  const [result, setResult] = useState([]);

  useEffect(() => {
    async function getTransactions() {
      await axios
        .get("http://localhost:5001/gettransactions")
        .then((response) => {
          setResult(response.data);
          setShowResults(true);
        });
    }
    getTransactions();
  }, []);

  return (
    <section className={styles.tableContainer}>
      <table className={styles.table}>
        <thead>
          <tr className={styles.tableHeader_row}>
            <th>VERSION</th>
            <th>TYPE</th>
            <th>TIMESTAMP</th>
            <th>SENDER</th>
            <th>SENT TO</th>
            <th>FUNCTION</th>
            <th>AMOUNT</th>
          </tr>
        </thead>
        <tbody>
          {showResults &&
            result.map((txn, i) => {
              if (txn.events && txn.events[0]?.type) {
                return (
                  <tr className={styles.tableBody_row}>
                    <td>{txn.version}</td>
                    <td>
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        fill="none"
                        viewBox="0 0 24 24"
                        strokeWidth="1.5"
                        stroke="currentColor"
                        className={styles.svg}
                      >
                        <path
                          strokeLinecap="round"
                          strokeLinejoin="round"
                          d="M7.5 21L3 16.5m0 0L7.5 12M3 16.5h13.5m0-13.5L21 7.5m0 0L16.5 12M21 7.5H7.5"
                        />
                      </svg>
                    </td>
                    <td>
                      {
                        new Date(txn.timestamp / 1000)
                          .toLocaleString("sv-SE")
                          .split(" ")[0]
                      }
                    </td>
                    <td>
                      {txn.events[0]?.guid.account_address && (
                        <span className={styles.address}>
                          {txn.events[0]?.guid.account_address.slice(0, 12)}
                        </span>
                      )}
                    </td>
                    <td>
                      {txn.events[1] && (
                        <span className={styles.address}>
                          {txn.events[1]
                            ? txn.events[1].guid.account_address.slice(0, 12)
                            : ""}
                        </span>
                      )}
                    </td>
                    <td>
                      {txn.events[0]?.type && (
                        <span className={styles.function}>
                          {txn.events[0]?.type.split("::").slice(1).join("::")}
                        </span>
                      )}
                    </td>
                    <td className={styles.amountData}>
                      <span>
                        {txn.events[0]?.data.amount
                          ? `${Number(txn.events[0].data.amount).toFixed(
                              4
                            )} APT`
                          : "0 APT"}
                      </span>
                      <span>
                        Gas {Number(txn.gas_used / 100000000).toFixed(8)} APT
                      </span>
                    </td>
                  </tr>
                );
              }
            })}
        </tbody>
      </table>
    </section>
  );
}
