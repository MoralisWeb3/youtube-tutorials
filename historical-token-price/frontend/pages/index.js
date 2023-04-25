import styles from "../styles/Home.module.css";

import Header from "../components/header.js";
import Main from "../components/main.js";

export default function Home() {
  return (
    <section className={styles.container}>
      <Header />
      <Main />
    </section>
  );
}
