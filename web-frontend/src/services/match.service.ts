import {
  ListMatchesQueryResponse,
  ListRolesQueryResponse,
  StartMatchCommandResponse,
} from "../models/matches";
import { api } from "./axios.service";

export function listUserCreatedMatches(
  page: number,
  limit: number,
  abortController: AbortController,
) {
  return api.get<ListMatchesQueryResponse[]>("matches", {
    params: { page, limit },
    signal: abortController.signal,
  });
}

export function startMatch(description: string) {
  return api.post<StartMatchCommandResponse>("matches/start", { description });
}

export function listMatchRoles(abortController: AbortController) {
  return api.get<ListRolesQueryResponse[]>("matches/roles", {
    signal: abortController.signal,
  });
}

export function takePartOfMatchAs(
  roleId: number,
  shouldSpectate: boolean,
  matchId: number,
  authCode: string | null = null,
) {
  return api.patch(`matches/take-part/${matchId}`, {
    roleId,
    shouldSpectate,
    authCode,
  });
}
