import { useEffect, useState } from "react";
import axios from "axios";
import { useAccount } from "wagmi";
import styles from "../styles/Home.module.css";

import CardComp from "./card.js";

export default function getNfts() {
  const [nfts, setNfts] = useState([]);
  const { address } = useAccount();
  const chain = "0x5";

  useEffect(() => {
    let response;
    async function getData() {
      response = axios
        .get("http://localhost:5001/getnfts", {
          params: { address, chain },
        })
        .then((response) => {
          setNfts(response.data.result);
        });
    }
    getData();
  }, []);

  return (
    <section className={styles.dataContainer}>
      {nfts.map((nft) => {
        return nft.metadata && <CardComp uri={nft} key={nft.token_uri} />;
      })}
    </section>
  );
}
