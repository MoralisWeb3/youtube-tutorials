import React from "react";
import styles from "../styles/Home.module.css";

import CarouselComp from "./carousel";
import LandingPage from "./landingPage";

export default function Main() {
  return (
    <section className={styles.main}>
      <section>
        <CarouselComp />
        <LandingPage />
      </section>
    </section>
  );
}
