from brownie import Certificate 
from scripts.helpful_scripts import get_account

def deploy_certificate():
    account = get_account()
    name = "Moralis Certificate"
    symbol = "MOCRT"
    certificate = Certificate.deploy(name, symbol, {"from": account})
    print(certificate.address)
    return certificate

def main():
    deploy_certificate()
    