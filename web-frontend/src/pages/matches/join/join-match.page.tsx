import {
  Box,
  CircularProgress,
  Container,
  Stack,
  Typography,
} from "@mui/material";
import { useSignalRContext } from "../../../contexts/signalr.context";
import { useEffect } from "react";
import {
  SignalRMatchHubClientEndpoints,
  SignalRMatchHubServerEndpoints,
} from "../../../consts/signalr/signalr-match-hub.endpoints";
import { useNavigate, useParams } from "react-router";

export function JoinMatchPage() {
  const { matchId } = useParams();

  const navigate = useNavigate();

  const {
    invokeAsyncFor,
    registerEndpointFor,
    disconnectFromEndpointFor,
    signalRClient,
  } = useSignalRContext();

  useEffect(() => {
    disconnectFromEndpointFor(
      signalRClient,
      SignalRMatchHubClientEndpoints.ApproveJoinRequest,
    ).then(() => {
      registerEndpointFor(
        signalRClient,
        SignalRMatchHubClientEndpoints.ApproveJoinRequest,
        () => {
          navigate(`/dashboard/matches/party/${matchId}`);
        },
      ).then(() => {
        invokeAsyncFor(
          signalRClient,
          SignalRMatchHubServerEndpoints.JoinMatch,
          Number(matchId),
        );
      });
    });
  }, []);
  return (
    <Box
      sx={{
        padding: "32px",
        margin: "16px auto",
        maxWidth: "1200px",
      }}
    >
      <Container>
        <Box
          sx={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
          }}
        >
          <Stack spacing={2} direction="row">
            <CircularProgress />
            <Box sx={{ display: "flex", alignItems: "center" }}>
              <Typography variant="h4"> Joining...</Typography>
            </Box>
          </Stack>
        </Box>
      </Container>
    </Box>
  );
}
