import { useEffect, useState } from "react";
import axios from "axios";
import Image from "next/image";
import { utils } from "ethers";
import {
  useAccount,
  useContract,
  useContractWrite,
  usePrepareContractWrite,
  useProvider,
  useSigner,
} from "wagmi";
import { Eth } from "@web3uikit/icons";
import styles from "@/styles/Home.module.css";

import { abi, NFTMarketplace_CONTRACT_ADDRESS } from "../contracts/index.js";

export default function MMCollectionPurchaseSection() {
  const { address } = useAccount();
  const provider = useProvider();
  const { data: signer } = useSigner();
  const [metadata, setMetadata] = useState([]);
  const [showResult, setShowResult] = useState(false);
  const [id, setId] = useState(0);
  const [price, setPrice] = useState("");
  const [market, setMarket] = useState([]);

  const NftMarketplace = useContract({
    address: NFTMarketplace_CONTRACT_ADDRESS,
    abi: abi,
    signerOrProvider: signer || provider,
  });

  const { config } = usePrepareContractWrite({
    address: NFTMarketplace_CONTRACT_ADDRESS,
    abi: abi,
    overrides: {
      from: address,
      value: price,
    },
    functionName: "createMarketSale",
    args: ["0x6017c7326F37F1acA0eF204Ccf9Fc5B6F27D891a", id],
  });
  const { write } = useContractWrite(config);

  useEffect(() => {
    const getNftData = async () => {
      const response = await axios.get("http://localhost:5001/getcontractnft", {
        params: {
          contractAddress: "0x6017c7326F37F1acA0eF204Ccf9Fc5B6F27D891a",
          chain: "0x5",
        },
      });

      let first20 = [];

      response.data.result.map((nft, i) => {
        if (i < response.data.result.length) {
          first20.push(JSON.parse(nft.metadata));
        }
        setMetadata(first20);
      });

      setShowResult(true);
    };

    const getSell = async () => {
      try {
        const tx = await NftMarketplace.fetchMarketItems();
        setMarket(tx);
        console.log("Request Completed");
      } catch (error) {
        console.log(error);
      }
    };

    getSell();
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
            <th>{market.length} LISTED</th>
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
            market.map((nft, i) => {
              const topBid = Math.random() * (17.2 - 12.8) + 12.8;
              return (
                <tr
                  className={styles.mmCollectionPurchaseSection_tbody_row}
                  key={i}
                >
                  <td>
                    <input type="checkbox" />
                  </td>
                  <td
                    className={styles.purchase_btn}
                    onClick={() => {
                      setId(nft[0].toNumber());
                      setPrice(
                        (Number(utils.formatEther(nft.price)) * 1e18).toFixed(0)
                      );
                      write?.();
                    }}
                  >
                    <Image
                      src={`http://ipfs.io/ipfs/${
                        metadata[i]?.image.split("//")[1]
                      }`}
                      alt="nft image"
                      width="50"
                      height="50"
                      className={styles.collection_thumbnail}
                    />
                    {metadata[i]?.name}
                  </td>
                  <td className={styles.rarity}>
                    {Math.round(Math.random() * (10000 - 5000) + 5000)}
                  </td>
                  <td>
                    {Number(utils.formatEther(nft.price)).toFixed(0)}
                    <Eth fontSize="18px" />
                  </td>
                  <td>{(Math.random() * (17.6 - 12.34) + 12.34).toFixed(2)}</td>
                  <td>
                    {topBid.toFixed(2)}
                    <Eth fontSize="18px" />
                  </td>
                  <td>{nft.seller.slice(0, 6)}</td>
                  <td>{Math.round(Math.random() * (12 - 1) + 1)}</td>
                </tr>
              );
            })}
        </tbody>
      </table>
    </section>
  );
}
