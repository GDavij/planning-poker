import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from "@microsoft/signalr";
import { useEffect, useState } from "react";
import { suspendWhileTrueAsyncFor as suspendAsyncWhile } from "../helpers/async.helper";

export function useSignalR(hubName: string) {
  const createClient = () =>
    new HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_SIGNALR_HUB_DOMAIN}/${hubName}`, {
        withCredentials: true,
      })
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect()
      .build();

  const  registerEndpointFor = async (
    signalRClient: HubConnection,
    endpointName: string,
    handler: (...args: unknown[]) => Promise<void> | void,
  ) => {
    const cannotInvokeHubSinceItIsNotConnected = () =>
      signalRClient.state !== HubConnectionState.Connected;

    // Block SignalR Hub call till handshake
    await suspendAsyncWhile(cannotInvokeHubSinceItIsNotConnected);

    console.log({endpointName, handler})
    signalRClient.on(endpointName,(args) => {
      console.log({args});
      handler(args);
    }
  );
  };

  const invokeAsyncFor = async (
    signalRClient: HubConnection,
    methodName: string,
    ...args: unknown[]
  ): Promise<unknown> => {
    const cannotInvokeHubSinceItIsNotConnected = () =>
      signalRClient.state !== HubConnectionState.Connected;

    // Block SignalR Hub call till handshake
    await suspendAsyncWhile(cannotInvokeHubSinceItIsNotConnected);

    if (args.length > 0) {
      return signalRClient
        .invoke(methodName, ...args)
        .then((res) => {
          console.log({ success: res });
        })
        .catch((err) => console.error({ err }));
    } else {
      return signalRClient
        .invoke(methodName)
        .then((res) => {
          console.log({ success: res });
        })
        .catch((err) => console.error({ err }));
    }
  };

  const [signalRClient] = useState<HubConnection>(createClient());

  useEffect(() => {
    if (signalRClient.state === HubConnectionState.Disconnected) {
      signalRClient.start();
    }
  }, [hubName]);

  return { signalRClient, registerEndpointFor, invokeAsyncFor };
}
