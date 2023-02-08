import styles from "../styles/Home.module.css";

export default function Card(props) {
  return (
    <section className={styles.tableAsset}>
      <section className={styles.titleData}>
        <img src={props.token.walletBalance.thumbnail} />
        <section className={styles.nameAndSymbol}>
          {
            <span className={styles.tokenSymbol}>
              {props.token.walletBalance.symbol}
            </span>
          }
          {
            <span className={styles.tokenName}>
              {props.token.walletBalance.name}
            </span>
          }
        </section>
      </section>
      <section className={styles.portfolioValues}>
        <span>
          {`${(
            ((props.token.calculatedBalance * props.token.usdPrice) /
              props.total) *
            100
          ).toFixed(2)} %`}
        </span>
      </section>
      <section className={styles.price}>
        <span>{`$${props.token.usdPrice.toFixed(2)}`}</span>
      </section>
      <section className={styles.dollarAndAmount}>
        <span className={styles.dollarAmount}>{`$${(
          props.token.calculatedBalance * props.token.usdPrice
        ).toFixed(2)}`}</span>
        <span>{`${props.token.calculatedBalance} ${props.token.walletBalance.symbol}`}</span>
      </section>
    </section>
  );
}
