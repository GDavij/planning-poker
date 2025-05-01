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
      .withAutomaticReconnect([1, 2, 3, 4, 5])
      .build();

  const registerEndpointFor = async (
    signalRClient: HubConnection,
    endpointName: string,
    handler: (...args: unknown[]) => Promise<void> | void,
  ) => {
    signalRClient.on(endpointName, handler);
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
        .catch((err) => console.error({ err }));
    } else {
      return signalRClient
        .invoke(methodName)
        .catch((err) => console.error({ err }));
    }
  };

  const disconnectFromEndpointFor = async (
    signalRClient: HubConnection,
    endpointName: string,
  ) => {
    const cannotInvokeHubSinceItIsNotConnected = () =>
      signalRClient.state !== HubConnectionState.Connected;

    await suspendAsyncWhile(cannotInvokeHubSinceItIsNotConnected);

    signalRClient.off(endpointName);
  };

  const [signalRClient] = useState<HubConnection>(createClient());

  useEffect(() => {
    if (signalRClient.state === HubConnectionState.Disconnected) {
      signalRClient.start();
    }
  }, [hubName]);

  return {
    signalRClient,
    registerEndpointFor,
    invokeAsyncFor,
    disconnectFromEndpointFor,
  };
}
