import { Box, Button, Card, Modal, Stack, TextField } from "@mui/material";
import { useForm } from "react-hook-form";
import { useEffect } from "react";
import { Story } from "../../shared/models/matches";
import { useSnackbar } from "../../shared/ui/snackbar";
import { useParams } from "react-router";
import { useSaveStory } from "../../shared/hooks/integrations/api/matches/use-save-story.integration";
import { useCreateStoryFormModalStore } from "../../shared/stores/create-story-form-modal.store";

interface CreateStoryForm {
  name: string;
  storyNumber: string;
}

export function CreateStoryFormModal() {
  const matchId = Number(useParams()?.matchId);

  const { isOpen, close, story } = useCreateStoryFormModalStore();
  const { showSuccess, showError } = useSnackbar();
  const { saveStory, isSaving } = useSaveStory();

  const {
    register,
    formState: { errors },
    handleSubmit,
    reset,
  } = useForm<CreateStoryForm>();

  useEffect(() => {
    reset(story as CreateStoryForm);
  }, [isOpen]);

  const save = (form: CreateStoryForm) => {
    let storyToSave: Partial<Story>;

    if (story !== null) {
      storyToSave = {
        ...story,
        name: form.name,
        storyNumber: form.storyNumber,
      };
    } else {
      storyToSave = {
        matchId,
        name: form.name,
        storyNumber: form.storyNumber,
      };
    }

    saveStory(storyToSave as Story)
      .then(() => {
        showSuccess("Story has been saved with Success!");
        close();
      })
      .catch(() => showError("Failed to add new Story"));
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
              <Button variant="outlined" onClick={close} disabled={isSaving}>
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

                <Button variant="contained" type="submit" loading={isSaving}>
                  Save
                </Button>
              </Stack>
            </form>
          </Stack>
        </Card>
      </Box>
    </Modal>
  );
}
