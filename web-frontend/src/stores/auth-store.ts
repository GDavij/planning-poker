import { create } from "zustand";

interface AuthActions {
  accountId: number;
  setAccountIdAs: (accountId: number) => void;
}

export const useAuth = create<AuthActions>((set) => ({
  accountId: 0,
  setAccountIdAs: (accountId) => set({ accountId: accountId }),
}));
