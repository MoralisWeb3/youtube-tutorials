import React from "react";
import { useState, useEffect } from "react";
import "../App.css";

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
    <div className="totalValue">
      <h3>Portfolio Total Value</h3>
      <h2>
       ${totalValue}
      </h2>
    </div>
    </>
  );
}

export default PortfolioValue;
