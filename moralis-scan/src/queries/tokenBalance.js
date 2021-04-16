import { tokenValueTxt } from "./utils";

export const processTokenBalance = (t) => ({
  name: `${t.name} (${t.symbol})`,
  symbol: t.symbol,
  decimals: t.decimals,
  balance: tokenValueTxt(t.balance, t.decimals, t.symbol),
});
