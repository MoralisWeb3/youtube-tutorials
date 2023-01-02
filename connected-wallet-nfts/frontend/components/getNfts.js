import { useAccount } from "wagmi";
import axios from "axios";
import { useEffect, useState } from "react";
import styles from "../styles/Home.module.css";
import Card from "./card.js";

export default function getNfts() {
  const [nfts, setNfts] = useState([]);
  const { address } = useAccount();
  const chain = "0x89";

  useEffect(() => {
    let response;
    async function getData() {
      response = await axios
        .get(`http://localhost:5001/getnfts`, {
          params: { address, chain },
        })
        .then((response) => {
          setNfts(response.data.result);
          console.log(response);
        });
    }
    getData();
  }, []);

  return (
    <section className={styles.dataContainer}>
      {nfts.map((nft) => {
        return nft.metadata && <Card uri={nft} key={nft.token_uri} />;
      })}
    </section>
  );
}
