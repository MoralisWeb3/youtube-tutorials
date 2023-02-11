import Image from "next/image";
import Link from "next/link";
import styles from "../styles/Home.module.css";

import Books from "../public/assets/books.jpg";
import EthToken from "../public/assets/ethtoken.jpg";
import Fitness from "../public/assets/fitness.jpg";
import KitchenEssentials from "../public/assets/kitchenEssentials.jpg";
import Fashion from "../public/assets/fashion.jpg";
import Outdoor from "../public/assets/outdoor.jpg";
import PersonalCare from "../public/assets/personalCare.jpg";
import PetCare from "../public/assets/petCare.jpg";

export default function LandingPage() {
  return (
    <section className={styles.landingpage}>
      <section className={styles.landingpage_container}>
        <section className={styles.previwContainer}>
          <section className={styles.previewSection}>
            <p className={styles.previewTitle}>Explore Books</p>
            <Image
              src={Books}
              alt="Books image"
              width=""
              height=""
              className={styles.previewImage}
            />
            <p className={styles.previewAction}>See More</p>
          </section>
          <section className={styles.previewSection}>
            <p className={styles.previewTitle}>ETH Token</p>
            <Image
              src={EthToken}
              alt="Eth Token image"
              width=""
              height=""
              className={styles.previewImage}
            />
            <Link href="/product" className={styles.previewAction}>
              Shop Now
            </Link>
          </section>
          <section className={styles.previewSection}>
            <p className={styles.previewTitle}>For Your Fitness Needs</p>
            <Image
              src={Fitness}
              alt="Fitness image"
              width=""
              height=""
              className={styles.previewImage}
            />
            <p className={styles.previewAction}>Shop Now</p>
          </section>
          <section className={styles.previewSection}>
            <p className={styles.previewTitle}>Kitchen Essentials</p>
            <Image
              src={KitchenEssentials}
              alt="Kitchen Essentials image"
              width=""
              height=""
              className={styles.previewImage}
            />
            <p className={styles.previewAction}>See More</p>
          </section>
        </section>
        <section className={styles.previwContainer}>
          <section className={styles.previewSection}>
            <p className={styles.previewTitle}>Fashion For All</p>
            <Image
              src={Fashion}
              alt="Fashion image"
              width=""
              height=""
              className={styles.previewImage}
            />
            <p className={styles.previewAction}>Shop Now</p>
          </section>
          <section className={styles.previewSection}>
            <p className={styles.previewTitle}>Outdoor Tools</p>
            <Image
              src={Outdoor}
              alt="Outdoor image"
              width=""
              height=""
              className={styles.previewImage}
            />
            <p className={styles.previewAction}>Explore Now</p>
          </section>
          <section className={styles.previewSection}>
            <p className={styles.previewTitle}>Health & Personal Care</p>
            <Image
              src={PersonalCare}
              alt="Personal Care image"
              width=""
              height=""
              className={styles.previewImage}
            />
            <p className={styles.previewAction}>Shop Now</p>
          </section>
          <section className={styles.previewSection}>
            <p className={styles.previewTitle}>Pet Care</p>
            <Image
              src={PetCare}
              alt="Pet Care image"
              width=""
              height=""
              className={styles.previewImage}
            />
            <p className={styles.previewAction}>See More</p>
          </section>
        </section>
      </section>
    </section>
  );
}
