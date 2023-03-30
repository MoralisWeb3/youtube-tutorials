import Image from "next/image";
import styles from "../styles/Home.module.css";

import MoralisLogo from "../public/assets/moralis.png";
import AptosLogo from "../public/assets/aptos_white.png";
import GasPump from "../public/assets/gas_pump.png";

export default function Header() {
  return (
    <section className={styles.header}>
      <Image
        src={MoralisLogo}
        alt="Moralis logo image"
        width="102"
        height="82"
      />
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
      <Image src={AptosLogo} alt="Aptos logo image" width="82" height="82" />
      <h1 className={styles.title}>Estimate Gas Price</h1>
      <Image
        src={GasPump}
        alt="Gas pump image"
        width="45"
        height="45"
        className={styles.gas}
      />
    </section>
  );
}
