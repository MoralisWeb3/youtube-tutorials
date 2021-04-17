import React from 'react'
import { useParams } from 'react-router';
import { processTransaction } from '../queries/transactions';
import Paginator from './Paginator';
import TransResults from './TransResults';

export default function Transactions() {
  const {address } = useParams();
  if (!address) {
    return null;
  }

  return (
    <div>
      <Paginator
        userAddress={address}
        methodName="getTransactions"
        options={{ postProcess: processTransaction }}
      >
        <TransResults />
      </Paginator>
    </div>
  );
}
