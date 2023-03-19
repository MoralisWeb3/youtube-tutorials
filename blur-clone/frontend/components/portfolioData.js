import styles from "@/styles/Home.module.css";

import GetNfts from "./getNfts.js";

export default function PortfolioData() {
  return (
    <section className={styles.PortfolioData_section}>
      <section>
        <GetNfts />
      </section>
    </section>
  );
}
