import { LoginUsingEmailAndPassword } from "./auth";

export function useEmailAndPasswordLogin() {
  function loginWithEmailAndPassword(login: LoginUsingEmailAndPassword) {
    return Promise.resolve();
  }

  return { loginWithEmailAndPassword };
}
