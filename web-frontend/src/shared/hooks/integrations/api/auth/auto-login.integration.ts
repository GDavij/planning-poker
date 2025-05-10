import { integrationError } from "../../../../errors/integration-error";
import { ApiResponse } from "../../../../models/base";
import { api } from "../axios.service";

export function useAutoLogin() {
  const registerAutoLogin = async () => {
    const apiResponse = await api
      .post<ApiResponse<unknown>>("auth/autologin")
      .then((t) => t.data);

    if (!apiResponse.success) {
      throw new integrationError(apiResponse);
    }
  };

  return { registerAutoLogin };
}
