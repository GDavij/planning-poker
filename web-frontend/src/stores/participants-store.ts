import { create } from "zustand";
import { Participant } from "../models/matches";

interface PartyParticipantsActions {
  participants: Participant[];
  allocateParticipants: (participants: Participant[]) => void;
  voteOrReplace: (storyId: number, points: number, accountId: number) => void;
}

export const useParticipants = create<PartyParticipantsActions>((set) => ({
  participants: [],
  allocateParticipants: (participants) => set({ participants }),
  voteOrReplace: (storyId, points, accountId) =>
    set((state) => {
      const stateCopy = { ...state };

      if (
        stateCopy.participants.some((p) =>
          p.votes.some((v) => v.storyId == storyId),
        )
      ) {
        let currentVote = stateCopy.participants
          .find((p) => p.accountId == accountId)!
          .votes.find((v) => v.storyId == storyId);
        currentVote!.hasVotedAlready = true;
        currentVote!.points = points;

        return stateCopy;
      }

      const participant = stateCopy.participants.find(
        (p) => p.accountId == accountId,
      );
      if (!participant) {
        return stateCopy;
      }

      participant.votes.push({
        storyId: storyId,
        hasVotedAlready: true,
        points: points,
      });

      return stateCopy;
    }),
}));
