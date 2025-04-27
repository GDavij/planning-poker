import {
  Button,
  Card,
  Container,
  Grid2,
  Stack,
  Typography,
} from "@mui/material";
import { memo, useEffect, useState } from "react";
import { useSignalRContext } from "../../../contexts/signalr.context";
import { Participant, Story, Vote } from "../../../models/matches";
import {
  listMatchStories,
  voteForStory,
} from "../../../services/match.service";
import { useParams } from "react-router";
import { StoriesListForm } from "../../../forms/stories/stories-list.form";
import { SignalRMatchHubServerEndpoints } from "../../../consts/signalr/signalr-match-hub.endpoints";
import { AppCard } from "../../../components/app-card";
import { HighlightPaper } from "../../../components/app-paper";
import { useSnackbar } from "../../../components/snackbar";
import { useDebounce } from "../../../hooks/debounce";
import { useSpring, animated } from "@react-spring/web";
import { CurrenlyShowingStoryViewer } from "../../../forms/stories/currentVotingStory.form";
import { useParticipants } from "../../../stores/participants-store";
import { useAuth } from "../../../stores/auth-store";
import { useMatch } from "../../../stores/match-store";

export function PartyPage() {
  const complexities = [
    {
      points: 1,
      description: "Low complexity",
    },
    {
      points: 2,
      description: "A little complex",
    },
    {
      points: 3,
      description: "Basic complexity",
    },
    {
      points: 5,
      description: "Maybe takes me a Day",
    },
    {
      points: 8,
      description: "Can take more than a Day",
    },
    {
      points: 13,
      description: "This surely is complex",
    },
    {
      points: 21,
      description: "This gonna be good",
    },
  ];

  const matchId = Number(useParams().matchId);

  const { showSuccess, showError } = useSnackbar();

  const {
    signalRClient,
    registerEndpointFor,
    invokeAsyncFor,
    disconnectFromEndpointFor,
  } = useSignalRContext();

  const { participants, voteOrReplace } = useParticipants();
  const { accountId } = useAuth();
  const { currentShowingStory } = useMatch();

  const [stories, setStories] = useState<Story[]>([]);

  useEffect(() => {
    listMatchStories(matchId).then((stories) => {
      setStories(stories);
    });

    invokeAsyncFor(
      signalRClient,
      SignalRMatchHubServerEndpoints.JoinMatch,
      Number(matchId),
    );

    registerEndpointFor(
      signalRClient,
      "UpdateStoriesOfMatchWith",
      (stories) => {
        setStories(stories as Story[]);
      },
    );

    registerEndpointFor(signalRClient, "SomeoneVoted", (vote) => {
      const voteObj = vote as {
        storyId: number;
        complexity: number;
        accountId: number;
      };

      voteOrReplace(voteObj.storyId, voteObj.complexity, voteObj.accountId);
    });

    return () => {
      disconnectFromEndpointFor(signalRClient, "UpdateStoriesOfMatchWith");
      disconnectFromEndpointFor(signalRClient, "SomeoneVoted");
    };
  }, []);

  const hasVotedFor = (
    complexity: number,
    participant: Participant | undefined,
  ) => {
    if (!participant) {
      return false;
    }

    const hasVoted = participant.votes.some(
      (v: Vote) =>
        v.storyId == currentShowingStory?.storyId &&
        v.hasVotedAlready &&
        complexity == v.points,
    );
    return hasVoted;
  };

  const votePointsAs = (points: number) => {
    voteForStory(matchId, currentShowingStory!.storyId, points)
      .then(() => {
        showSuccess(`Voted Story with a complexity about ${points} points`);
      })
      .catch(() => {
        showError(`Could not vote story..., try again soon`);
      });
  };

  return (
    <Stack sx={{ height: "100vh" }}>
      <Grid2 container sx={{ height: "100%" }}>
        <Grid2 size={3} zIndex={6}>
          <StoriesListForm />
        </Grid2>
        <Grid2 size={9} sx={{ padding: 6 }}>
          <Stack spacing={4}>
            <CurrenlyShowingStoryViewer stories={stories} />
            <Grid2>
              <Grid2
                container
                spacing={2}
                wrap="nowrap"
                sx={{ overflowX: "auto", padding: 2 }}
              >
                {currentShowingStory !== null &&
                  complexities.map((complexity) => (
                    <Grid2 key={complexity.points} xs="auto">
                      <Button
                        sx={{ background: "#0000" }}
                        onClick={() => votePointsAs(complexity.points)}
                      >
                        <Card
                          sx={{
                            padding: 2,
                            textAlign: "center",
                            cursor: "pointer",
                            height: 150,
                            display: "flex",
                            flexDirection: "column",
                            justifyContent: "center",
                            "&:hover": {
                              boxShadow: 6,
                            },
                            background: hasVotedFor(
                              complexity.points,
                              participants.find(
                                (p) => p.accountId == accountId,
                              ),
                            )
                              ? "#88d"
                              : "#fafafa",
                          }}
                        >
                          <Typography variant="h5" fontWeight="bold">
                            {complexity.points}
                          </Typography>
                          <Typography variant="body2" color="text.secondary">
                            {complexity.description}
                          </Typography>
                        </Card>
                      </Button>
                    </Grid2>
                  ))}
              </Grid2>
            </Grid2>
          </Stack>
        </Grid2>
      </Grid2>
    </Stack>
  );
}
