/**
 * @type import('hardhat/config').HardhatUserConfig
 */
require("@nomiclabs/hardhat-waffle");
require("dotenv/config");

const { HARDHAT_PORT } = process.env;

module.exports = {
  solidity: "0.7.3",
  networks: {
    localhost: { url: `http://127.0.0.1:${HARDHAT_PORT}` },
    hardhat: {
      accounts: [{"privateKey":"0x6717acf4d7ec1befbe090dc2e6f1fb997ee322cf307ef0c3a28ec5378ef03e24","balance":"1000000000000000000000"},{"privateKey":"0xabbd3f5261f33efab61dfa9dc92d8a6775e6d391f512c7c820765c006bcc3ea4","balance":"1000000000000000000000"},{"privateKey":"0xd2c47413b3a5b1c5b75ffceb2b65797b67543a08a76c8f7c0d1bf6340b14f029","balance":"1000000000000000000000"},{"privateKey":"0x0ca02cc0ec8878279dee66bc2b2d793b2387a77d20e073e837e5323e5f0e7eeb","balance":"1000000000000000000000"},{"privateKey":"0x8efc3ce855ffa48663610e4c72d34c3f3053409fc97a23d43f32be2b06e1c694","balance":"1000000000000000000000"},{"privateKey":"0x091915e1dbfc93c3a075ede32ad072cbdbaf912c0159b03d89816c05e564a117","balance":"1000000000000000000000"},{"privateKey":"0xf59b97139bba57e6cb8cd601be3bf9a9540d6b9ac232cfced7b6349b0d80dbbd","balance":"1000000000000000000000"},{"privateKey":"0xc971c82d681507f1e22d684327dee361d6b9ffe0e80d0849a62bc366c5dc7132","balance":"1000000000000000000000"},{"privateKey":"0x311d1ce35727a0277c1c7f305c42372fb627dd5d292d13918f04fddc5fb4d06e","balance":"1000000000000000000000"},{"privateKey":"0x288fce79cccf379299665751c61219c2a500d478aff45c57fb280a6a4dfb5942","balance":"1000000000000000000000"}]
    },
  },
  paths: {
    sources: './contracts',
    tests: './__tests__/contracts',
    cache: './cache',
    artifacts: './artifacts',
  },
};