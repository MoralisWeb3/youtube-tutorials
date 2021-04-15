import { useResultContext } from "./Paginator";

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
  const { results } = useResultContext();
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
