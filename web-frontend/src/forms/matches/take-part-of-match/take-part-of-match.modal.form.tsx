import {
  Box,
  Button,
  Card,
  FormControl,
  FormHelperText,
  InputLabel,
  MenuItem,
  Modal,
  Select,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { useEffect, useState } from "react";
import {
  listMatchRoles,
  takePartOfMatchAs,
} from "../../../services/match.service";
import { ListRolesQueryResponse } from "../../../models/matches";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router";

type TakePartOfMatchModalFormProps = {
  open: boolean;
  onClose: () => void;
  afterSuccess: () => void;
  matchId: number;
  authCode: string | null;
};

type TakePartOfMatchModalFormData = {
  roleId: number;
  shouldSpectate: boolean;
};

const shouldObservateOptions = [
  {
    value: true,
    label: "Yes",
  },
  {
    value: false,
    label: "No",
  },
];

export function TakePartOfMatchModalForm({
  open,
  onClose,
  afterSuccess,
  matchId,
  authCode = null,
}: TakePartOfMatchModalFormProps) {
  const [roles, setRoles] = useState<ListRolesQueryResponse[]>([]);

  const [abortController] = useState(new AbortController());
  const navigate = useNavigate();

  const [isTakingPartOfMatch, setIsTakingPartOfMatch] = useState(false);

  const loadRoles = () => {
    setIsTakingPartOfMatch(true);

    listMatchRoles(abortController)
      .then((res) => setRoles(res.data))
      .then(() => setValue("roleId", roles[0].roleId))
      .catch((err) => {
        console.error({ err });
      })
      .finally(() => {
        setIsTakingPartOfMatch(false);
      });
  };

  useEffect(() => {
    if (roles.length == 0) {
      loadRoles();
    }

    reset();

    return () => {};
  }, [open]);

  const { register, handleSubmit, watch, setValue, reset, formState } =
    useForm<TakePartOfMatchModalFormData>();

  const { errors } = formState;

  const joinMatch = (joinData: TakePartOfMatchModalFormData) => {
    takePartOfMatchAs(
      joinData.roleId,
      Boolean(joinData.shouldSpectate),
      matchId,
      authCode,
    )
      .then((res) => {
        navigate(`/match/${matchId}`);
      })
      .catch((err) => {
        console.error({ err });
      });
  };

  return (
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
            <Typography variant="h5"> Join Match! </Typography>
          </Box>

          <form onSubmit={handleSubmit(joinMatch)}>
            <Stack spacing={2}>
              <TextField
                id="roleId"
                label="What's is your role?"
                error={!!errors.roleId}
                helperText={errors.roleId?.message}
                {...register("roleId", {
                  required: "Role is required",
                })}
                value={watch("roleId") || ""}
                onChange={(e) => setValue("roleId", Number(e.target.value))}
                select
              >
                {roles.map((r) => (
                  <MenuItem key={r.roleId} value={r.roleId}>
                    {r.name} {!!r.abbreviation ? r.abbreviation : ""}
                  </MenuItem>
                ))}
              </TextField>

              <TextField
                id="shouldObseravate"
                label="Gonna observate?"
                error={!!errors.shouldSpectate}
                helperText={errors.shouldSpectate?.message}
                {...register("shouldSpectate", {
                  required:
                    "You need to say if you are gonna join as observer (Can be changed)",
                })}
                value={watch("shouldSpectate") || false}
                onChange={(e) =>
                  setValue("shouldSpectate", Boolean(e.target.value))
                }
                select
              >
                {shouldObservateOptions.map((r) => (
                  <MenuItem key={r.value} value={r.value}>
                    {r.label}
                  </MenuItem>
                ))}
              </TextField>
            </Stack>

            <Button
              variant="contained"
              type="submit"
              disabled={isTakingPartOfMatch}
              loading={isTakingPartOfMatch}
            >
              {" "}
              Join{" "}
            </Button>
          </form>
        </Card>
      </Box>
    </Modal>
  );
}
