import { generateEvmApiCloud } from './generateEvmApiCloud';
import { generateSolApiCloud } from './generateSolApiCloud';

const run = async () => {
  await Promise.resolve([generateEvmApiCloud(), generateSolApiCloud()]);
};

run();
