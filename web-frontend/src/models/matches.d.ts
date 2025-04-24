export interface ListMatchesQueryResponse {
  matchId: number;
  description: string;
  hasStarted: boolean;
  hasClosed: boolean;
}

export interface ListRolesQueryResponse {
  roleId: number;
  name: string;
  abbreviation: string;
}

export interface StartMatchCommandResponse {
  matchId: number;
}

export interface Story {
  storyId: number;
  matchId: number;
  name: string;
  storyNumber?: string;
  order: number;
  storyPoints: StoryPointResponse[];
}

export interface StoryPointResponse {
  points: number;
  participantName: string;
}
