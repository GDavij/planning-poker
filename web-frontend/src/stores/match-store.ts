import { create } from "zustand";
import { Story } from "../models/matches";

interface MatchActions {
  currentShowingStory: Story | null;
  showStory: (story: Story | null) => void;
}

export const useMatch = create<MatchActions>((set) => ({
  currentShowingStory: null,
  showStory: (story: Story) => set({ currentShowingStory: story }),
}));
