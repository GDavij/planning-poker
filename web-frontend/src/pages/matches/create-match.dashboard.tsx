import {
  Box,
  Button,
  Card,
  CircularProgress,
  Container,
  Grid2,
  MenuItem,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router";
import { useStartMatch } from "../../shared/hooks/integrations/api/matches/use-start-match-as.integration";
import { useListMatchRoles } from "../../shared/hooks/integrations/api/matches/use-list-match-roles.integration";

type CreateMatchFormData = {
  description: string;
  roleId: number;
  shouldSpectate: boolean;
};

export function CreateMatchPage() {
  const navigate = useNavigate();

  const [abortController] = useState(new AbortController());

  const { startMatch, isStarting } = useStartMatch();
  const { roles, isFetching } = useListMatchRoles(abortController);

  const { register, handleSubmit, setValue, watch, formState } =
    useForm<CreateMatchFormData>({
      defaultValues: {
        description: "",
        roleId: 0,
        shouldSpectate: true,
      },
    });

  const { errors } = formState;

  const createMatch = ({
    description,
    roleId,
    shouldSpectate,
  }: CreateMatchFormData) => {
    startMatch({ description, roleId, shouldSpectate }).then((match) =>
      navigate(`/dashboard/matches/join/${match!.matchId}`),
    );
  };

  return (
    <form id="create-match-form" onSubmit={handleSubmit(createMatch)}>
      <Container>
        <Card
          sx={{
            px: 4,
            py: 2,
            display: "flex",
            flexDirection: "column",
            gap: 6,
          }}
        >
          {isFetching ? (
            <CircularProgress />
          ) : (
            <>
              <Box sx={{ display: "flex", justifyContent: "space-between" }}>
                <Typography variant="h4">Create Match</Typography>

                <Stack direction="row" spacing={2}>
                  <Button
                    type="submit"
                    variant="contained"
                    loading={isStarting}
                  >
                    Create
                  </Button>
                </Stack>
              </Box>
              <Stack spacing={2}>
                <TextField
                  fullWidth
                  label="Description"
                  {...register("description", {
                    required: "Description of match is required",
                    maxLength: {
                      message:
                        "Description of match must have a max of 120 characters",
                      value: 120,
                    },
                  })}
                  error={!!errors.description}
                  helperText={errors.description?.message}
                />

                <Grid2
                  container
                  spacing={2}
                  direction={{
                    xl: "row",
                    lg: "row",
                    md: "row",
                    sm: "column",
                    xs: "column",
                  }}
                >
                  <Grid2 size={{ lg: 10, md: 10, xl: 10, xs: 12, sm: 12 }}>
                    <TextField
                      select
                      fullWidth
                      label="Role"
                      {...register("roleId", {
                        required: "Role is required",
                        min: {
                          message: "Role is required",
                          value: 1,
                        },
                      })}
                      value={watch("roleId")}
                      onChange={(ev) =>
                        setValue("roleId", Number(ev.target.value))
                      }
                      error={!!errors.roleId}
                      helperText={errors.roleId?.message}
                    >
                      {roles.map((r) => (
                        <MenuItem key={r.roleId} value={r.roleId}>
                          {r.name +
                            (r.abbreviation ? ` - ${r.abbreviation}` : "")}
                        </MenuItem>
                      ))}
                    </TextField>
                  </Grid2>
                  <Grid2 size={{ lg: 2, md: 2, xl: 2, xs: 12, sm: 12 }}>
                    <TextField
                      label="Gonna Observate"
                      {...register("shouldSpectate")}
                      select
                      fullWidth
                      value={watch("shouldSpectate")}
                      onChange={(ev) =>
                        setValue("shouldSpectate", Boolean(ev.target.value))
                      }
                      error={!!errors.shouldSpectate}
                      helperText={errors.shouldSpectate?.message}
                    >
                      {/* @ts-ignore*/}
                      <MenuItem value={false}>No</MenuItem>

                      {/* @ts-ignore*/}
                      <MenuItem value={true}>Yes</MenuItem>
                    </TextField>
                  </Grid2>
                </Grid2>
              </Stack>
              )
            </>
          )}
        </Card>
      </Container>
    </form>
  );
}
