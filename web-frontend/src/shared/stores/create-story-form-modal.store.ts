import { create } from "zustand";
import { ModalHandlerState } from "../models/form";
import { Story } from "../models/matches";

export interface EditStoryModalStateHandler extends ModalHandlerState<Story> {
  story: Story | null;
}

export const useCreateStoryFormModalStore =
  create<EditStoryModalStateHandler>()((set) => ({
    open: (story: Story | null = null) => set({ isOpen: true, story }),
    close: () => set({ isOpen: false }),
    isOpen: false,
    story: null,
  }));
