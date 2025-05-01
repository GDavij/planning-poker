import { Story } from "./matches";

export interface ModalHandlerState<TArg> {
  isOpen: boolean;
  open: (story: TArg | null = null) => void;
  close: () => void;
}

export interface EditStoryModalStateHandler extends ModalHandlerState<Story> {
  story: Story | null;
}
