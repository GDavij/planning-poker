import { Me } from "../models/auth";
import { api } from "./axios.service";

export function saveSession(token: string) {
  return api.post<
    {
      oAuthToken: string;
    },
    void
  >("/auth/save-session", {
    oAuthToken: token,
  });
}

export async function autoLogin() {
  return api.post("auth/autologin");
}

export async function getCurrentAccount() {
  return api.get<Me>("auth/me").then((r) => r.data);
}
