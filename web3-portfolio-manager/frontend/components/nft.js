import { useEffect, useState } from "react";
import axios from "axios";
import styles from "../styles/Home.module.css";
import { useAccount } from "wagmi";
import Card1 from "./card.js";

export default function Nft() {
  const { address } = useAccount();
  const [nfts, setNfts] = useState([]);

  useEffect(() => {
    if (address) {
      async function getWalletNft() {
        const response = await axios
          .get("http://localhost:5001/getwalletnft", {
            params: { address, chain: "0x89" },
          })
          .then((response) => {
            setNfts(response.data.result);
            console.log(response);
          });
      }
      getWalletNft();
    } else {
      console.log("Not Connected");
    }
  }, [address]);

  return (
    <section className={styles.result_nft}>
      {nfts.map((nft) => {
        return nft.metadata && <Card1 uri={nft} key={nft.token_uri} />;
      })}
    </section>
  );
}
