const MorarableToken = artifacts.require("MorarableToken");

module.exports = function (deployer) {
  deployer.deploy(MorarableToken);
};
