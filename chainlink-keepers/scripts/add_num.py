from brownie import AddNum
from scripts.helpful_scripts import get_account


def add_num():
    account = get_account()
    num = AddNum[-1]
    tx = num.add({"from": account})
    tx.wait(1)
    new_num = num.getNumber()
    print(f"New number is {new_num}")


def main():
    add_num()
