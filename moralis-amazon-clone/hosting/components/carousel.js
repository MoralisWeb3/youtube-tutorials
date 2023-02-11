import { Carousel } from "antd";
import Image from "next/image";
import styles from "../styles/Home.module.css";

import Carousel1 from "../public/assets/carousel/carousel1.jpg";
import Carousel2 from "../public/assets/carousel/carousel2.jpg";
import Carousel3 from "../public/assets/carousel/carousel3.jpg";

const carousel = [Carousel1, Carousel2, Carousel3];

export default function CarouselComp() {
  return (
    <section className={styles.carousel}>
      <Carousel autoplay dots={false}>
        {carousel.map((i) => {
          return <Image src={i} alt="carousel" key={i} height="650" />;
        })}
      </Carousel>
    </section>
  );
}
