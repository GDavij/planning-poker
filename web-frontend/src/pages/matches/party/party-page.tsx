import { Card, Container, Grid2, Stack, Typography } from "@mui/material";
import { memo, useEffect, useState } from "react";
import { useSignalRContext } from "../../../contexts/signalr.context";
import { Story } from "../../../models/matches";
import { listMatchStories } from "../../../services/match.service";
import { useParams } from "react-router";
import { StoriesListForm } from "../../../forms/stories/stories-list.form";
import { SignalRMatchHubServerEndpoints } from "../../../consts/signalr/signalr-match-hub.endpoints";
import { AppCard } from "../../../components/app-card";
import { HighlightPaper } from "../../../components/app-paper";
import { useSnackbar } from "../../../components/snackbar";
import { useDebounce } from "../../../hooks/debounce";
import { useSpring, animated } from "@react-spring/web";
import { CurrenlyShowingStoryViewer } from "../../../forms/stories/currentVotingStory.form";

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

  const { signalRClient, registerEndpointFor, invokeAsyncFor } =
    useSignalRContext();

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
        console.log({ newStoriesFromSignalR: stories }); // Only for test purposes
        setStories(stories as Story[]);
      },
    );
  }, []);

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
                {complexities.map((complexity) => (
                  <Grid2 key={complexity.points} xs="auto">
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
                      }}
                    >
                      <Typography variant="h5" fontWeight="bold">
                        {complexity.points}
                      </Typography>
                      <Typography variant="body2" color="text.secondary">
                        {complexity.description}
                      </Typography>
                    </Card>
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
