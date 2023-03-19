import { useEffect, useState } from "react";
import axios from "axios";
import Image from "next/image";
import Link from "next/link";
import { Arrow, Eth } from "@web3uikit/icons";
import styles from "@/styles/Home.module.css";

import BAYC from "../public/assets/blurBackground.png";
import ChecksVVOriginals from "../public/assets/checksVVOriginals.png";
import Azuki from "../public/assets/azuki.png";
import PudgyPenguins from "../public/assets/pudgyPenguins.png";
import Moonbirds from "../public/assets/moonbirds.png";

import BAYCProfile from "../public/assets/bayc_profile.webp";
import ChecksVVOriginalsProfile from "../public/assets/checksbboriginals_profile.webp";
import AzukiProfile from "../public/assets/azuki_profile.webp";
import PudgyPenguinsProfile from "../public/assets/pudgypenguins_profile.webp";
import MoonbirdsProfile from "../public/assets/moonbirds_profile.webp";

export default function HeroSection() {
  const [oneDayTradingVolume, setOneDayTradingVolume] = useState("");

  useEffect(() => {
    const yesterday = new Date(new Date().getTime() - 24 * 60 * 60 * 1000);
    const getNftData = async () => {
      const response = await axios.get("http://localhost:5001/getnftdata", {
        params: {
          contractAddress: "0xBC4CA0EdA7647A8aB7C2061c2E118A18a936f13D",
        },
      });

      let tradingVolume = 0;

      response.data.result.map((sale) => {
        if (sale.block_timestamp > yesterday.toISOString()) {
          return (tradingVolume += sale.price / 10 ** 18);
        }
      });
      setOneDayTradingVolume(tradingVolume);
    };

    getNftData();
  });

  return (
    <section className={styles.heroSection}>
      <h2>Bored Ape Yacht Club</h2>
      <p>BY YUGALABS</p>
      <section className={styles.bayc_data}>
        <section>
          <p className={styles.bayc_data__title}>FLOOR PRICE</p>
          <p>
            67.90
            <Eth fontSize="17px" />
          </p>
        </section>
        <section>
          <p className={styles.bayc_data__title}>1D VOLUME</p>
          <p className={styles.collection_oneDayTradingVolume}>
            {Number(oneDayTradingVolume).toFixed(2)}
            <Eth fontSize="17px" />
          </p>
        </section>
        <button className={styles.viewCollection_btn}>
          VIEW COLLECTION
          <Arrow fontSize="20px" className={styles.arrow} />
        </button>
      </section>
      <section className={styles.carousel}>
        <section className={styles.carousel_section}>
          <Image
            src={BAYC}
            alt="bored ape yacht club"
            width=""
            height=""
            className={styles.carousel_img}
          />
          <Image
            src={BAYCProfile}
            alt="bored ape yacht club profile"
            width=""
            height=""
            className={styles.carousel_profile}
          />
          <p>Bored Ape Yacht Club</p>
        </section>
        <section className={styles.carousel_section}>
          <Image
            src={ChecksVVOriginals}
            alt="checks vv originals"
            width=""
            height=""
            className={styles.carousel_img}
          />
          <Image
            src={ChecksVVOriginalsProfile}
            alt="checks vv originals profile"
            width=""
            height=""
            className={styles.carousel_profile}
          />
          <p>Checks VV Originals</p>
        </section>
        <Link href="/viewcollection">
          <section className={styles.carousel_section}>
            <Image
              src={Azuki}
              alt="azuki"
              width=""
              height=""
              className={styles.carousel_img}
            />
            <Image
              src={AzukiProfile}
              alt="azuki profile"
              width=""
              height=""
              className={styles.carousel_profile}
            />
            <p>Azuki</p>
          </section>
        </Link>
        <section className={styles.carousel_section}>
          <Image
            src={PudgyPenguins}
            alt="pudgy penguins"
            width=""
            height=""
            className={styles.carousel_img}
          />
          <Image
            src={PudgyPenguinsProfile}
            alt="pudgy penguins profile"
            width=""
            height=""
            className={styles.carousel_profile}
          />
          <p>Pudgy Penguins</p>
        </section>
        <section className={styles.carousel_section}>
          <Image
            src={Moonbirds}
            alt="moonbirds"
            width=""
            height=""
            className={styles.carousel_img}
          />
          <Image
            src={MoonbirdsProfile}
            alt="moonbirds profile"
            width=""
            height=""
            className={styles.carousel_profile}
          />
          <p>Moonbirds</p>
        </section>
      </section>
    </section>
  );
}
