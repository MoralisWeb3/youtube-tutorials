import { useEffect, useMemo, useState } from "react";
import { useMoralisCloudQuery } from "./cloudQuery";

const defaultPaginationOptions = {
  postProcess: (r) => r.attributes, // function to apply to each result (result) => result
};

export const usePagination = (methodName, userAddress, options = defaultPaginationOptions) => {
  const [pageSize, setPageSize] = useState(10);
  const [currPage, setCurrPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [numResults, setNumResults] = useState(0);

  const queryOptions = useMemo(()=> ({
    includesCount: true,
    countName: "count",
    params: {
      userAddress,
      pageSize,
      pageNum: currPage,
    },
    postProcess: options.postProcess,
  }), [userAddress, pageSize, currPage, options.postProcess])
  const { data, error, loading } = useMoralisCloudQuery(methodName, queryOptions);

  useEffect(() => {
    if (data) {
      setNumResults(data.count || 0);
      const n = Math.ceil(data.count / pageSize);
      setTotalPages(n);
    } else {
      setNumResults(0);
      setTotalPages(1);
    }
  }, [data, pageSize]);

  const nextPage = () => {
    if (currPage >= totalPages) {
      // already at the last page
      return;
    }
    const nextPage = currPage + 1;
    setCurrPage(nextPage);
  };

  const prevPage = () => {
    if (currPage <= 1) {
      // already at the first page
      return;
    }
    const nextPage = currPage - 1;
    setCurrPage(nextPage);
  };

  return {
    pageSize,
    setPageSize,
    totalPages,
    currPage,
    setCurrPage,
    nextPage,
    prevPage,
    results: data?.results || [],
    numResults,
    loading,
    error,
  };

}
