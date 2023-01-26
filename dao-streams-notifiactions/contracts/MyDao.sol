//SPDX-License-Identifier: MIT

pragma solidity ^0.8.0;

import "./Box.sol";
import "./GovernanceToken.sol";
import "@openzeppelin/contracts/access/Ownable.sol";

contract MyDao is Ownable {
    Box public boxInstance;
    GovernanceToken public governanceTokenInstance;

    constructor(address _tokenAddress, address _boxAddress) {
        boxInstance = Box(_boxAddress);
        governanceTokenInstance = GovernanceToken(_tokenAddress);
    }

    struct Proposal {
        address proposer;
        uint256 value;
        bool vote;
    }

    enum Results {
        Approved,
        Rejected
    }

    address[] public voters;
    uint256 proposalid = 0;

    mapping(address => bool) public members;
    mapping(address => bool) public userHasVoted;
    Proposal[] public proposals;

    event ProposalAdded(uint256 proposalId);
    event ProposalVoted(uint256 proposalId, bool vote);
    event ProposalExecuted(uint256 proposalId, Results result, uint256 value);

    function addProposal(uint256 _value) public {
        require(isMember(msg.sender), "You are not a member");
        Proposal memory proposal = Proposal({
            proposer: msg.sender,
            value: _value,
            vote: false
        });
        proposals.push(proposal);
        proposalid++;
        emit ProposalAdded(proposals.length - 1);
    }

    function vote(uint256 proposalId, bool _vote) public {
        Proposal storage proposal = proposals[proposalId];
        require(isMember(msg.sender), "Only Members can vote");
        require(userHasVoted[msg.sender] == false, "You have already voted");
        proposal.vote = _vote;
        userHasVoted[msg.sender] = true;
        voters.push(msg.sender);
        emit ProposalVoted(proposalId, _vote);
    }

    function executeProposal(uint256 proposalId) public {
        Proposal storage proposal = proposals[proposalId];
        require(proposal.vote);

        uint256 yesVotes = 0;
        uint256 noVotes = 0;
        for (uint256 i = 0; i < proposals.length; i++) {
            if (proposals[i].vote) {
                yesVotes++;
            } else {
                noVotes++;
            }
        }
        require(yesVotes > noVotes, "Proposal has been rejected");
        boxInstance.store(proposal.value);
        clearVotes();
        emit ProposalExecuted(proposalId, Results.Approved, proposal.value);
    }

    function clearVotes() public onlyOwner {
        for (uint256 i = 0; i < voters.length; i++) {
            userHasVoted[voters[i]] = false;
        }
    }

    function isMember(address _address) public view returns (bool) {
        uint256 tokenBalance = governanceTokenInstance.balanceOf(_address);
        return tokenBalance > 0;
    }

    function getProposalId() public view returns (uint256) {
        return proposalid;
    }
}
