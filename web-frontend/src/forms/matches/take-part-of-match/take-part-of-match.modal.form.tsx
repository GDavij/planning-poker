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
import { listMatchRoles } from "../../../services/match.service";
import { ListRolesQueryResponse } from "../../../models/matches";
import { useForm } from "react-hook-form";

type TakePartOfMatchModalFormProps = {
  open: boolean;
  onClose: () => void;
  afterSuccess: () => void;
  matchId: string;
};

type TakePartOfMatchModalFormData = {
  roleId: number;
  shouldSpectate: boolean;
};

export function TakePartOfMatchModalForm({
  open,
  onClose,
  afterSuccess,
  matchId,
}: TakePartOfMatchModalFormProps) {
  const [showError, setShowError] = useState(false);
  const [roles, setRoles] = useState<ListRolesQueryResponse[]>([]);

  const [abortController] = useState(new AbortController());

  const loadRoles = () => {
    listMatchRoles(abortController)
      .then((res) => setRoles(res.data))
      .then(() => setValue("roleId", roles[0].roleId))
      .catch((err) => {
        console.error({ err });
        setShowError(true);
      })
      .finally(() => {});
  };

  useEffect(() => {
    loadRoles();

    return () => {};
  }, [matchId]);

  const { register, handleSubmit, watch, setValue, formState } =
    useForm<TakePartOfMatchModalFormData>();

  const { errors } = formState;

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
          </Stack>

          <Button variant="contained"> Join </Button>
        </Card>
      </Box>
    </Modal>
  );
}
