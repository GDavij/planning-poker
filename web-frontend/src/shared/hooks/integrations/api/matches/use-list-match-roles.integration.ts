import { useEffect, useState } from "react";
import { ListRolesQueryResponse } from "../../../../models/matches";
import { api } from "../axios.service";
import { ApiResponse } from "../../../../models/base";

export function useListMatchRoles(abortController: AbortController) {
  const [isFetching, setIsFetching] = useState(false);
  const [roles, setRoles] = useState<ListRolesQueryResponse[]>([]);

  useEffect(() => {
    setIsFetching(true);

    api
      .get<ApiResponse<ListRolesQueryResponse[]>>("matches/roles", {
        signal: abortController.signal,
      })
      .then((r) => setRoles(r.data.data!))
      .finally(() => setIsFetching(false));
  }, []);

  return { roles, isFetching };
}
