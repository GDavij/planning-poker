import {
  Box,
  CircularProgress,
  Container,
  Stack,
  Typography,
} from "@mui/material";
import { useEffect } from "react";
import { useNavigate, useParams } from "react-router";
import { useSignalRContext } from "../../../shared/contexts/signalr.context";
import { SignalRHooks } from "../../../shared/consts/signalRHooks";
import { SignalRPorts } from "../../../shared/consts/signalRPorts";

export function JoinMatchPage() {
  const { matchId } = useParams();

  const navigate = useNavigate();

  const { invokeAsyncFor, registerEndpointFor } = useSignalRContext();

  useEffect(() => {
    registerEndpointFor(SignalRHooks.OnApproveJoinRequest, () => {
      navigate(`/dashboard/matches/party/${matchId}`);
    }).then(() => {
      invokeAsyncFor(SignalRPorts.ToJoinMatch, Number(matchId));
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
