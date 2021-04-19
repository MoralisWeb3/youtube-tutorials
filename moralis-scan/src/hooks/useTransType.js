import { useEffect, useState } from "react"
import { useParams } from "react-router";
import { processTokenBalance } from "../queries/tokenBalance";
import { processTokenTransfer } from "../queries/tokenTransfer";
import { processTransaction } from "../queries/transactions";

const TransTypeOptions = {
  main: {
    cols: [
      { colName: "Txn Hash", key: "hash" },
      { colName: "Block", key: "block_number" },
      { colName: "Age", key: "ago" },
      { colName: "From", key: "from_address" },
      { colName: "To", key: "to_address" },
      { colName: "Value", key: "value" },
      { colName: "Txn Fee", key: "gas_price" },
    ],
    methodName: "getTransactions",
    postProcess: processTransaction,
  },
  erc20: {
    cols: [
      { colName: "Txn Hash", key: "transaction_hash" },
      { colName: "Age", key: "ago" },
      { colName: "From", key: "from_address" },
      { colName: "To", key: "to_address" },
      { colName: "Value", key: "value" },
      { colName: "Token", key: "name" },
    ],
    methodName: "getTokenTranfers",
    postProcess: processTokenTransfer,
  },
  tokenBalance: {
    cols: [
      { colName: "Name", key: "name" },
      { colName: "Balance", key: "balance" },
    ],
    methodName: "getTokenBalances",
    postProcess: processTokenBalance,
    // itemName: "tokens",
  },
};

export const useTransType = () => {
  const [state, setState] = useState({});
  const { transType } = useParams();

  useEffect(()=>{
    setState(TransTypeOptions[transType])
  }, [transType])

  return state;
}
