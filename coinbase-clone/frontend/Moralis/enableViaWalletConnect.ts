import WalletConnect from "@walletconnect/client";
import Web3 from "web3";
import { AbstractProvider } from "web3-core";
import { JsonRpcPayload, JsonRpcResponse } from "web3-core-helpers";

// eslint-disable-next-line functional/prefer-readonly-type
export async function enableViaWalletConnect({ connector }: { connector: WalletConnect }) {
  // @ts-ignore
  const { connector: newConnector } = await connector.connect();

  const makeJsonRpcResponse = (payload: JsonRpcPayload, result: any, error?: Error): JsonRpcResponse => ({
    id: +payload.id,
    jsonrpc: payload.jsonrpc,
    result,
    error: error ? error.message : undefined,
  });

  const abstractProvider: AbstractProvider = {
    sendAsync: (payload, callback) => {
      newConnector
        .sendCustomRequest(payload)
        .then((result) => callback(null, makeJsonRpcResponse(payload, result)))
        .catch((error) => callback(error, makeJsonRpcResponse(payload, null, error)));
    },
    send: (payload, callback) => {
      newConnector
        .sendCustomRequest(payload)
        .then((result) => callback(null, makeJsonRpcResponse(payload, result)))
        .catch((error) => callback(error, makeJsonRpcResponse(payload, null, error)));
    },
    connected: newConnector.connected,
  };

  const web3 = new Web3(abstractProvider);

  return web3;
}
