import { Card, Container } from "@mui/material";
import { SignInForm } from "../../../forms/auth/sign-in/sign-in.form";

export function SignInPage() {
  return (
    <>
      <Container>
        <Card sx={{ paddingX: 4, paddingY: 2 }}>
          <SignInForm />
        </Card>
      </Container>
    </>
  );
}
