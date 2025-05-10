import { create } from "zustand";
import { Story } from "../models/matches";

interface MatchActions {
  currentShowingStory: Story | null;
  stories: Story[];
  setStories: (stories: Story[]) => void;
  setStoriesFunc: (stories: Story[], cb: (stories: Story[]) => Story[]) => void;
  showStory: (story: Story | null) => void;
}

export const useMatch = create<MatchActions>((set) => ({
  currentShowingStory: null,
  stories: [],
  setStories: (stories: Story[]) => set({ stories }),
  setStoriesFunc: (stories, cb) => set({ stories: cb(stories) }),
  showStory: (story: Story | null) => {
    set({ currentShowingStory: story });
  },
}));
