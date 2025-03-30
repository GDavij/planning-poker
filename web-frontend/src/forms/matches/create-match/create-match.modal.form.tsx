import {
  Modal,
  Box,
  Card,
  Typography,
  Stack,
  TextField,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
} from "@mui/material";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { startMatch } from "../../../services/match.service";

type CreateMatchModalFormProps = {
  open: boolean;
  onClose: () => void;
  afterCreateEffect: () => void;
};

type CreateMatchFormData = {
  description: string;
};

export function CreateMatchModalForm({
  open,
  onClose,
  afterCreateEffect,
}: CreateMatchModalFormProps) {
  const [showError, setShowError] = useState(false);
  const [isCreatingMatch, setIsCreatingMatch] = useState(false);

  const { register, handleSubmit, formState } = useForm<CreateMatchFormData>({
    defaultValues: {
      description: "",
    },
  });
  const { errors } = formState;

  const createMatch = ({ description }: CreateMatchFormData) => {
    setIsCreatingMatch(true);
    startMatch(description)
      .then(afterCreateEffect)
      .catch((err) => {
        console.error({ err });
        setShowError(true);
      })
      .finally(() => setIsCreatingMatch(false));
  };

  return (
    <>
      <Modal open={open} onClose={onClose}>
        <Box
          sx={{
            position: "absolute",
            top: "50%",
            left: "50%",
            transform: "translate(-50%, -50%)",
            width: 400,
            p: 4,
          }}
        >
          <Card
            sx={{
              width: 400,
              minHeight: 200,
              px: 4,
              py: 2,
              display: "flex",
              flexDirection: "column",
              gap: 4,
            }}
          >
            <Box sx={{ display: "flex", justifyContent: "center" }}>
              <Typography variant="h5"> Create a Match! </Typography>
            </Box>

            <form onSubmit={handleSubmit(createMatch)}>
              <Stack
                sx={{
                  display: "flex",
                  flexDirection: "column",
                  justifyContent: "space-between",
                  minHeight: 200,
                }}
              >
                <Stack spacing={2}>
                  <TextField
                    label="Description"
                    type="text"
                    {...register("description", {
                      required: "Description is Required",
                      maxLength: {
                        message:
                          "Description must have a maximum of 120 characters",
                        value: 120,
                      },
                    })}
                    error={!!errors.description}
                    helperText={errors.description?.message}
                  />
                </Stack>
                <Button
                  type="submit"
                  variant="contained"
                  loading={isCreatingMatch}
                  disabled={isCreatingMatch}
                >
                  Create
                </Button>
              </Stack>
            </form>
          </Card>
        </Box>
      </Modal>

      <Dialog open={showError} onClose={() => setShowError(false)}>
        <DialogTitle>Create a Match was not possible</DialogTitle>
        <DialogContent>
          We have a problem when trying to create a match for you.
        </DialogContent>
        <DialogActions>
          <Button> Okay! </Button>
        </DialogActions>
      </Dialog>
    </>
  );
}
