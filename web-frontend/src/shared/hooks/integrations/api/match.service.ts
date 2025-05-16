import { Story, Participant } from "../../../models/matches";
import { api } from "./axios.service";

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
