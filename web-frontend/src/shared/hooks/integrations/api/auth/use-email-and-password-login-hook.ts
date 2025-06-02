import { useState } from "react";
import { LoginUsingEmailAndPassword } from "./auth";

export function useEmailAndPasswordLogin() {
  const [isLogin, setIsLogin] = useState(false);
  function loginWithEmailAndPassword(login: LoginUsingEmailAndPassword) {
    return Promise.resolve();
  }

  return { loginWithEmailAndPassword, isLogin };
}
