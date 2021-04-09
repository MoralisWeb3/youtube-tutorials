import React from 'react'
import { processTransaction } from '../queries/transactions';
import { useMoralisCloudQuery } from '../hooks/cloudQuery';

const cols = [
  { colName: "Txn Hash", key: "hash" },
  { colName: "Block", key: "block_number" },
  { colName: "Age", key: "ago" },
  { colName: "From", key: "from_address" },
  { colName: "To", key: "to_address" },
  { colName: "Value", key: "value" },
  { colName: "Txn Fee", key: "gas_price" },
];

const options = {
  params: { userAddress: "0x29781d9fca70165cbc952b8c558d528b85541f0b" },
  postProcess: processTransaction,
};

export default function TransResults() {
  const { data: results, error, loading } = useMoralisCloudQuery("getTransactions", options);

  if (!results) {
    return null;
  }
  
  return (
    <div>
      <table className="table">
        <thead className="thead-light">
          <tr>
            {cols.map((col) => (
              <th scope="col" key={col.colName}>
                {col.colName}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {results.map((t, i) => (
            <tr key={i}>
              {cols.map((col) => (
                <td>{t[col.key]}</td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
