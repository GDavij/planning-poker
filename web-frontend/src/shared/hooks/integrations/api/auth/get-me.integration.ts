import { useEffect } from "react";
import { Me } from "./auth";
import { ApiResponse } from "../../../../models/base";
import { api } from "../axios.service";
import { useAuthStore } from "../../../../stores/auth-store";
import { integrationError } from "../../../../errors/integration-error";
import { useNavigate } from "react-router";
import { useSnackbar } from "../../../../ui/snackbar";

export function useGetMe() {
  const { me, setMe, isFetchingMe, setIsFetchingMe } = useAuthStore();

  const navigate = useNavigate();
  const { showError } = useSnackbar();

  const getMeAsync = async () => {
    if (me === null) {
      try {
        setIsFetchingMe(true);
        const response = await api.get<ApiResponse<Me>>("auth/me");

        if (response.status == 401) {
          showError(
            "User was not authorized... please login again into the platform",
          );
          navigate("/sign-in");
          return;
        }

        if (!response.data.success) {
          throw new integrationError(response.data);
        }

        setMe(response.data.data!);
      } finally {
        setIsFetchingMe(false);
      }
    }
  };

  useEffect(() => {
    getMeAsync();
  }, []);

  return { me, isFetchingMe };
}
