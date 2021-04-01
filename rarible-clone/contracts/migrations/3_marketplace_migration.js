const MorarableMarketContract = artifacts.require("MorarableMarketContract");

module.exports = function (deployer) {
  deployer.deploy(MorarableMarketContract);
};
