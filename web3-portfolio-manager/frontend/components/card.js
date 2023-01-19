import { useState } from "react";
import styles from "../styles/Home.module.css";
import { Card, Illustration } from "@web3uikit/core";

export default function Card1(props) {
  const [nft, setNft] = useState(JSON.parse(props.uri.metadata));
  const [nftImage, setNftImage] = useState(() => {
    if (nft?.image) {
      return nft.image.includes("ipfs")
        ? `https://ipfs.io/ipfs/${nft.image.split("ipfs://")[1]}`
        : nft.image.split("\\")[0];
    }
  });
  let name = nft?.name ? nft.name : "No NFT title can be shown.";

  return (
    <section className={styles.cardContainer}>
      <Card
        onClick={function noRefCheck() {}}
        setIsSelected={function noRefCheck() {}}
        title={name}
      >
        <div>
          {nftImage ? <img src={nftImage} /> : <Illustration logo="lazyNft" />}
        </div>
      </Card>
    </section>
  );
}
