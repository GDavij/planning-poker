import {
  Button,
  Stack,
  Container,
  Typography,
  CircularProgress,
  Alert,
} from "@mui/material";
import { Google } from "@mui/icons-material";
import { getAuth, GoogleAuthProvider, signInWithPopup } from "firebase/auth";
import firebase from "../../../drivers/firebase";
import { autoLogin, saveSession } from "../../../services/auth.service";
import { useNavigate } from "react-router";
import { useState } from "react";
import { useSnackbar } from "../../../components/snackbar";

export function SignInForm() {
  const navigate = useNavigate();
  const [isAuthenticating, setIsAuthenticating] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const { showError } = useSnackbar();

  const redirectToDashboard = () => navigate("/dashboard");

  const signInGoogle = () => {
    setIsAuthenticating(true);
    setErrorMessage(null);
    const googleProvider = new GoogleAuthProvider();
    const auth = getAuth(firebase);

    signInWithPopup(auth, googleProvider)
      .then((userCredential) => {
        GoogleAuthProvider.credentialFromResult(userCredential);

        userCredential.user
          .getIdToken()
          .then((token) => saveSession(token))
          .then(autoLogin)
          .then(redirectToDashboard)
          .catch((error) => {
            console.error({ error });
            setIsAuthenticating(false);
            setErrorMessage("An error occurred while processing your login.");
          });
      })
      .catch((reject) => {
        console.error({ reject });
        setIsAuthenticating(false);
        setErrorMessage("Failed to Login with Google.");
        showError("Failed to Login with Google.");
      });
  };

  return (
    <Stack spacing={3} alignItems="center">
      <Container>
        <Typography variant="h4" align="center">
          Sign in
        </Typography>
        <Typography variant="subtitle2" align="center">
          Start planning your sprints
        </Typography>
      </Container>

      {errorMessage && (
        <Container>
          <Alert severity="error">{errorMessage}</Alert>
        </Container>
      )}

      <Stack>
        <Container sx={{ display: "flex", justifyContent: "center" }}>
          <Button
            onClick={signInGoogle}
            disabled={isAuthenticating}
            variant="contained"
            color="primary"
            startIcon={<Google />}
            sx={{
              padding: "10px 20px",
              fontSize: "16px",
              fontWeight: "bold",
              textTransform: "none",
              boxShadow: 3,
            }}
          >
            {isAuthenticating ? (
              <CircularProgress size={24} color="inherit" />
            ) : (
              "Login with Google"
            )}
          </Button>
        </Container>
      </Stack>
    </Stack>
  );
}
