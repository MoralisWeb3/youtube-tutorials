import React from "react";
import { useState, useEffect } from "react";

function PortfolioValue({ tokens, nativeValue }) {
  const [totalValue, setTotalValue] = useState(0);


  useEffect(() => {
    let val = 0;
    for (let i = 0; i < tokens.length; i++) {
      val = val + Number(tokens[i].val);
    }
    val = val + Number(nativeValue);

    setTotalValue(val.toFixed(2));
  }, [nativeValue, tokens]);

  return (
    <>
      <h1>Portfolio Total Value</h1>
      <p>
        <span>Total Balance: ${totalValue}</span>
      </p>
    </>
  );
}

export default PortfolioValue;
