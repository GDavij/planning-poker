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
import { listUserCreatedMatches } from "../../services/match.service";
import { ListMatchesQueryResponse } from "../../models/matches";
import { useNavigate } from "react-router";

export function HomeDashboard() {
  const [abortController] = useState(new AbortController());

  const navigate = useNavigate();

  const [isLoadingCreatedMatches, setIsLoadingCreatedMatches] = useState(false);

  const [userCreatedMatches, setUserCreatedMatches] = useState<
    ListMatchesQueryResponse[]
  >([]);

  const loadTop10CreatedMatches = () => {
    setIsLoadingCreatedMatches(true);

    listUserCreatedMatches(1, 3, abortController)
      .then((res) => setUserCreatedMatches(res.data))
      .catch(() => {})
      .finally(() => setIsLoadingCreatedMatches(false));
  };

  useEffect(() => {
    loadTop10CreatedMatches();
  }, []);

  const createMatch = () => navigate("/dashboard/matches/new");

  return (
    <>
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
              <Button variant="outlined" onClick={() => {}}>
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
              <Typography variant="h6">Created Matches</Typography>
            </Divider>

            <Stack spacing={2}>
              {isLoadingCreatedMatches
                ? [1, 2, 3].map(() => <Skeleton width="100%" height={120} />)
                : userCreatedMatches.map((match) => (
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
                          <Typography fontWeight={700}>
                            Copy Share Link
                          </Typography>
                        </Button>
                        <Button variant="contained">
                          <Typography fontWeight={700}>Join Match</Typography>
                        </Button>
                      </Box>
                    </Card>
                  ))}

              <Button fullWidth> See all Matches </Button>
            </Stack>
          </Box>

          <Box>
            <Divider textAlign="center">
              <Typography variant="h6">Joined Matches</Typography>
            </Divider>
          </Box>
        </Card>
      </Container>
    </>
  );
}
