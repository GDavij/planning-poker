import React from "react";
import { Outlet } from "react-router";
import { SnackbarHook, useSnackbar } from "../ui/snackbar";

export function NotificationErrorBoundary(props: {
  children: React.ReactNode;
}) {
  const snackbar = useSnackbar();

  return (
    <NotificationErrorBoundaryHandler snackbar={snackbar}>
      {props.children}
    </NotificationErrorBoundaryHandler>
  );
}

class NotificationErrorBoundaryHandler extends React.Component<{
  snackbar: SnackbarHook;
  children: React.ReactNode;
}> {
  override componentDidCatch(error: Error, errorInfo: React.ErrorInfo): void {
    console.log({ error, errorInfo });
    this.props.snackbar.showError(error.message, 1000);
  }

  override render(): React.ReactNode {
    return this.props.children;
  }
}
