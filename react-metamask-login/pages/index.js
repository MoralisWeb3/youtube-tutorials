import Login from "../components/Login";
import { useMoralis } from "react-moralis";

export default function Home() {
  const { isAuthenticated, logout } = useMoralis();
  return (
    <div>
      {isAuthenticated ? (
        <p>
          You are logged in
          <button onClick={logout}>Sign Out</button>
        </p>
      ) : (
        <Login />
      )}
    </div>
  );
}
