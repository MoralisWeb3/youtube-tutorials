import Image from "next/image";
import styles from "../styles/Home.module.css";

import AptosLogo from "../public/assets/AptosLogo.png";

export default function Header() {
  return (
    <section className={styles.header}>
      <Image src={AptosLogo} alt="aptos logo image" width="85" height="80" />
      <h1 className={styles.title}>Aptos Transactions</h1>
    </section>
  );
}
