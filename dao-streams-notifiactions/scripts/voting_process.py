from brownie import Box, MyDao, accounts
from scripts.helpful_scripts import get_account
from scripts.deploy_contracts import deploy_contracts

VALUE = 7


def voting_process():
    account = get_account()
    box = Box[-1]
    my_dao = MyDao[-1]

    print(f"value of box: {box.getValue()}")

    tx = my_dao.addProposal(VALUE, {"from": account})
    tx.wait(1)

    proposal = my_dao.getProposalId() - 1

    tx2 = my_dao.vote(proposal, True, {"from": account})
    tx2.wait(1)

    # tx3 = my_dao.vote(0, True, {"from": accounts[1]})
    # tx3.wait(1)
    # tx4 = my_dao.vote(0, True, {"from": accounts[2]})
    # tx4.wait(1)

    tx5 = my_dao.executeProposal(proposal, {"from": account})
    tx5.wait(1)

    print(f"value of box: {box.getValue()}")


def main():
    voting_process()
