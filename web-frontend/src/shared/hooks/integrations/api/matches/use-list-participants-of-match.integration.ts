import { useEffect, useState } from "react";
import { Participant } from "../../../../models/matches";
import { api } from "../axios.service";
import { useMatchStore } from "../../../../stores/match-store";
import { useParticipants } from "../../../../stores/participants-store";
import { SignalRHooks } from "../../../../consts/signalRHooks";
import { useSnackbar } from "../../../../ui/snackbar";
import { useSignalRContext } from "../../../../contexts/signalr.context";

export function useListParticipantsOfMatch(matchId: number) {
  const [isFetching, setIsFetching] = useState(false);
  const { participants, allocateParticipants } = useParticipants();
  const { showInfo, showError } = useSnackbar();
  const { registerEndpointFor } = useSignalRContext();

  useEffect(() => {
    setIsFetching(true);

    api
      .get<Participant[]>(`/matches/match/${matchId}/participants`)
      .then((r) => allocateParticipants(r.data))
      .catch(() => {
        showError("Could not load party participants");
      })
      .finally(() => setIsFetching(false));

    registerEndpointFor(SignalRHooks.OnParticipantJoin, (participants) => {
      showInfo("A new participant has join the party");
      allocateParticipants(participants as Participant[]);
    });
  }, [matchId]);

  return { participants, isFetching };
}
