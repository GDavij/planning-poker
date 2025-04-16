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

export function startMatchAs(
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

export function listMatchRoles(abortController: AbortController) {
  return api
    .get<ListRolesQueryResponse[]>("matches/roles", {
      signal: abortController.signal,
    })
    .then((r) => r.data);
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
