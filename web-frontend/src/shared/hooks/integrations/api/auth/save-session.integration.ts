import { useState } from "react";
import { api } from "../axios.service";
import { ApiResponse } from "../../../../models/base";
import { integrationError } from "../../../../errors/integration-error";
import { AxiosResponse } from "axios";

export function UseSaveSession() {
  const [isSaving, setIsSaving] = useState(false);

  const saveSession = async (token: string) => {
    setIsSaving(true);

    const apiResponse = await api
      .post<
        {
          oAuthToken: string;
        },
        AxiosResponse<ApiResponse<unknown>>
      >("/auth/save-session", {
        oAuthToken: token,
      })
      .then((r) => r.data);

    if (!apiResponse.success) {
      throw new integrationError(apiResponse);
    }
  };

  return { saveSession, isSaving };
}
