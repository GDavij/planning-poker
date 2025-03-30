import { useEffect, useState } from "react";
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from "@microsoft/signalr";

export function useSignalR() {
  const [signalrClient, setSignalrClient] = useState<HubConnection | null>(
    null,
  );

  const connectIfNot = () => {
    if (
      [
        HubConnectionState.Disconnected,
        HubConnectionState.Reconnecting,
      ].includes(signalrClient!.state)
    ) {
      return signalrClient?.start();
    }

    return Promise.resolve();
  };

  const createClient = () =>
    new HubConnectionBuilder()
      .withUrl(import.meta.env.VITE_API_DOMAIN, {
        withCredentials: true,
      })
      .configureLogging(LogLevel.Trace)
      .build();

  useEffect(() => {
    setSignalrClient(createClient());

    return () => {
      signalrClient
        ?.stop()
        .then(() => console.log("SignalR connection closed"));
    };
  }, []);

  return { signalrClient, connectIfNot };
}
