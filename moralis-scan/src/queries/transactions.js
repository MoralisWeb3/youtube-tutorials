import { agoTxt, getEllipsisTxt, toEth, toGwei } from "./utils";

export const processTransaction = (r) => ({
    block_number: r.attributes.block_number,
    hash: getEllipsisTxt(r.attributes.hash),
    ago: agoTxt(r.attributes.block_timestamp.valueOf()),
    from_address: getEllipsisTxt(r.attributes.from_address),
    to_address: getEllipsisTxt(r.attributes.to_address),
    value: toEth(r.attributes.value),
    gas_price: toGwei(r.attributes.gas_price),
  });
