import { Participant } from "../models/matches";
import { useEffect } from "react";
import { useSignalRContext } from "../contexts/signalr.context";
import { Avatar, Box, Card, Stack, Typography } from "@mui/material";
import { AppCard } from "./app-card";
import { listParticipantsForMatch } from "../services/match.service";
import { useParams } from "react-router";
import { useSnackbar } from "./snackbar";
import { LocalFireDepartment, Timer } from "@mui/icons-material";
import { useParticipants } from "../stores/participants-store";
import { useMatch } from "../stores/match-store";
import { useAuth } from "../stores/auth-store";

export function PartyParticipantsViewer() {
  const matchId = Number(useParams()?.matchId);

  const { signalRClient, registerEndpointFor, disconnectFromEndpointFor } =
    useSignalRContext();
  const { participants, allocateParticipants, voteOrReplace } =
    useParticipants();
  const { currentShowingStory } = useMatch();
  const { accountId } = useAuth();
  const { showError, showInfo } = useSnackbar();

  useEffect(() => {
    registerEndpointFor(
      signalRClient,
      "CurrentListOfParticipantsIs",
      (participants) => {
        console.log({ signalRParticipants: participants });
        showInfo("A new participant has join the party");
        allocateParticipants(participants as Participant[]);
      },
    );

    registerEndpointFor(
      signalRClient,
      "ParticipantVoteForStoryIs",
      (votePoints) => {
        console.log({ SeeTheVotes: votePoints });
        voteOrReplace(
          currentShowingStory!.storyId,
          votePoints as number,
          accountId,
        );
      },
    );

    listParticipantsForMatch(matchId)
      .then(allocateParticipants)
      .catch(() => {
        showError("Could not load party participants");
      });

    return () => {
      disconnectFromEndpointFor(signalRClient, "CurrentListOfParticipantsIs");
    };
  }, [matchId]);

  const hasVoted = (participant: Participant) => {
    console.log({ hasVOted: participant.votes });
    return participant.votes.some(
      (v) => v.storyId == currentShowingStory?.storyId,
    );
  };

  return (
    <AppCard sx={{ paddingX: 4, paddingY: 2 }}>
      <Typography> Participants </Typography>
      <Stack direction="row">
        {participants.map((participant) => (
          <Card key={participant.accountId} sx={{ paddingX: 1, paddingY: 2 }}>
            <Stack direction="row" spacing={2}>
              <Avatar>{participant.participantName[0]}</Avatar>
              <Stack spacing={1}>
                <Typography> {participant.participantName}</Typography>
                <Box alignItems={"center"}>
                  {" "}
                  {hasVoted(participant) ? (
                    <Typography
                      sx={{ display: "flex", alignItems: "flex-end", gap: 1 }}
                    >
                      Voted as{" "}
                      {
                        participant.votes.find(
                          (v) => v.storyId == currentShowingStory?.storyId,
                        )?.points
                      }
                      <LocalFireDepartment />
                    </Typography>
                  ) : (
                    <Typography
                      sx={{ display: "flex", alignItems: "flex-end", gap: 1 }}
                    >
                      {" "}
                      Waiting Vote
                      <Timer />
                    </Typography>
                  )}
                </Box>
              </Stack>
            </Stack>
          </Card>
        ))}
      </Stack>
    </AppCard>
  );
}
