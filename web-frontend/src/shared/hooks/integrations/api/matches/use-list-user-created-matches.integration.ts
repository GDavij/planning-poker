import { useEffect, useState } from "react";
import { ApiResponse } from "../../../../models/base";
import { api } from "../axios.service";
import { ListMatchesQueryResponse } from "./matches";

export function useListUserCreatedMatches(
  page: number,
  limit: number,
  abortController: AbortController,
) {
  const [matches, setmatches] = useState<ListMatchesQueryResponse[]>([]);
  const [isFetching, setIsFetching] = useState<boolean>(false);

  useEffect(() => {
    setIsFetching(true);
    api
      .get<ApiResponse<ListMatchesQueryResponse[]>>("matches", {
        params: { page, limit },
        // signal: abortController.signal,
      })
      .then((res) => setmatches(res.data.data!))
      .finally(() => setIsFetching(false));

    return () => {
      abortController.abort();
    };
  }, []);

  return { matches, isFetching };
}
