from brownie import BoxV1, UpgradeableProxy, ProxyAdmin, config, network, Contract

from scripts.helpful_scripts import get_account, encode_function_data


def main():
    account = get_account()
    print(f'Deploying to {network.show_active()}')
    box = BoxV1.deploy({'from': account},
                       publish_source=config['networks'][network.show_active()].get('verify', False))

    proxy_admin = ProxyAdmin.deploy(
        {'from': account}, publish_source=config['networks'][network.show_active()].get('verify', False))

    box_encoded_initializer_function = encode_function_data()

    proxy = UpgradeableProxy.deploy(
        box.address, proxy_admin.address, box_encoded_initializer_function, {'from': account, 'gas_limit': 1000000}, publish_source=config['networks'][network.show_active()].get('verify', False))
    print(f"Proxy deployed at {proxy.address}, upgrade to V2 now available")
    proxy_box = Contract.from_abi(
        'BoxV1', proxy.address, BoxV1.abi)
    print(f"Proxy box has value {proxy_box.getValue()}")
