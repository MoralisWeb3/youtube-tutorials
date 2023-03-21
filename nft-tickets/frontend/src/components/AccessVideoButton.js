import React from "react";
import { useNavigate } from "react-router-dom";
import { useVideoAccess } from "../hooks/useVideoAccess";
import sytles from "../styles/Home.module.css";

export const AccessVideoButton = ({
  provider,
  signer,
  account,
  contractAddress,
}) => {
  const navigate = useNavigate();
  const { checkAccess } = useVideoAccess(
    provider,
    signer,
    account,
    contractAddress,
    navigate
  );

  const handleClick = async () => {
    await checkAccess();
  };

  return (
    <button onClick={handleClick} className={sytles.form_btn}>
      Access
    </button>
  );
};
