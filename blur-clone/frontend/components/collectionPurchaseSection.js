import { useEffect, useState } from "react";
import axios from "axios";
import Image from "next/image";
import { Eth } from "@web3uikit/icons";
import styles from "@/styles/Home.module.css";

export default function CollectionPurchaseSection() {
  const [result, setResult] = useState([]);
  const [metadata, setMetadata] = useState([]);
  const [showResult, setShowResult] = useState(false);

  useEffect(() => {
    const getNftData = async () => {
      const response = await axios.get("http://localhost:5001/getcontractnft", {
        params: {
          contractAddress: "0xED5AF388653567Af2F388E6224dC7C4b3241C544",
        },
      });

      let first20 = [];

      response.data.result.map((nft, i) => {
        if (i < 20) {
          first20.push(JSON.parse(nft.metadata));
        }
        setMetadata(first20);
      });
      setResult(response.data.result);
      setShowResult(true);
    };

    getNftData();
  }, []);

  return (
    <section className={styles.collectionPurchaseSection}>
      <section className={styles.collectionPurchaseSection_titles}>
        <p>ITEMS</p>
        <p>BIDS</p>
      </section>
      <table className={styles.collectionPurchaseSection_table}>
        <thead className={styles.collectionPurchaseSection_thead}>
          <tr className={styles.collectionPurchaseSection_thead_row}>
            <th>
              <input type="checkbox" />
            </th>
            <th>1,102 LISTED</th>
            <th>RARITY</th>
            <th>BUY NOW</th>
            <th>LAST SALE</th>
            <th>TOP BID</th>
            <th>OWNER</th>
            <th>#HELD</th>
          </tr>
        </thead>
        <tbody className={styles.collectionPurchaseSection_tbody}>
          {showResult &&
            result.map((nft, i) => {
              if (i < 20) {
                console.log(metadata[i]);
                const topBid = Math.random() * (17.2 - 12.8) + 12.8;
                return (
                  <tr
                    className={styles.collectionPurchaseSection_tbody_row}
                    key={i}
                  >
                    <td>
                      <input type="checkbox" />
                    </td>
                    <td>
                      <Image
                        src={`http://ipfs.io/ipfs/${
                          metadata[i].image.split("//")[1]
                        }`}
                        alt="nft image"
                        width="50"
                        height="50"
                        className={styles.collection_thumbnail}
                      />
                      {metadata[i].name}
                    </td>
                    <td className={styles.rarity}>
                      {Math.round(Math.random() * (10000 - 5000) + 5000)}
                    </td>
                    <td>{Number(topBid + 0.34).toFixed(2)}</td>
                    <td>
                      {(Math.random() * (17.6 - 12.34) + 12.34).toFixed(2)}
                    </td>
                    <td>
                      {topBid.toFixed(2)}
                      <Eth fontSize="18px" />
                    </td>
                    <td>{nft.minter_address.slice(0, 6)}</td>
                    <td>{Math.round(Math.random() * (12 - 1) + 1)}</td>
                  </tr>
                );
              }
            })}
        </tbody>
      </table>
    </section>
  );
}
