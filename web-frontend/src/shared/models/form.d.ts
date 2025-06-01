import { Story } from "./matches";

export interface ModalHandlerState<TArg> {
  isOpen: boolean;
  open: (story: TArg | null = null) => void;
  close: () => void;
}
