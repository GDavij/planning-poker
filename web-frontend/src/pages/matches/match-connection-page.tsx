import { Outlet } from "react-router";
import { SignalRHubs } from "../../consts/signalr/signalRHubs";
import {
  SignalRContextProvider,
  useSignalRContext,
} from "../../contexts/signalr.context";
import { useCallback, useEffect, useState } from "react";
import { SignalRMatchHubClientEndpoints } from "../../consts/signalr/signalr-match-hub.endpoints";
import { Snackbar, SnackbarCloseReason } from "@mui/material";
import { Notification } from "../../models/notification";

export function MatchGamePage() {
  return (
    <SignalRContextProvider hubName={SignalRHubs.MatchHub}>
      <SignalRErrorPopUpHandler>
        <Outlet />
      </SignalRErrorPopUpHandler>
    </SignalRContextProvider>
  );
}

type SignalRErrorPopUpHandler = { children: React.ReactNode };

function SignalRErrorPopUpHandler({ children }: SignalRErrorPopUpHandler) {
  const { registerEndpointFor, signalRClient } = useSignalRContext();

  const [errorMessage, setErrorMessage] = useState<string>("");
  const haveError = useCallback(() => {
    return !!errorMessage ? true : false;
  }, [errorMessage]);

  useEffect(() => {
    registerEndpointFor(
      signalRClient,
      SignalRMatchHubClientEndpoints.ReceiveErrorAsync,
      (notification) => {
        const notificationObj = notification as Notification;
        setErrorMessage(notificationObj.message);
        setTimeout(() => {
          setErrorMessage("");
        });
      },
    );
  });

  const handleClose = (
    event: Event | React.SyntheticEvent,
    reason: SnackbarCloseReason,
  ) => {
    if (reason == "clickaway") {
      return;
    }

    setErrorMessage("");
  };

  return (
    <>
      {children}
      <Snackbar
        open={haveError()}
        autoHideDuration={4000}
        onClose={handleClose}
        message={errorMessage}
      />
    </>
  );
}
