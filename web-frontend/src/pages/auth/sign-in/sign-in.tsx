import { Card, Container, Divider, Stack, Typography } from "@mui/material";
import { SignInForm } from "../../../features/auth/sign-in/sign-in.form";
import { RoutesDef } from "../../../shared/consts/routesDef";
import { Link } from "react-router";

export function SignInPage() {
  return (
    <Container sx={{ display: "flex", flexDirection: "column", gap: 4 }}>
      <Card sx={{ py: 2 }}>
        <Container sx={{ pb: 4 }}>
          <Stack gap={2}>
            <Stack>
              <Typography variant="h4" align="center">
                Sign in
              </Typography>
              <Typography variant="subtitle2" align="center">
                Start planning your sprints
              </Typography>
            </Stack>
            <Divider />
          </Stack>
        </Container>
        <SignInForm />
      </Card>

      <Card
        sx={{
          p: 2,
          textWrap: "wrap",
          display: "flex",
          justifyContent: "center",
        }}
      >
        <Typography
          color="#888"
          variant="button"
          component={"span"}
          align="center"
        >
          Don't have a Account?{" "}
          <Link style={{ display: "inline" }} to={RoutesDef.CreateAccount}>
            <Typography variant="button" color="primary">
              Click here to create one
            </Typography>
          </Link>
        </Typography>
      </Card>
    </Container>
  );
}
