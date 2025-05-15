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
  storyNumber: string;
  order: number;
  storyPoints: StoryPointResponse[];
}

export interface StoryPointResponse {
  points: number;
  participantName: string;
}

export interface Participant {
  accountId: number;
  roleName: string;
  isSpectating: boolean;
  participantName: string;
  votes: Vote[];
}

export interface Vote {
  storyId: number;
  hasVotedAlready: boolean;
  points: number;
}
