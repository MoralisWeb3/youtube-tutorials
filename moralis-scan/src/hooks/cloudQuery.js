import Moralis from "moralis";
import { useEffect, useState } from "react";

const defaultCloudQueryOptions = {
  params: {},
  postProcess: (r) => r.attributes,
  onSuccess: () => {},
};

export function useMoralisCloudQuery(
  methodName,
  options = defaultCloudQueryOptions
) {
  const [state, setState] = useState({
    data: null,
    error: null,
    loading: false,
  });

  useEffect(() => {
    if (methodName) {
      setState((v) => ({ ...v, loading: true }));
      Moralis.Cloud.run(methodName, options.params)
        .then((data) => {
          if (data) {
            const output = options.postProcess
              ? data.map(options.postProcess)
              : data;
            setState({ data: output, error: null, loading: false });
          } else {
            setState({ data: null, error: null, loading: false });
          }

          if (typeof options.onSuccess === "function") {
            options.onSuccess();
          }
        })
        .catch((error) => {
          console.error(error);
          setState({ data: null, error, loading: false });
        });
    }
  }, [methodName, options]);

  return state;
}
