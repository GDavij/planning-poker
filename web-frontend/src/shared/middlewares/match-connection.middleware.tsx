import { Outlet } from "react-router";
import { SignalRHubs } from "../consts/signalRHubs";
import { SignalRContextProvider } from "../contexts/signalr.context";

export function MatchGamePage() {
  return (
    <SignalRContextProvider hubName={SignalRHubs.MatchHub}>
      <SignalRErrorPopUpHandler>
        <Outlet />
      </SignalRErrorPopUpHandler>
    </SignalRContextProvider>
  );
}

type SignalRErrorPopUpHandler = { children: React.ReactNode };

function SignalRErrorPopUpHandler({ children }: SignalRErrorPopUpHandler) {
  return children;
}
