import { useState } from "react";
import { useContract, useProvider, useSigner } from "wagmi";
import { Card, Illustration } from "@web3uikit/core";
import styles from "../styles/Home.module.css";
import { abi, NFTMarketplace_CONTRACT_ADDRESS } from "../contracts/index.js";

export default function CardComp(props) {
  const provider = useProvider();
  const { data: signer } = useSigner();
  const [nftPrice, setNftPrice] = useState(0);
  const [nft, setNft] = useState(JSON.parse(props.uri.metadata));
  const [nftImage, setNftImage] = useState(() => {
    if (nft?.image) {
      return nft.image.include("ipfs")
        ? `https://ipfs.io/ipfs/${nft.image.split("ipfs://")[1]}`
        : nft.image.split("\\")[0];
    }
  });

  const NftMarketplace = useContract({
    address: NFTMarketplace_CONTRACT_ADDRESS,
    abi: abi,
    signerOrProvider: signer || provider,
  });

  const handleChange = (e) => {
    setNftPrice(Number(e.target.value));
  };

  const handleSubmit = async () => {
    try {
      const tx = await NftMarketplace.createMarketItem(
        "0x6017c7326F37F1acA0eF204Ccf9Fc5B6F27D891a",
        props.uri.token_id,
        nftPrice
      );
      await tx.wait();
      console.log("Request Completed");
    } catch (error) {
      console.log(error);
    }
  };

  return (
    <section className={styles.cardContainer}>
      <Card
        onClick={function noRefCheck() {}}
        setIsSelected={function noRefCheck() {}}
        title={nft.name}
      >
        <section>
          {nftImage ? <img src={nftImage} /> : <Illustration logo="lazyNft" />}
        </section>
      </Card>
      <section className={styles.sellSection}>
        <input
          placeholder="amount"
          disabled=""
          type="number"
          className={styles.inputField_amount}
          onChange={handleChange}
        />
        <button className={styles.sell_btn} onClick={handleSubmit}>
          LIST FOR SALE
        </button>
      </section>
    </section>
  );
}
