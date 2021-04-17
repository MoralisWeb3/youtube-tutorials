import React from 'react'
import { useParams } from 'react-router';
import { useTransType } from '../hooks/useTransType';
import Paginator from './Paginator';
import TransResults from './TransResults';

export default function Transactions() {
  const {address } = useParams();
  const {methodName, postProcess} = useTransType();
  if (!address) {
    return null;
  }

  return (
    <div>
      <Paginator
        userAddress={address}
        methodName={methodName}
        options={{ postProcess }}
      >
        <TransResults />
      </Paginator>
    </div>
  );
}
