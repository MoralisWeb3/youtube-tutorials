import { DateTime } from "luxon";

export const n6 = new Intl.NumberFormat("en-us", {
  style: "decimal",
  minimumFractionDigits: 0,
  maximumFractionDigits: 6,
});

/**
 * Return a formatted string with the symbol at the end
 * @param {number} value integer value
 * @param {number} decimals number of decimals
 * @param {string} symbol token symbol
 * @returns {string}
 */
export const tokenValueTxt = (value, decimals, symbol) =>
  decimals ? `${n6.format(value / Math.pow(10, decimals))} ${symbol}` : `${value}`;

/**
 * Return human readable interval from the given time stamp (i.e. 1 minute ago)
 * @param {number} unixTimeStampMili Unix style timestamp in miliseconds
 * @returns {string}
 */
export const agoTxt = (unixTimeStampMili) => DateTime.fromMillis(unixTimeStampMili).toRelative();

/**
 * Returns a string of form "abc...xyz"
 * @param {string} str string to strink
 * @param {number} n number of chars to keep at front/end
 * @returns {string}
 */
export const getEllipsisTxt = (str, n = 6) => {
  return `${str.substr(0, n)}...${str.substr(
    str.length - n,
    str.length
  )}`;
}

export const toEth = (wei) => tokenValueTxt(wei, 18, "ETH");

export const toGwei = (wei) => {
  return `${Math.round(wei / 1e9)} gwei`;
}
