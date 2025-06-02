import {
  Button,
  Stack,
  Container,
  Typography,
  TextField,
  Divider,
  IconButton,
  InputAdornment,
} from "@mui/material";
import { Google, Visibility, VisibilityOff } from "@mui/icons-material";
import { getAuth, GoogleAuthProvider, signInWithPopup } from "firebase/auth";
import firebase from "../../../drivers/firebase";
import { useState } from "react";
import { UseSaveSession } from "../../../shared/hooks/integrations/api/auth/save-session.integration";
import { useAutoLogin } from "../../../shared/hooks/integrations/api/auth/auto-login.integration";
import { useForm } from "react-hook-form";
import { useEmailAndPasswordLogin } from "../../../shared/hooks/integrations/api/auth/use-email-and-password-login-hook";

type EmailAndPasswordLoginForm = {
  email: string;
  password: string;
};

export function SignInForm() {
  const { saveSession } = UseSaveSession();
  const { registerAutoLogin } = useAutoLogin();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<EmailAndPasswordLoginForm>({});
  const [isShowingPassword, setIsShowingPassword] = useState(false);
  const toggleShowPassword = () => setIsShowingPassword((v) => !v);

  const signInGoogle = () => {
    const googleProvider = new GoogleAuthProvider();
    const auth = getAuth(firebase);

    signInWithPopup(auth, googleProvider)
      .then((userCredential) => {
        GoogleAuthProvider.credentialFromResult(userCredential);

        userCredential.user
          .getIdToken()
          .then((token) => saveSession(token))
          .then(registerAutoLogin)
          // .then(useNavigate())
          .catch((error) => {
            throw error;
          });
      })
      .catch((reject) => {
        // setIsAuthenticating(false);
        throw reject;
      });
  };

  const { loginWithEmailAndPassword, isLogin: isLoginUserAndPassword } =
    useEmailAndPasswordLogin();

  // const {} = useGoogleFirebaseSSOLogin();
  // STOPED HERE

  return (
    <form onSubmit={handleSubmit(loginWithEmailAndPassword)}>
      <Container sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
        <TextField
          fullWidth
          type={"email"}
          id="email"
          label="E-mail"
          helperText={errors.email?.message}
          error={!!errors.email?.message}
          {...register("email", {
            required: {
              value: true,
              message: "E-mail is required",
            },
            pattern: {
              value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
              message: "Enter a valid e-mail",
            },
            maxLength: {
              value: 60,
              message: "E-mail must have a max of 60 characters",
            },
          })}
        />

        <TextField
          fullWidth
          type={isShowingPassword ? "text" : "password"}
          id="password"
          label="Password"
          slotProps={{
            input: {
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton onClick={toggleShowPassword}>
                    {isShowingPassword ? <VisibilityOff /> : <Visibility />}
                  </IconButton>
                </InputAdornment>
              ),
            },
          }}
          helperText={errors.password?.message}
          error={!!errors.password?.message}
          {...register("password", {
            required: {
              value: true,
              message: "Password is required",
            },
            pattern: {
              value:
                /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$/,
              message:
                "Password must have at least 8 characters, one uppercase letter, one lowercase letter, one number and one special character",
            },
            maxLength: {
              value: 512,
              message: "Password must have a max of 512 characters",
            },
          })}
        />
        <Button type="submit" fullWidth variant="contained">
          Enter
        </Button>

        <Divider />

        <Stack>
          <Typography variant="h6" align="center">
            SSO
          </Typography>
          <Stack direction={"row"} justifyContent={"center"}>
            <IconButton type="button">
              <Google sx={{ color: "#4285F4" }} />
            </IconButton>
          </Stack>
        </Stack>
      </Container>
    </form>
  );
  /* <Stack>
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
      </Stack> */
}
