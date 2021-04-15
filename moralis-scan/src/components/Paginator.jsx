import React, { createContext, useContext } from 'react'
import { usePagination } from '../hooks/pagination';

export const ResultContext = createContext({results: []});
export const  useResultContext = () => useContext(ResultContext);

export default function Paginator({methodName, userAddress, options, children}) {
  const {
    // pageSize,
    // setPageSize,
    totalPages,
    setCurrPage,
    currPage,
    nextPage,
    prevPage,
    results,
    numResults,
  } = usePagination(methodName, userAddress, options);

  return (
    <div>
      <div className="d-flex justify-content-between align-items-start mb-2">
        <span>A total of {numResults} transactions found</span>
        <div>
          <button className="btn btn-info ms-1" onClick={() => setCurrPage(1)}>
            First
          </button>
          <button className="btn btn-info ms-1" onClick={prevPage}>
            {"<"}
          </button>
          <span className="btn btn-info ms-1">
            Page {currPage} of {totalPages}
          </span>
          <button className="btn btn-info ms-1" onClick={nextPage}>
            {">"}
          </button>
          <button
            className="btn btn-info ms-1"
            onClick={() => setCurrPage(totalPages)}
          >
            Last
          </button>
        </div>
      </div>
      <ResultContext.Provider value={{ results }}>
        {children}
      </ResultContext.Provider>
    </div>
  );
}
