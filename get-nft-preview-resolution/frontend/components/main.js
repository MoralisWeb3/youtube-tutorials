import { useState } from "react";
import axios from "axios";
import { Card } from "@web3uikit/core";
import styles from "../styles/Home.module.css";

import CardComp from "./card.js";

export default function Main() {
  const [showPreviewResult, setShowPreviewResult] = useState(false);
  const [showMetadataResult, setShowMetadataResult] = useState(false);
  const [result, setResult] = useState([]);

  const getPreviewNfts = async () => {
    const response = await axios.get("http://localhost:5001/getpreview");

    console.log("preview", response.data.result);
    setResult(response.data.result);
    setShowMetadataResult(false);
    setShowPreviewResult(true);
  };

  const getMetadataNfts = async () => {
    const response = await axios.get("http://localhost:5001/getmetadata");

    console.log("metadata", response.data.result);
    setResult(response.data.result);
    setShowPreviewResult(false);
    setShowMetadataResult(true);
  };

  return (
    <section className={styles.main}>
      <section className={styles.main_form}>
        <button className={styles.submit_btn} onClick={getPreviewNfts}>
          GET PREVIEW NFTS
        </button>
        <button className={styles.submit_btn} onClick={getMetadataNfts}>
          GET METADATA NFTS
        </button>
      </section>
      <section className={styles.resultContainer}>
        {showPreviewResult &&
          result.map((nft, i) => {
            if (nft.media?.media_collection) {
              return (
                <section className={styles.resultSection} key={i}>
                  <section className={styles.card}>
                    <Card
                      onClick={function noRefCheck() {}}
                      setIsSelected={function noRefCheck() {}}
                      title={nft.name}
                    >
                      <img
                        src={nft.media?.media_collection?.low.url}
                        className={styles.nftImage}
                      />
                    </Card>
                  </section>
                </section>
              );
            }
          })}
        {showMetadataResult &&
          result.map((nft, i) => {
            return nft.metadata && <CardComp uri={nft} key={i} />;
          })}
      </section>
    </section>
  );
}
