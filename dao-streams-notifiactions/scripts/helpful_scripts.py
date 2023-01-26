from brownie import network, accounts, config
import os
import shutil
import yaml
import json

LOCAL_BLOCKCHAIN_ENVIRONMENTS = ["development"]


def get_account(index=None, id=None):
    if index:
        return accounts[index]
    if id:
        return accounts.load(id)
    if network.show_active() in LOCAL_BLOCKCHAIN_ENVIRONMENTS:
        return accounts[0]
    return accounts.add(config["wallets"]["from_key"])


def update_frontend():
    # Send the build Folder to front-end
    copy_folder_to_frontend("./build", "./frontend/src/chain-info")
    # Send brownie-config.yaml to front-end
    with open("brownie-config.yaml", "r") as brownie_config:
        config_dict = yaml.load(brownie_config, Loader=yaml.FullLoader)
        with open("./frontend/src/brownie-config.json", "w") as brownie_config_json:
            json.dump(config_dict, brownie_config_json)


def copy_folder_to_frontend(src, dest):
    if os.path.exists(dest):
        shutil.rmtree(dest)
    shutil.copytree(src, dest)
