import axios from "axios";
import { useState, useEffect } from "react";
import { Button, Card } from "antd";
import styles from "./styles/Home.module.css";
import Logo from "./img/Moralis_logo.png";
import Monsters from "./img/Monstermons.png";
import { ethers } from "ethers";
import contractAddress from "./chain-info/deployments/map.json";
import contractABI from "./chain-info/contracts/RandomNFT.json";
import "./App.css";

function App() {
  const { Meta } = Card;
  const [isConnected, setIsConnected] = useState(false);
  const [account, setAccount] = useState();
  const [signer, setSigner] = useState();
  const [nfts, setNfts] = useState();

  const contract = contractAddress["5"]["RandomNFT"][0];
  const abi = contractABI.abi;

  useEffect(() => {
    console.log(account);
  }, [account]);

  async function handleConnect() {
    try {
      const provider = new ethers.providers.Web3Provider(window.ethereum);
      const acc = await provider.send("eth_requestAccounts", []);
      const sign = provider.getSigner(acc[0]);
      setIsConnected(true);
      setAccount(acc[0]);
      setSigner(sign);
    } catch (err) {
      console.error(err);
    }
  }

  async function handleMint() {
    const nftContract = new ethers.Contract(contract, abi, signer);
    const value = await nftContract.getMintFee();
    let tx = await nftContract.requestNft({ gasLimit: 1000000, value: value });
    await tx.wait(1);
  }

  const refreshNFTs = async () => {
    await axios
      .get(`/get_owners?address=${contract}`)
      .then((res) => {
        setNfts(res.data.result);
        console.log(res.data.result);
      })
      .catch((err) => console.log(err));
  };

  const renderedNFTS =
    nfts &&
    Object.values(nfts).map((nft) => {
      if (nft.owner_of === account) {
        return (
          <div className="nfts">
            <Card
              className="horizontal-card result-card"
              hoverable
              cover={
                <img alt="example" src={nft?.normalized_metadata?.image} />
              }
            >
              <Meta
                title={nft?.normalized_metadata?.name}
                description={nft?.token_address}
              />

              <p>
                <b>Token id:</b> {nft?.token_id}
              </p>
              <p>{nft?.normalized_metadata?.description}</p>
              <p>
                <b>Traits</b>
              </p>
              <div className="traits">
                <p>
                  <b>Type: </b>
                  {nft?.normalized_metadata?.attributes[0]?.value}
                </p>
                <p>
                  <b>Skill: </b>
                  {nft?.normalized_metadata?.attributes[1]?.value}
                </p>
                <p>
                  <b>HP: </b>
                  {nft?.normalized_metadata?.attributes[2]?.value}
                </p>
              </div>
            </Card>
          </div>
        );
      }
    });

  // const renderedPrice = nftPrice && (
  //   <Card
  //     hoverable
  //     className="result-card"
  //     title={nftPrice.marketplace_address}
  //     extra={
  //       <p>
  //         <b>Token Id:</b> {nftPrice.token_ids[0]}
  //       </p>
  //     }
  //     style={{ width: 300 }}
  //   >
  //     <div className="result-element">
  //       <p className="result-begin">Seller:</p>
  //       <p className="result-end">{nftPrice.seller_address}</p>
  //     </div>
  //     <div className="result-element">
  //       <p className="result-begin">Buyer:</p>
  //       <p className="result-end">{nftPrice.buyer_address}</p>
  //     </div>
  //     <p className="result-begin" style={{ fontSize: 16 }}>
  //       Transaction Hash:
  //     </p>
  //     <div className="result-element">
  //       <p className="result-end">{nftPrice.transaction_hash}</p>
  //     </div>
  //   </Card>
  // );

  return (
    <div>
      <div className={styles.header}>
        <div className={styles.moralis_logo}>
          <img src={Logo} alt="Logo" width="102" height="82" />
        </div>
        \
        <button
          onClick={handleConnect}
          disabled={isConnected}
          className={styles.connect_button}
        >
          {isConnected ? "Connected" : "Connect Wallet"}
        </button>
      </div>
      <div className={styles.main}>
        <div className={styles.hero_text}>
          <h1 className={styles.title}>
            Mint Your Random <br />
            NFT Now!{" "}
          </h1>
          <div className={styles.btns}>
            <button className={styles.form_btn} onClick={handleMint}>
              Mint!
            </button>
            <button className={styles.form_btn} onClick={refreshNFTs}>
              Get My NFTs
            </button>
          </div>
        </div>
        <div>
          <img src={Monsters} alt="xd" width="850px" />
        </div>
      </div>
      <div className="results">
        {nfts && <div className="nfts">{renderedNFTS}</div>}
      </div>
    </div>
  );
}

export default App;
