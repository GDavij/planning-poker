import {
  Box,
  Button,
  Card,
  Container,
  Divider,
  Skeleton,
  Stack,
  Typography,
} from "@mui/material";
import { useEffect, useState } from "react";
import { useListUserCreatedMatches } from "../../shared/hooks/integrations/api/match.service";
import { ListMatchesQueryResponse } from "../../shared/models/matches";
import { useNavigate } from "react-router";
import { useJoinMatchModal } from "../../features/stories/join-match.modal.form";

export function HomeDashboard() {
  const [abortController] = useState(new AbortController());

  const navigate = useNavigate();

  const [isLoadingCreatedMatches, setIsLoadingCreatedMatches] = useState(false);

  const { open: openJoinModal } = useJoinMatchModal();

  const { matches, isFetching } = useListUserCreatedMatches(
    1,
    50,
    abortController,
  );

  const createMatch = () => navigate("/dashboard/matches/new");

  const joinMatch = (match: ListMatchesQueryResponse) =>
    navigate(`/dashboard/matches/join/${match.matchId}`);

  const copyShareLinkForMatch = (match: ListMatchesQueryResponse) => {
    const shareLink = `${window.location.origin}/dashboard/matches/join/${match.matchId}`;

    navigator.clipboard
      .writeText(shareLink)
      .then(() => {
        alert("Share link copied to clipboard");
      })
      .catch((err) => {
        alert("An Error occurred when coping from clipboard");
      });
  };

  return (
    <>
      <Box
        sx={{
          padding: "32px",
          margin: "16px auto",
          maxWidth: "1200px",
        }}
      >
        <Container>
          <Card
            sx={{
              px: 4,
              py: 2,
              display: "flex",
              flexDirection: "column",
              gap: 6,
            }}
          >
            <Box sx={{ display: "flex", justifyContent: "space-between" }}>
              <Typography variant="h4">Matches</Typography>

              <Stack direction="row" spacing={2}>
                <Button variant="outlined" onClick={() => openJoinModal(null)}>
                  {" "}
                  Join a Match
                </Button>

                <Button variant="contained" onClick={createMatch}>
                  {" "}
                  Create a new Match{" "}
                </Button>
              </Stack>
            </Box>

            <Box>
              <Divider textAlign="center">
                <Typography variant="h6">Active matches</Typography>
              </Divider>

              <Stack spacing={2}>
                {isFetching
                  ? [1, 2, 3].map((cardIndex) => (
                      <Skeleton key={cardIndex} width="100%" height={120} />
                    ))
                  : matches.map((match) => (
                      <Card
                        sx={{
                          widows: "100%",
                          height: 120,
                          px: 4,
                          py: 2,
                          display: "flex",
                          flexDirection: "column",
                          justifyContent: "space-between",
                          border: "1px solid #eee",
                        }}
                        key={match.matchId}
                        elevation={3}
                      >
                        <Box>
                          <Typography variant="h6" fontWeight={700}>
                            {match.description}
                          </Typography>
                        </Box>

                        <Box
                          sx={{
                            display: "flex",
                            justifyContent: "flex-end",
                            gap: 2,
                          }}
                        >
                          <Button>
                            <Typography
                              fontWeight={700}
                              onClick={() => copyShareLinkForMatch(match)}
                            >
                              Copy Share Link
                            </Typography>
                          </Button>
                          <Button
                            variant="contained"
                            onClick={() => joinMatch(match)}
                          >
                            <Typography fontWeight={700}>Join Match</Typography>
                          </Button>
                        </Box>
                      </Card>
                    ))}
              </Stack>
            </Box>
          </Card>
        </Container>
      </Box>
    </>
  );
}
