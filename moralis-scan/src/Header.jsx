import React from 'react'
import Search from './components/Search';

export default function Header() {
  return (
    <header className="d-flex justify-content-between align-items-center p-2">
      <div className="col col-sm-6">
        <span className="h1">Moralis Scan</span>
      </div>
      <div className="col col-sm-6">
        <Search />
      </div>
    </header>
  );
}
