import { useEffect, useState } from "react";
import Image from "next/image";
import axios from "axios";
import styles from "../styles/Home.module.css";

import GasPump from "../public/assets/gas_pump.png";

export default function Header() {
  const [showResult, setShowResult] = useState(false);
  const [result, setResult] = useState([]);

  useEffect(() => {
    async function getGas() {
      await axios.get("http://localhost:5001/getgasprice").then((response) => {
        setResult(response.data);
        setShowResult(true);
        console.log(response);
      });
    }

    getGas();
  }, []);

  return (
    <section className={styles.main}>
      {showResult && (
        <section className={styles.result}>
          <span>
            {
              new Date(result.__headers.date)
                .toLocaleString("sv-SE")
                .split(" ")[0]
            }
          </span>
          <section className={styles.resultContainer}>
            <section className={styles.deprioritized}>
              <span>DEPRIORITIZED</span>
              <Image
                src={GasPump}
                alt="Gas pump image"
                width="130"
                height="130"
              />
              {result.deprioritized_gas_estimate}
            </section>
            <section className={styles.estimated}>
              <span>ESTIMATED</span>
              <Image
                src={GasPump}
                alt="Gas pump image"
                width="130"
                height="130"
              />
              {result.gas_estimate}
            </section>
            <section className={styles.prioritized}>
              <span>PRIORITIZED</span>
              <Image
                src={GasPump}
                alt="Gas pump image"
                width="130"
                height="130"
              />
              {result.prioritized_gas_estimate}
            </section>
          </section>
        </section>
      )}
    </section>
  );
}
