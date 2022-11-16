import { Endpoint } from '../types/Endpoint';
import fs from 'fs';

type Module = 'EvmApi' | 'SolApi';

const getModulePrefix = (module: Module) => {
  switch (module) {
    case 'EvmApi':
      return '';
    case 'SolApi':
      return 'sol-';
    default:
      throw new Error(`No prefix defined for module '${module}'`);
  }
};

const generateCloudCode = (module: Module, endpoint: Endpoint) => {
  let code = '';
  const name = `${getModulePrefix(module)}${endpoint.name}`;
  code += `Parse.Cloud.define("${name}", async ({params, user, ip}: any) => {
  try {
    await beforeApiRequest(user, ip, '${endpoint.name}');
    const result = await Moralis.${module}.${endpoint.group}.${endpoint.name}(params);
    return result?.raw;
  } catch (error) {
    throw new Error(getErrorMessage(error, '${name}'));
  }
})`;

  return code;
};

const generateAllCloudCode = (module: Module, endpoints: Endpoint[]) => {
  let output = `/* eslint-disable @typescript-eslint/no-var-requires */
/* eslint-disable @typescript-eslint/no-explicit-any */
import Moralis from 'moralis'
import { MoralisError } from '@moralisweb3/core';
import { handleRateLimit } from '../../rateLimit'
import { AxiosError } from 'axios'
declare const Parse: any;

const getErrorMessage = (error: Error, name: string) => {
  // Resolve Axios data inside the MoralisError
  if (
    error instanceof MoralisError &&
    error.cause &&
    error.cause instanceof AxiosError &&
    error.cause.response &&
    error.cause.response.data
  ) {
    return JSON.stringify(error.cause.response.data);
  }

  if (error instanceof Error) {
    return error.message;
  } 

  return \`API error while calling \${name}\`
}

const beforeApiRequest = async (user: any, ip: any, name: string) => {
  if (!(await handleRateLimit(user, ip))) {
    throw new Error(
      \`Too many requests to \${name} API from this particular client, the clients needs to wait before sending more requests.\`
    );
  }
}

`;

  endpoints.forEach((endpoint) => {
    output += generateCloudCode(module, endpoint);
    output += '\n\n';
  });

  return output;
};

export const createCloudFile = async (outPath: string, module: Module, endpoints: Endpoint[]) => {
  const code = generateAllCloudCode(module, endpoints);
  await fs.writeFileSync(outPath, code);
};
