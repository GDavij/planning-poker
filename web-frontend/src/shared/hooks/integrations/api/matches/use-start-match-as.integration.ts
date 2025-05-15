import { StartMatchCommandResponse } from "../../../../models/matches";
import { api } from "../axios.service";

export function useStartMatchAs(
  description: string,
  roleId: number,
  shouldSpectate: boolean,
) {
  return api.post<StartMatchCommandResponse>("matches/start", {
    description,
    roleId,
    shouldObservate: shouldSpectate,
  });
}
