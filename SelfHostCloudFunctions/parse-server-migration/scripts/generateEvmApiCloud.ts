import { createCloudFile } from './utils/generateApiCloudCode';
import { fetchEndpoints } from './utils/prepareSwaggerForApi';
import path from 'path';

const OUTPUT_FILE = path.join(__dirname, '../src/cloud/generated', 'evmApi.ts');
const API_SWAGGER_URL = 'https://deep-index.moralis.io/api-docs/v2/swagger.json';

export const generateEvmApiCloud = async () => {
  const { endpoints } = await fetchEndpoints(API_SWAGGER_URL);
  await createCloudFile(OUTPUT_FILE, 'EvmApi', endpoints);
};
