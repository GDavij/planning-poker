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
