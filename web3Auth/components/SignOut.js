import { useMoralis } from "react-moralis";
import signOutStyle from "../styles/SignOut.module.css";
import styles from "../styles/Home.module.css";
import { useEffect, useState } from "react";

export const SignOut = () => {
  const { logout, Moralis, user } = useMoralis();
  const [balance, setBalance] = useState(0);
  const fetchBalance = async () => {
    try {
      const options = { chain: Moralis.Chains.ETH_ROPSTEN };
      const balance = await Moralis.Web3API.account.getNativeBalance(options);
      setBalance(balance.balance / 10 ** 18);
    } catch {}
  };
  useEffect(() => {
    fetchBalance();
  }, []);

  const handleTransfer = async () => {
    try {
      await Moralis.transfer({
        amount: Moralis.Units.ETH("0.1"),
        receiver: "0xd2305b8155c4710c7fff1358d084f23959c999f3",
        type: "native",
      }).then((e) => {
        alert("sucesfully transfered");
      });
      await fetchBalance();
    } catch {}
  };

  return (
    <div className={signOutStyle.signOutCard}>
      <h4>Welcome To Moralis x Web3Auth!</h4>
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
