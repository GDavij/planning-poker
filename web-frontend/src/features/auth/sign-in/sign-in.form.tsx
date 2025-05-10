import {
  Button,
  Stack,
  Container,
  Typography,
  CircularProgress,
} from "@mui/material";
import { Google } from "@mui/icons-material";
import { getAuth, GoogleAuthProvider, signInWithPopup } from "firebase/auth";
import firebase from "../../../drivers/firebase";
import { useNavigate } from "react-router";
import { useState } from "react";
import { UseSaveSession } from "../../../shared/hooks/integrations/api/auth/save-session.integration";
import { useAutoLogin } from "../../../shared/hooks/integrations/api/auth/auto-login.integration";

export function SignInForm() {
  const navigate = useNavigate();
  const { saveSession } = UseSaveSession();
  const { registerAutoLogin } = useAutoLogin();

  const [isAuthenticating, setIsAuthenticating] = useState(false);

  const redirectToDashboard = () => navigate("/dashboard");

  const signInGoogle = () => {
    setIsAuthenticating(true);
    const googleProvider = new GoogleAuthProvider();
    const auth = getAuth(firebase);

    signInWithPopup(auth, googleProvider)
      .then((userCredential) => {
        GoogleAuthProvider.credentialFromResult(userCredential);

        userCredential.user
          .getIdToken()
          .then((token) => saveSession(token))
          .then(registerAutoLogin)
          .then(redirectToDashboard)
          .catch((error) => {
            setIsAuthenticating(false);
            throw error;
          });
      })
      .catch((reject) => {
        setIsAuthenticating(false);
        throw reject;
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
