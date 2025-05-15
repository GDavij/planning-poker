import { useEffect, useState } from "react";
import {
  ListRolesQueryResponse,
  Story,
  StartMatchCommandResponse,
  Participant,
} from "../../../models/matches";
import { api } from "./axios.service";

export function listMatchRoles(abortController: AbortController) {
  return api
    .get<ListRolesQueryResponse[]>("matches/roles", {
      signal: abortController.signal,
    })
    .then((r) => r.data);
}

export function listMatchStories(matchId: number) {
  return api
    .get<Story[]>(`/matches/match/${matchId}/stories`)
    .then((r) => r.data);
}

export function updateStory(story: Story) {
  return api.put<void>(
    `/matches/match/${story.matchId}/story/${story.storyId}/update`,
    story,
  );
}

export function createStory(story: Story) {
  return api.post<void>(`/matches/match/${story.matchId}/story/add`, story);
}

export function deleteStory(story: Story) {
  return api.delete<void>(
    `/matches/match/${story.matchId}/story/${story.storyId}`,
  );
}

export function selectStoryToAnalyze(story: Story) {
  return api.patch(`/matches/match/${story.matchId}/story/${story.storyId}`);
}

export function listParticipantsForMatch(matchId: number) {
  return api
    .get<Participant[]>(`/matches/match/${matchId}/participants`)
    .then((r) => r.data);
}

export function voteForStory(matchId: number, storyId: number, points: number) {
  return api.patch<void>(
    `/matches/match/${matchId}/story/${storyId}/vote/${points}`,
  );
}

export function finishMatch(matchId: number) {
  return api.patch<void>(`matches/match/${matchId}/finish`);
}
