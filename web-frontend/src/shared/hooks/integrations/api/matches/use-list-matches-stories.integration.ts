import { useEffect, useState } from "react";
import { api } from "../axios.service";
import { Story } from "../../../../models/matches";
import { ApiResponse } from "../../../../models/base";
import { useSignalRContext } from "../../../../contexts/signalr.context";
import { SignalRHooks } from "../../../../consts/signalRHooks";
import { useMatch } from "../../../../stores/match-store";

export function useListMatchStories(matchId: number) {
  const [isFetching, setIsFetching] = useState(false);

  const { stories, setStories, setStoriesFunc } = useMatch();

  const { registerEndpointFor } = useSignalRContext();

  useEffect(() => {
    setIsFetching(true);

    api
      .get<ApiResponse<Story[]>>(`/matches/match/${matchId}/stories`)
      .then((r) => setStories(r.data.data!))
      .finally(() => setIsFetching(false));

    registerEndpointFor(SignalRHooks.OnListMatchStories, (serverStories) => {
      setStories(serverStories as Story[]);
    });
  }, [matchId]);

  return { stories, isFetching, setStoriesFunc };
}
