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
  const { showError, showInfo } = useSnackbar();

  useEffect(() => {
    disconnectFromEndpointFor(
      signalRClient,
      "CurrentListOfParticipantsIs",
    ).then(() => {
      registerEndpointFor(
        signalRClient,
        "CurrentListOfParticipantsIs",
        (participants) => {
          showInfo("A new participant has join the party");
          allocateParticipants(participants as Participant[]);
        },
      );
    });

    disconnectFromEndpointFor(signalRClient, "ParticipantVoteForStoryIs").then(
      () => {
        registerEndpointFor(
          signalRClient,
          "ParticipantVoteForStoryIs",
          (vote) => {
            const voteObj = vote as {
              storyId: number;
              points: number;
              accountId: number;
            };
            voteOrReplace(
              voteObj.storyId as number,
              voteObj.points as number,
              voteObj.accountId as number,
            );
          },
        );
      },
    );

    listParticipantsForMatch(matchId)
      .then(allocateParticipants)
      .catch(() => {
        showError("Could not load party participants");
      });
  }, [matchId]);

  const hasVoted = (participant: Participant) => {
    const hasVoted = participant.votes.some(
      (v) => v.storyId == currentShowingStory?.storyId && v.hasVotedAlready,
    );
    return hasVoted;
  };

  return (
    <AppCard sx={{ paddingX: { xs: 2, sm: 4 }, paddingY: 2 }}>
      <Typography variant="h6" sx={{ mb: 2 }}>
        {" "}
        Participants{" "}
      </Typography>
      <Stack
        direction="row"
        flexWrap="wrap"
        gap={2}
        justifyContent={{ xs: "center", sm: "flex-start" }}
        sx={{ width: "100%" }}
      >
        {participants.map((participant) => (
          <Card
            key={participant.accountId}
            sx={{
              paddingX: 1.5,
              paddingY: 1.5,
              minWidth: "240px",
              maxWidth: { xs: "100%", sm: "300px" },
              flexGrow: { xs: 1, sm: 0 },
              flexShrink: 0,
              flexBasis: { xs: "100%", sm: "auto" },
            }}
            elevation={1}
          >
            <Stack direction="row" spacing={2} alignItems="center">
              <Avatar sx={{ width: 40, height: 40 }}>
                {participant.participantName[0]}
              </Avatar>
              <Stack spacing={0.5} sx={{ overflow: "hidden", width: "100%" }}>
                <Typography
                  noWrap
                  title={participant.participantName}
                  sx={{ fontWeight: "medium" }}
                >
                  {participant.participantName}
                </Typography>
                <Box>
                  {hasVoted(participant) ? (
                    <Box
                      sx={{
                        display: "flex",
                        alignItems: "center",
                        gap: 1,
                        flexWrap: "wrap",
                      }}
                    >
                      <Typography variant="body2" color="text.secondary">
                        Voted as
                      </Typography>
                      <Box
                        sx={{ display: "flex", alignItems: "center", gap: 0.5 }}
                      >
                        <Typography variant="body2" fontWeight="bold">
                          {
                            participant.votes.find(
                              (v) => v.storyId == currentShowingStory?.storyId,
                            )?.points
                          }
                        </Typography>
                        <LocalFireDepartment fontSize="small" color="error" />
                      </Box>
                    </Box>
                  ) : (
                    <Box
                      sx={{
                        display: "flex",
                        alignItems: "center",
                        gap: 1,
                        flexWrap: "wrap",
                      }}
                    >
                      <Typography variant="body2" color="text.secondary">
                        Waiting to Vote
                      </Typography>
                      <Timer fontSize="small" color="action" />
                    </Box>
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
