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
