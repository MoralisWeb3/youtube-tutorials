import { useTransType } from "../hooks/useTransType";
import { useResultContext } from "./Paginator";

export default function TransResults() {
  const { results } = useResultContext();
  const { cols } = useTransType();
  if (!results || !cols) {
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
