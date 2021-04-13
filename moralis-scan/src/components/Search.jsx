import React, { useMemo, useState } from 'react'
import { useHistory } from 'react-router';
import { useMoralisCloudQuery } from '../hooks/cloudQuery';

export default function Search() {
  const [searchTxt, setSearchTxt] = useState("");
  const [address, setAddress] = useState("");
  const [error, setError] = useState(null);
  const history = useHistory();

  const onSearchTextChanged = (e) => setSearchTxt(e.target.value);

  const watchParams = useMemo(()=> ({
    params: {address}, // query params
    onSuccess: () => history.push(`/address/${address}`),
  }), [address, history]);
  const {loading} = useMoralisCloudQuery("watchEthAddress", watchParams);

  const submitSearch = async (e) => {
    e.preventDefault();
    setError(null);

    const searchAddress = searchTxt.trim().toLowerCase();
    console.log("Search:", searchAddress);
    if (searchAddress.length !== 42) {
      const msg = "not an address";
      console.log(msg);
      setError(msg);
      return;
    }
    setAddress(searchAddress);
  };

  return (
    <div>
      <form onSubmit={submitSearch}>
        <div className="input-group">
          <input
            type="text"
            className="form-control"
            placeholder="Search by address"
            aria-label="Search by address"
            aria-describedby="btn-search"
            onChange={onSearchTextChanged}
          />
          <button className="btn btn-outline-secondary" id="btn-search">
            Search
            {loading && (
              <>
                <span
                  className="spinner-border spinner-border-sm"
                  role="status"
                  aria-hidden="true"
                ></span>
                <span className="visually-hidden">Loading...</span>
              </>
            )}
          </button>
        </div>
        <div className="text-danger">{error}</div>
      </form>
    </div>
  );
}
