import {
  Box,
  CircularProgress,
  Container,
  Stack,
  Typography,
} from "@mui/material";
import { useSignalRContext } from "../../contexts/signalr.context";
import { useEffect } from "react";
import { SignalRMatchHubClientEndpoints, SignalRMatchHubServerEndpoints } from "../../consts/signalr/signalr-match-hub.endpoints";
import { useNavigate, useParams } from "react-router";

export function JoinMatchPage() {
  const { matchId } = useParams();

  const navigate = useNavigate();

  const { invokeAsyncFor, registerEndpointFor, signalRClient } = useSignalRContext();

  useEffect(() => {
  console.log("Registering ApproveJoinRequest handler...");
  registerEndpointFor(signalRClient, SignalRMatchHubClientEndpoints.ApproveJoinRequest, () => {
    console.log("ApproveJoinRequest received!");
    navigate(`/matches/party/${matchId}`);
  }).then(() => {
    console.log("ApproveJoinRequest handler registered, invoking JoinMatch...");
    invokeAsyncFor(
      signalRClient,
      SignalRMatchHubServerEndpoints.JoinMatch,
      Number(matchId),
    );
  });
},  []);
  return (
    <Container>
      <Box
        sx={{ display: "flex", justifyContent: "center", alignItems: "center" }}
      >
        <Stack spacing={2} direction="row">
          <CircularProgress />
          <Box sx={{ display: "flex", alignItems: "center" }}>
            <Typography variant="h4"> Joining...</Typography>
          </Box>
        </Stack>
      </Box>
    </Container>
  );
}
