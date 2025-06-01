import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { RouterProvider } from "react-router";
import { router } from "./router";
import { ThemeProvider } from "@emotion/react";
import { CssBaseline } from "@mui/material";
import theme from "./theme.mui";
import { SnackbarProvider } from "./shared/ui/snackbar";
import { ConfirmationProvider } from "./shared/ui/confirmation-dialog";
import { NotificationErrorBoundary } from "./shared/middlewares/error-boundary.middleware";
import "./main.css";

// Add this near the top of the file
window.addEventListener("error", (event) => {
  // Access your snackbar store directly
  import("./shared/ui/snackbar").then(({ useSnackbarStore }) => {
    useSnackbarStore
      .getState()
      .enqueueSnackbar(event.reason?.message || "An error occurred", "error");
  });
});

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <SnackbarProvider>
        <NotificationErrorBoundary>
          <ConfirmationProvider>
            <RouterProvider router={router} />
          </ConfirmationProvider>
        </NotificationErrorBoundary>
      </SnackbarProvider>
    </ThemeProvider>
  </StrictMode>,
);
