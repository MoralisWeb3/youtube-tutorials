import React, { useMemo } from 'react'
import { processTransaction } from '../queries/transactions';
import { useParams } from 'react-router';
import { usePagination } from '../hooks/pagination';

const cols = [
  { colName: "Txn Hash", key: "hash" },
  { colName: "Block", key: "block_number" },
  { colName: "Age", key: "ago" },
  { colName: "From", key: "from_address" },
  { colName: "To", key: "to_address" },
  { colName: "Value", key: "value" },
  { colName: "Txn Fee", key: "gas_price" },
];

export default function TransResults() {
  const {address: userAddress} = useParams();
  const options = useMemo(()=> ({
    postProcess: processTransaction,
  }), []);
  const { results, error, loading } = usePagination("getTransactions", userAddress, options);

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
                <td key={col.colName}>{t[col.key]}</td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
