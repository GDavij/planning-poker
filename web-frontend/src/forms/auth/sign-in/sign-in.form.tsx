import { useForm } from "react-hook-form";
import { TextField, Button, Stack, Container, Typography } from "@mui/material";
import { Google, Microsoft } from "@mui/icons-material";
import { getAuth, GoogleAuthProvider, signInWithPopup } from "firebase/auth";
import firebase from "../../../drivers/firebase";
import { autoLogin, saveSession } from "../../../services/auth.service";
import { useNavigate } from "react-router";

type AuthUserFormData = {
  email: string;
  password: string;
};

export function SignInForm() {
  const form = useForm<AuthUserFormData>();
  const { register, handleSubmit, formState } = form;
  const { errors } = formState;

  const navigate = useNavigate();

  const redirectToDashboard = () => navigate("/dashboard");

  const signInEmailPassword = (data: AuthUserFormData) => {
    console.log(data);
  };

  const signInGoogle = () => {
    const googleProvider = new GoogleAuthProvider();

    const auth = getAuth(firebase);

    signInWithPopup(auth, googleProvider)
      .then((userCredential) => {
        const accessInfo =
          GoogleAuthProvider.credentialFromResult(userCredential);

        console.log({
          accessInfo,
          userCredential,
          token: userCredential.user.getIdToken(),
        });

        console.log({ cookie: document.cookie });

        userCredential.user
          .getIdToken()
          .then((token) => saveSession(token))
          .then(autoLogin)
          .then(redirectToDashboard)
          .catch((error) => console.error({ error }));
      })
      .catch((reject) => {
        console.error({ reject });
      });
  };

  const signInMicrosoft = () => {};

  return (
    <Stack spacing={2}>
      <Container>
        <Typography variant="h4">Sign in</Typography>
        <Typography variant="subtitle2">Start planning your sprints</Typography>
      </Container>

      <form noValidate onSubmit={handleSubmit(signInEmailPassword)}>
        <Stack spacing={2}>
          <TextField
            label="Email"
            type="email"
            {...register("email", {
              required: "Email is required",
              pattern: {
                value: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
                message: "Enter a valid email address",
              },
              maxLength: {
                value: 255,
                message: "Email must have a max of 255 characters",
              },
            })}
            error={!!errors.email}
            helperText={errors.email?.message}
          />
          <TextField
            label="Password"
            type="password"
            {...register("password", {
              required: "Password is required",
              minLength: {
                value: 16,
                message: "Password must have a minimum of 16 characters",
              },
              maxLength: {
                value: 255,
                message: "Password must have a maximum of 255 characters",
              },
            })}
            error={!!errors.password}
            helperText={errors.password?.message}
          />

          <Button type="submit" variant="contained" color="primary">
            Sign In
          </Button>
        </Stack>
      </form>

      <Stack>
        <Container sx={{ display: "flex", justifyContent: "center" }}>
          <Button onClick={signInGoogle}>
            <Google />
          </Button>
          <Button>
            <Microsoft />
          </Button>
        </Container>
      </Stack>

      <Stack>
        <Button variant="text" href="/sign-up">
          <Typography> Create an Account</Typography>
        </Button>
      </Stack>
    </Stack>
  );
}
