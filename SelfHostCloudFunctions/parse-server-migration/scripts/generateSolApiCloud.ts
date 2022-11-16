import { createCloudFile } from './utils/generateApiCloudCode';
import { fetchEndpoints } from './utils/prepareSwaggerForApi';
import path from 'path';

const OUTPUT_FILE = path.join(__dirname, '../src/cloud/generated', 'solApi.ts');
const API_SWAGGER_URL = 'https://solana-gateway.moralis.io/api-json/';

export const generateSolApiCloud = async () => {
  const { endpoints } = await fetchEndpoints(API_SWAGGER_URL);
  await createCloudFile(OUTPUT_FILE, 'SolApi', endpoints);
};
