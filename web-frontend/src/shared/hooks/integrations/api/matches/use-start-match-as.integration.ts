import { useState } from "react";
import { ApiResponse } from "../../../../models/base";
import { StartMatchCommandResponse } from "../../../../models/matches";
import { api } from "../axios.service";

type JoinInfo = {
  description: string;
  roleId: number;
  shouldSpectate: boolean;
};

export function useStartMatch() {
  const [isStarting, setIsStarting] = useState(false);

  const startMatch = (joinInfo: JoinInfo) => {
    setIsStarting(true);
    return api
      .post<ApiResponse<StartMatchCommandResponse>>("matches/start", joinInfo)
      .then((res) => res.data.data)
      .finally(() => setIsStarting(false));
  };

  return { startMatch, isStarting };
}
