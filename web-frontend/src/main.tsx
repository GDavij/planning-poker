import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { RouterProvider } from "react-router";
import { routes } from "./routes";
import { ThemeProvider } from "@emotion/react";
import { CssBaseline } from "@mui/material";
import theme from "./theme.mui";
import { SnackbarProvider } from "./components/snackbar";
import { ConfirmationProvider } from "./components/confirmation-dialog";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <SnackbarProvider>
        <ConfirmationProvider>
          <RouterProvider router={routes} />
        </ConfirmationProvider>
      </SnackbarProvider>
    </ThemeProvider>
  </StrictMode>,
);
