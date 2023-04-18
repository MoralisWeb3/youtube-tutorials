import Head from "next/head";
import styles from "../styles/Home.module.css";

import Header from "../components/header";
import Main from "../components/main";

export default function Home() {
  return (
    <section className={styles.container}>
      <main className={styles.main}>
        <Header />
        <Main />
      </main>
    </section>
  );
}
