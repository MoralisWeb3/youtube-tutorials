import Image from "next/image";
import { useEffect, useState } from "react";
import { Modal } from "antd";
import { ethers } from "ethers";
import axios from "axios";
import { useFirebase } from "./FirebaseInitializer";
import { app } from "./firebase";
import { getFirestore, collection, addDoc } from "firebase/firestore";
import styles from "../styles/Home.module.css";

import EthToken from "../public/assets/ethtoken.jpg";

export default function ProductComp() {
  const { auth } = useFirebase();
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [delivery, setDelivery] = useState("");
  const [usdPrice, setUsdPrice] = useState(0);
  const toAddress = "0x77cFF9A3467cf7F12b0Cbd32E5Ad1375f5E070a3";
  const price = 9.99;

  const database = getFirestore(app);
  const dbInstance = collection(database, "purchase");

  useEffect(() => {
    async function getPrice() {
      const response = await axios.get("http://localhost:5001/ethtoken");
      setUsdPrice(response.data.usdPrice.toFixed(2));
    }
    getPrice();
  }, []);

  const getDeliveryAddress = async (e) => {
    setDelivery(e.target.value);
  };

  const handleOk = async () => {
    const provider = new ethers.providers.Web3Provider(window.ethereum);
    await provider.send("eth_requestAccounts", []);
    const signer = provider.getSigner();
    const tx = signer.sendTransaction({
      to: toAddress,
      value: ethers.utils.parseEther((price / usdPrice).toString()),
    });
    addDoc(dbInstance, {
      purchasedBy: auth.currentUser?.displayName,
      deliveryAddress: delivery,
      productPurchased: "Eth Token",
    });
    setIsModalVisible(false);
  };

  return (
    <section className={styles.main}>
      <section className={styles.productContainer}>
        <section>
          <Image
            src={EthToken}
            alt="Eth Token image"
            width="600"
            height="600"
          />
        </section>
        <section className={styles.productInfoSection}>
          <section className={styles.productInfoTitleSection}>
            <h3>
              Ethereum Coins-Protective Collectible Gifts. | Blockchain
              Cryptocurrency | with Original Commemorative Tokens | Chase Coin
            </h3>
            <p className={styles.visitStore}>Visit The Crypto Store</p>
            <section className={styles.productRating}>
              <section className={styles.productStars}>
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 24 24"
                  fill="currentColor"
                  className="w-6 h-6"
                >
                  <path
                    fillRule="evenodd"
                    d="M10.788 3.21c.448-1.077 1.976-1.077 2.424 0l2.082 5.007 5.404.433c1.164.093 1.636 1.545.749 2.305l-4.117 3.527 1.257 5.273c.271 1.136-.964 2.033-1.96 1.425L12 18.354 7.373 21.18c-.996.608-2.231-.29-1.96-1.425l1.257-5.273-4.117-3.527c-.887-.76-.415-2.212.749-2.305l5.404-.433 2.082-5.006z"
                    clipRule="evenodd"
                  />
                </svg>
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 24 24"
                  fill="currentColor"
                  className="w-6 h-6"
                >
                  <path
                    fillRule="evenodd"
                    d="M10.788 3.21c.448-1.077 1.976-1.077 2.424 0l2.082 5.007 5.404.433c1.164.093 1.636 1.545.749 2.305l-4.117 3.527 1.257 5.273c.271 1.136-.964 2.033-1.96 1.425L12 18.354 7.373 21.18c-.996.608-2.231-.29-1.96-1.425l1.257-5.273-4.117-3.527c-.887-.76-.415-2.212.749-2.305l5.404-.433 2.082-5.006z"
                    clipRule="evenodd"
                  />
                </svg>
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 24 24"
                  fill="currentColor"
                  className="w-6 h-6"
                >
                  <path
                    fillRule="evenodd"
                    d="M10.788 3.21c.448-1.077 1.976-1.077 2.424 0l2.082 5.007 5.404.433c1.164.093 1.636 1.545.749 2.305l-4.117 3.527 1.257 5.273c.271 1.136-.964 2.033-1.96 1.425L12 18.354 7.373 21.18c-.996.608-2.231-.29-1.96-1.425l1.257-5.273-4.117-3.527c-.887-.76-.415-2.212.749-2.305l5.404-.433 2.082-5.006z"
                    clipRule="evenodd"
                  />
                </svg>
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 24 24"
                  fill="currentColor"
                  className="w-6 h-6"
                >
                  <path
                    fillRule="evenodd"
                    d="M10.788 3.21c.448-1.077 1.976-1.077 2.424 0l2.082 5.007 5.404.433c1.164.093 1.636 1.545.749 2.305l-4.117 3.527 1.257 5.273c.271 1.136-.964 2.033-1.96 1.425L12 18.354 7.373 21.18c-.996.608-2.231-.29-1.96-1.425l1.257-5.273-4.117-3.527c-.887-.76-.415-2.212.749-2.305l5.404-.433 2.082-5.006z"
                    clipRule="evenodd"
                  />
                </svg>
              </section>
              <p className={styles.rating}>512 ratings</p>
            </section>
          </section>
          <section className={styles.productInfoPriceSection}>
            <p className={styles.price}>${price}</p>
            <p className={styles.importAndShipping}>
              No Import Fees Deposit & $10 Shipping World Wide
            </p>
          </section>
          <section className={styles.productInfoAboutSection}>
            <h4>About this Item</h4>
            <ul>
              <li>
                💰VALUABLE PACKAGE : Each set of 3 original collection coins,
                the color is gold. Each coin is packed in a plastic bag and
                packed in an acrylic coin box to ensure perfect condition upon
                delivery
              </li>
              <li>
                💰 PREMIUM MATERIAL : Every coin is printed with attention to
                detail. This luxurious gift is made of strong zinc alloy metal.
              </li>
              <li>
                💰Perfect Gift - This collectible is great for any Ethereum or
                Blockchain fan. Each Ethereum coin comes in its own plastic
                display case to ensure that it arrives scratch and damage free.
                Makes a great business gift, lucky charm, or conversation. Must
                have memorabilia piece for every Ethereum enthusiast
              </li>
              <li>
                💰SUITABLE SIZE : These coins are nearly 2x the diameter of a
                quarter. This shiny, deluxe collectible item is perfect for any
                crypto fan or Ethereum miner. Beautiful souvenir or novelty
                piece. Perfect challenge coin, conversation starter or
                geocaching item
              </li>
              <li>
                💰100% Satisfaction Guaranteed - We take pride in the quality of
                our cryptocurrency coins and know you'll be thrilled. If you
                don't like it, we accept returns and exchanges at any time.
              </li>
            </ul>
          </section>
        </section>
        <section className={styles.productPurchaseSection}>
          <p className={styles.price}>${price}</p>
          <p className={styles.ImportInPurchaseSection}>
            No Import Fees Deposit & $10 Shipping World Wide
          </p>
          <section className={styles.stockForm}>
            <label className={styles.label} htmlFor="inputField">
              In Stock.
            </label>
            <input
              className={styles.stockInputField}
              type="text"
              id="inputField"
              name="inputField"
              maxLength="120"
              placeholder="Qty: 1"
              required
            />
          </section>
          <section className={styles.purchaseButtons}>
            <button className={styles.addToCartBtn}>Add to Cart</button>
            <button
              className={styles.buyNowBtn}
              onClick={() => setIsModalVisible(true)}
            >
              Buy Now
            </button>
          </section>
          <section className={styles.secureTransaction}>
            <section>
              <svg
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 24 24"
                fill="currentColor"
                className="w-6 h-6"
              >
                <path
                  fillRule="evenodd"
                  d="M12 1.5a5.25 5.25 0 00-5.25 5.25v3a3 3 0 00-3 3v6.75a3 3 0 003 3h10.5a3 3 0 003-3v-6.75a3 3 0 00-3-3v-3c0-2.9-2.35-5.25-5.25-5.25zm3.75 8.25v-3a3.75 3.75 0 10-7.5 0v3h7.5z"
                  clipRule="evenodd"
                />
              </svg>
            </section>
            <p>Secture Transaction</p>
          </section>
        </section>
      </section>
      <Modal
        title="Purchase Product"
        open={isModalVisible}
        onOk={handleOk}
        onCancel={() => setIsModalVisible(false)}
        width={600}
      >
        <section className={styles.modal}>
          <Image
            src={EthToken}
            alt="Eth Token image"
            width="300"
            height="300"
          />
          <section className={styles.modalInfo}>
            <h3>
              Ethereum Coins-Protective Collectible Gifts. | Blockchain
              Cryptocurrency | with Original Commemorative Tokens | Chase Coin
            </h3>
            <p className={styles.modalPrice}>$9.99</p>
            <label className={styles.modalLabel} htmlFor="inputField">
              Delivery Address
            </label>
            <input
              className={styles.modalInputField}
              type="text"
              id="inputField"
              name="inputField"
              maxLength="120"
              placeholder="Delivery Address"
              required
              onChange={getDeliveryAddress}
            />
          </section>
        </section>
      </Modal>
    </section>
  );
}
