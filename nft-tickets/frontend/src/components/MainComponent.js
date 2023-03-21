import axios from "axios";
import { useState, useEffect } from "react";
import { Button, Card } from "antd";
import styles from "../styles/Home.module.css";
import Logo from "../img/Moralis_logo.png";
import Tickets from "../img/tickets.png";
import { ethers } from "ethers";
import contractAddress from "../chain-info/deployments/map.json";
import contractABI from "../chain-info/contracts/TicketNFT.json";
import { AccessVideoButton } from "./AccessVideoButton";
import { useVideoAccess } from "../hooks/useVideoAccess";

function MainComponent() {
  const { Meta } = Card;
  const [isConnected, setIsConnected] = useState(false);
  const [account, setAccount] = useState();
  const [signer, setSigner] = useState();
  const [provider, setProvider] = useState(null);

  const [nfts, setNfts] = useState();
  const tokenUri =
    "https://ipfs.moralis.io:2053/ipfs/QmXSEUVTfhurDUg4dYGm9t63DEcFoNGuMNNo6X1L87d6LT/metadata.json";

  const contract = contractAddress["11155111"]["TicketNFT"][0];
  const abi = contractABI.abi;

  const { hasAccess, checkAccess, navigateToVideo } = useVideoAccess(
    provider,
    signer,
    account,
    contract
  );

  useEffect(() => {
    checkAccess();
  }, [account]);

  async function handleConnect() {
    try {
      const provider = new ethers.providers.Web3Provider(window.ethereum);
      const acc = await provider.send("eth_requestAccounts", []);
      const sign = provider.getSigner(acc[0]);
      setProvider(provider);
      setIsConnected(true);
      setAccount(acc[0]);
      setSigner(sign);
    } catch (err) {
      console.error(err);
    }
  }

  async function handleMint() {
    const nftContract = new ethers.Contract(contract, abi, signer);
    let tx = await nftContract.mintTicket(tokenUri, {
      gasLimit: 1000000,
      value: ethers.utils.parseEther("0.01"),
    });
    await tx.wait(1);
  }

  const refreshNFTs = async () => {
    await axios
      .get(`/get_owners?address=${contract}`)
      .then((res) => {
        setNfts(res.data.result);
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

              <div className="traits">
                <p>
                  <b>Type: </b>
                  {nft?.normalized_metadata?.attributes[0]?.value}
                </p>
                <p>
                  <b>Genre: </b>
                  {nft?.normalized_metadata?.attributes[1]?.value}
                </p>
                <p>
                  <b>Reward Points: </b>
                  {nft?.normalized_metadata?.attributes[2]?.value}
                </p>
              </div>
              <div>
                <AccessVideoButton
                  provider={provider}
                  account={account}
                  signer={signer}
                  contractAddress={contract}
                />
              </div>
            </Card>
          </div>
        );
      }
    });

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
            Crypto Concerts & Events <br />
            Buy Now!{" "}
          </h1>
          <div>
            <div className={styles.btns}>
              <button className={styles.form_btn} onClick={handleMint}>
                Buy Tickets
              </button>

              <button className={styles.form_btn} onClick={refreshNFTs}>
                My tickets
              </button>
            </div>
          </div>
        </div>
        <div>
          <img src={Tickets} alt="xd" width="850px" />
        </div>
      </div>
      <div className="results">
        {nfts && <div className="nfts">{renderedNFTS}</div>}
      </div>
    </div>
  );
}

export default MainComponent;
