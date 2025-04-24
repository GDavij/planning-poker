import { Container, Grid2, Stack, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { useSignalRContext } from "../../../contexts/signalr.context";
import { Story } from "../../../models/matches";
import { listMatchStories } from "../../../services/match.service";
import { useParams } from "react-router";
import { StoriesListForm } from "../../../forms/stories/stories-list.form";

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
  const gridSpacing = 12 / complexities.length;

  const matchId = Number(useParams().matchId);

  const { signalRClient, registerEndpointFor, invokeAsyncFor } =
    useSignalRContext();

  const [stories, setStories] = useState<Story[]>([]);

  useEffect(() => {
    listMatchStories(matchId).then((stories) => {
      setStories(stories);
    });

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
        <Grid2 size={2}>
          <StoriesListForm />
        </Grid2>
        <Grid2 size={10}></Grid2>
      </Grid2>
    </Stack>
  );
}
