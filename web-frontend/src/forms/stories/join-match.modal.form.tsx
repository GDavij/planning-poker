import { create } from "zustand";
import { ModalHandlerState } from "../../models/form";
import { Box, Button, Card, Modal, Stack, TextField } from "@mui/material";
import { useForm } from "react-hook-form";
import { useEffect, useState } from "react";
import { useSnackbar } from "../../components/snackbar";
import { useNavigate } from "react-router";

export const useJoinMatchModal = create<ModalHandlerState<unknown>>()(
  (set) => ({
    open: () => set({ isOpen: true }),
    close: () => set({ isOpen: false }),
    isOpen: false,
  }),
);

interface JoinMatchForm {
  matchNumber: number;
}

export function JoinMatchModal() {
  const { isOpen, close } = useJoinMatchModal();
  const navigate = useNavigate();

  const [isJoiningMatch, setIsJoiningMatch] = useState(false);

  const {
    register,
    formState: { errors },
    handleSubmit,
    reset,
  } = useForm<JoinMatchForm>();

  useEffect(() => {
    reset();
    setIsJoiningMatch(false);
  }, [isOpen]);

  const join = (form: JoinMatchForm) => {
    setIsJoiningMatch(true);

    close();

    navigate(`/dashboard/matches/join/${form.matchNumber}`);
  };

  return (
    <Modal open={isOpen} onClose={close}>
      <Box
        sx={{
          width: 400,
          height: 200,
          position: "absolute",
          top: "50%",
          left: "50%",
          transform: "translate(-50%, -50%)",
          maxWidth: "90%",
        }}
      >
        <Card sx={{ paddingX: 4, paddingY: 2 }}>
          <Stack spacing={4}>
            <Stack direction={"row"} justifyContent={"flex-end"}>
              <Button
                variant="outlined"
                onClick={close}
                disabled={isJoiningMatch}
              >
                Close
              </Button>
            </Stack>
            <form onSubmit={handleSubmit(join)}>
              <Stack spacing={2}>
                <TextField
                  label="Match Number"
                  type="number"
                  {...register("matchNumber", {
                    required: {
                      value: true,
                      message: "Match number is required",
                    },
                    min: {
                      value: 1,
                      message: "Match number must be greater than 0",
                    },
                  })}
                  error={!!errors.matchNumber}
                  helperText={errors.matchNumber?.message}
                />

                <Button
                  variant="contained"
                  type="submit"
                  disabled={isJoiningMatch}
                >
                  Join Match
                </Button>
              </Stack>
            </form>
          </Stack>
        </Card>
      </Box>
    </Modal>
  );
}
