export interface ModalHandlerState {
  isOpen: boolean;
  story: Story | null;
  open: (story: Story | null = null) => void;
  close: () => void;
}
