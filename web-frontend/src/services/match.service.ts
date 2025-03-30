import {
  ListMatchesQueryResponse,
  ListRolesQueryResponse,
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
  return api.post("matches/start", { description });
}

export function listMatchRoles(abortController: AbortController) {
  return api.get<ListRolesQueryResponse[]>("matches/roles", {
    signal: abortController.signal,
  });
}
