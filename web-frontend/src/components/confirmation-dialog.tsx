import { create } from "zustand";
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  IconButton,
} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";

interface ConfirmationOptions {
  title?: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  severity?: "warning" | "error" | "info" | "success";
}

interface ConfirmationDialogState {
  isOpen: boolean;
  options: ConfirmationOptions;
  onConfirm: (() => void) | null;
  onCancel: (() => void) | null;

  // Actions
  showConfirmation: (
    options: ConfirmationOptions,
    onConfirm?: () => void,
    onCancel?: () => void,
  ) => void;

  confirm: () => void;
  cancel: () => void;
  close: () => void;
}

export const useConfirmationDialogStore = create<ConfirmationDialogState>(
  (set, get) => ({
    isOpen: false,
    options: {
      title: "Confirmation",
      message: "Are you sure?",
      confirmText: "Confirm",
      cancelText: "Cancel",
      severity: "warning",
    },
    onConfirm: null,
    onCancel: null,

    showConfirmation: (options, onConfirm = () => {}, onCancel = () => {}) => {
      set({
        isOpen: true,
        options: { ...get().options, ...options },
        onConfirm,
        onCancel,
      });
    },

    confirm: () => {
      const { onConfirm } = get();
      if (onConfirm) onConfirm();
      set({ isOpen: false });
    },

    cancel: () => {
      const { onCancel } = get();
      if (onCancel) onCancel();
      set({ isOpen: false });
    },

    close: () => {
      set({ isOpen: false });
    },
  }),
);

export function ConfirmationDialog() {
  const { isOpen, options, confirm, cancel, close } =
    useConfirmationDialogStore();

  const {
    title = "Confirmation",
    message,
    confirmText = "Confirm",
    cancelText = "Cancel",
    severity = "warning",
  } = options;

  const getColorBySeverity = () => {
    switch (severity) {
      case "error":
        return "error";
      case "warning":
        return "warning";
      case "success":
        return "success";
      case "info":
      default:
        return "primary";
    }
  };

  return (
    <Dialog
      open={isOpen}
      onClose={cancel}
      maxWidth="sm"
      fullWidth
      aria-labelledby="confirmation-dialog-title"
      aria-describedby="confirmation-dialog-description"
    >
      <DialogTitle id="confirmation-dialog-title" sx={{ pr: 6 }}>
        {title}
        <IconButton
          aria-label="close"
          onClick={close}
          sx={{
            position: "absolute",
            right: 8,
            top: 8,
            color: (theme) => theme.palette.grey[500],
          }}
        >
          <CloseIcon />
        </IconButton>
      </DialogTitle>
      <DialogContent>
        <DialogContentText id="confirmation-dialog-description">
          {message}
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={cancel} color="inherit" variant="outlined">
          {cancelText}
        </Button>
        <Button
          onClick={confirm}
          color={getColorBySeverity()}
          variant="contained"
          autoFocus
        >
          {confirmText}
        </Button>
      </DialogActions>
    </Dialog>
  );
}

export function ConfirmationProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <>
      {children}
      <ConfirmationDialog />
    </>
  );
}

export function useConfirmation() {
  const { showConfirmation } = useConfirmationDialogStore();

  const confirm = (options: {
    title?: string;
    message: string;
    confirmText?: string;
    cancelText?: string;
    severity?: "warning" | "error" | "info" | "success";
  }): Promise<boolean> => {
    return new Promise((resolve) => {
      showConfirmation(
        options,
        () => resolve(true),
        () => resolve(false),
      );
    });
  };

  return { confirm };
}
