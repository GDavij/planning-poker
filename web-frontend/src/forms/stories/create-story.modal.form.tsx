import { create } from "zustand";
import {
  EditStoryModalStateHandler,
  ModalHandlerState,
} from "../../models/form";
import { Box, Button, Card, Modal, Stack, TextField } from "@mui/material";
import { useForm } from "react-hook-form";
import { useEffect, useState } from "react";
import { createStory, updateStory } from "../../services/match.service";
import { Story } from "../../models/matches";
import { useSnackbar } from "../../components/snackbar";
import { useParams } from "react-router";

export const useCreateStoryFormModal = create<EditStoryModalStateHandler>()(
  (set) => ({
    open: (story: Story | null = null) => set({ isOpen: true, story }),
    close: () => set({ isOpen: false }),
    isOpen: false,
    story: null,
  }),
);

interface CreateStoryForm {
  name: string;
  storyNumber: string;
}

export function CreateStoryFormModal() {
  const { isOpen, close, story } = useCreateStoryFormModal();
  const { showSuccess, showError } = useSnackbar();

  const matchId = Number(useParams()?.matchId);

  const [isSavingStory, setIsSavingStory] = useState(false);

  const {
    register,
    formState: { errors },
    handleSubmit,
    reset,
  } = useForm<CreateStoryForm>();

  useEffect(() => {
    reset(story as CreateStoryForm);
    setIsSavingStory(false);
  }, [isOpen]);

  const save = (form: CreateStoryForm) => {
    setIsSavingStory(true);
    if (story !== null) {
      let storyToUpdate: Story = {
        ...story,
        name: form.name,
        storyNumber: form.storyNumber,
      };

      updateStory(storyToUpdate)
        .then(() => showSuccess("Story updated with Success!"))
        .catch(() => showError("Failed to update Story"))
        .finally(close);
    } else {
      const storyToAdd: Partial<Story> = {
        matchId,
        name: form.name,
        storyNumber: form.storyNumber,
      };

      createStory(storyToAdd as Story)
        .then(() => showSuccess("Story added with Success!"))
        .catch(() => showError("Failed to add new Story"))
        .finally(close);
    }
  };

  return (
    <Modal open={isOpen} onClose={close}>
      <Box
        sx={{
          width: 800,
          height: 300,
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
                disabled={isSavingStory}
              >
                Close
              </Button>
            </Stack>
            <form onSubmit={handleSubmit(save)}>
              <Stack spacing={2}>
                <TextField
                  label="Name"
                  {...register("name", {
                    required: {
                      value: true,
                      message: "Name of Story is Required",
                    },
                    maxLength: {
                      value: 120,
                      message: "Name of Story must not be greater than 120",
                    },
                  })}
                  error={!!errors.name}
                  helperText={errors.name?.message}
                />

                <TextField
                  label="Story Number"
                  {...register("storyNumber", {
                    maxLength: {
                      value: 20,
                      message: "Story number must have a max of 20 characters",
                    },
                  })}
                  error={!!errors.storyNumber}
                  helperText={errors.storyNumber?.message}
                />

                <Button
                  variant="contained"
                  type="submit"
                  loading={isSavingStory}
                >
                  {" "}
                  Save{" "}
                </Button>
              </Stack>
            </form>
          </Stack>
        </Card>
      </Box>
    </Modal>
  );
}
