import { create } from "zustand";
import { Me } from "../hooks/integrations/api/auth/auth";

interface AuthActions {
  me: Me | null;
  setMe: (me: Me) => void;
  isFetchingMe: boolean;
  setIsFetchingMe: (isFetching: boolean) => void;
}

export const useAuthStore = create<AuthActions>((set) => ({
  me: null,
  setMe: (me) => set({ me }),
  isFetchingMe: false,
  setIsFetchingMe: (isFetching: boolean) => set({ isFetchingMe: isFetching }),
}));
