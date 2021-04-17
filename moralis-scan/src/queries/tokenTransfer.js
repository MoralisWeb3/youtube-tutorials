import { agoTxt, getEllipsisTxt, tokenValueTxt } from "./utils";

export const processTokenTransfer = (r) => ({
    transaction_hash: getEllipsisTxt(r.transaction_hash),
    ago: agoTxt(r.block_timestamp.valueOf()),
    from_address: getEllipsisTxt(r.from_address),
    to_address: getEllipsisTxt(r.to_address),
    value: tokenValueTxt(
      r.value,
      r.EthTokenBalance?.decimals,
      r.EthTokenBalance?.symbol
    ),
    name: r.EthTokenBalance?.name || "",
  });
