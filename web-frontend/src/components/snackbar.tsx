import { create } from "zustand";
import React from "react";
import { Snackbar, Alert } from "@mui/material";

export type SnackbarSeverity = "success" | "error" | "warning" | "info";

export interface SnackbarMessage {
  id: string;
  message: string;
  severity: SnackbarSeverity;
  duration?: number;
}

interface SnackbarState {
  queue: SnackbarMessage[];
  current: SnackbarMessage | null;
  open: boolean;

  // Actions
  enqueueSnackbar: (
    message: string,
    severity?: SnackbarSeverity,
    duration?: number,
  ) => void;
  closeSnackbar: () => void;
  processQueue: () => void;
}

export const useSnackbarStore = create<SnackbarState>((set, get) => ({
  queue: [],
  current: null,
  open: false,

  enqueueSnackbar: (message, severity = "success", duration = 5000) => {
    const newSnackbar: SnackbarMessage = {
      id: Date.now().toString(),
      message,
      severity,
      duration,
    };

    set((state) => ({ queue: [...state.queue, newSnackbar] }));

    // If there's no current snackbar showing, process the queue
    if (!get().current) {
      get().processQueue();
    }
  },

  closeSnackbar: () => {
    set({ open: false });

    // After closing animation, process next in queue
    setTimeout(() => {
      get().processQueue();
    }, 300); // Delay to allow close animation
  },

  processQueue: () => {
    set((state) => {
      if (state.queue.length === 0) {
        return { current: null, open: false };
      }

      // Get the next snackbar from the queue
      const [nextSnackbar, ...remainingQueue] = state.queue;

      // Set timer to auto-close this snackbar
      if (nextSnackbar.duration) {
        setTimeout(() => {
          get().closeSnackbar();
        }, nextSnackbar.duration);
      }

      return {
        current: nextSnackbar,
        queue: remainingQueue,
        open: true,
      };
    });
  },
}));

export function SnackbarProvider({ children }: { children: React.ReactNode }) {
  const { current, open, closeSnackbar } = useSnackbarStore();

  return (
    <>
      {children}

      <Snackbar
        open={open}
        autoHideDuration={current?.duration}
        onClose={() => closeSnackbar()}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert
          onClose={() => closeSnackbar()}
          severity={current?.severity}
          variant="filled"
          elevation={6}
          sx={{ width: "100%" }}
        >
          {current?.message}
        </Alert>
      </Snackbar>
    </>
  );
}

export function useSnackbar() {
  const { enqueueSnackbar } = useSnackbarStore();

  return {
    showSnackbar: (
      message: string,
      severity?: SnackbarSeverity,
      duration?: number,
    ) => enqueueSnackbar(message, severity, duration),
    showSuccess: (message: string, duration?: number) =>
      enqueueSnackbar(message, "success", duration),
    showError: (message: string, duration?: number) =>
      enqueueSnackbar(message, "error", duration),
    showWarning: (message: string, duration?: number) =>
      enqueueSnackbar(message, "warning", duration),
    showInfo: (message: string, duration?: number) =>
      enqueueSnackbar(message, "info", duration),
  };
}
