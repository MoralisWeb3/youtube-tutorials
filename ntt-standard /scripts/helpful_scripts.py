from brownie import accounts, network, config

LOCAL_BLOCKCHAIN_ENVIRONMENTS = ["hardhat", "development", "ganache", "mainnet-fork"]

def get_account(index=None, id=None):
    if index:
        return accounts[index]
    if id:
        return accounts.load(id)
    if network.show_active() in LOCAL_BLOCKCHAIN_ENVIRONMENTS:
        return accounts[0]
    return accounts.add(config["wallets"]["from_key"])