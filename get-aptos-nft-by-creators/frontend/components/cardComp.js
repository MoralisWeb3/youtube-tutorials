import styles from "@/styles/Home.module.css";
import { Card } from "@web3uikit/core";
import { useState, useEffect } from "react";
import axios from "axios";

export default function CardComp(props) {
  const [displayIpfsNft, setDisplayIpfsNft] = useState(false);
  const [nftImage, setNftImage] = useState("");

  useEffect(() => {
    if (props.nft.metadata_uri.includes("ipfs")) {
      const getIpfsNft = async () => {
        const ipfsJson = `https://ipfs.io/ipfs/${
          props.nft.metadata_uri.split("ipfs://")[1]
        }`;

        const response = await axios.get(ipfsJson);
        setDisplayIpfsNft(true);
        setNftImage(response.data.image.replace(/['"]+/g, ""));
      };
      getIpfsNft();
    }
  });
  return (
    <section className={styles.cardComp}>
      <Card
        onClick={function noRefCheck() {}}
        setIsSelected={function noRefCheck() {}}
        title={props.nft.collection_name}
        description={
          props.nft.description ? props.nft.description : props.nft.name
        }
      >
        <img
          src={displayIpfsNft ? nftImage : props.nft.metadata_uri}
          className={styles.nftImage}
        />
      </Card>
    </section>
  );
}
