import { useMoralis } from "react-moralis";
import signOutStyle from "../styles/SignOut.module.css";
import styles from "../styles/Home.module.css";
import { useEffect, useState } from "react";

export const SignOut = () => {
  const { logout, Moralis, user } = useMoralis();
  const [balance, setBalance] = useState(0);
  const fetchBalance = async () => {
    const options = { chain: Moralis.Chains.ETH_KOVAN };
    const balance = await Moralis.Web3API.account.getNativeBalance(options);
    console.log(balance.balance / 10 ** 18);
    setBalance(balance.balance / 10 ** 18);
  };
  useEffect(() => {
    fetchBalance();
  }, []);

  const handleTransfer = async () => {
    console.log("handle");
    await Moralis.transfer({
      amount: "1677",
      receiver: "0x2Eb7CDaD2E1a6A5626263D787E45dBd5455F505C",
      type: "native",
    }).then((e) => {
      alert("sucesfully transfered");
    });
    await fetchBalance();
  };

  return (
    <div className={signOutStyle.signOutCard}>
      <h4>Welcome To Moralis x Magic!</h4>
      <button className={`${signOutStyle.refresh}`} onClick={fetchBalance}>
        Refresh
      </button>
      <p className={signOutStyle.subHeader}>Details:</p>

      <div className={signOutStyle.detailsDiv}>
        <div>
          <h5>Account:</h5>
          <p>{user.attributes.accounts}</p>
        </div>
        <div>
          <h5>Balance (Eth)</h5>
          <p>{balance} </p>
        </div>
      </div>

      <div className={signOutStyle.fotter}>
        <button className={styles.loginButton} onClick={handleTransfer}>
          Test Transfer
        </button>
        <button className={styles.loginButton} onClick={logout}>
          Sign Out
        </button>
      </div>
    </div>
  );
};
