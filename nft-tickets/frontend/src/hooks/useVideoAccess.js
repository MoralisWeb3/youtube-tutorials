import { useState } from "react";
import { ethers } from "ethers";
import TicketNFT_JSON from "../chain-info/contracts/TicketNFT.json";

export const useVideoAccess = (signer, account, contractAddress, navigate) => {
  const [hasAccess, setHasAccess] = useState(false);
  const ERC721_ABI = TicketNFT_JSON.abi;

  const checkAccess = async () => {
    try {
      const contract = new ethers.Contract(contractAddress, ERC721_ABI, signer);
      const userTokenCount = await contract.balanceOf(account);

      let hasNFT = false;

      // Iterate through user's owned tokens and check if any belong to the desired contract
      for (let i = 0; i < userTokenCount; i++) {
        const tokenId = await contract.tokenOfOwnerByIndex(account, i);
        if (tokenId) {
          hasNFT = true;
          break;
        }
      }

      setHasAccess(hasNFT);
      if (navigate) {
        navigate("/video", { state: { hasAccess: hasNFT } });
      }

      return hasNFT;
    } catch (error) {
      console.error("Error checking access:", error);
      setHasAccess(false);
      return false;
    }
  };

  return { hasAccess, checkAccess };
};
