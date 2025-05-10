import { HubConnection } from "@microsoft/signalr";
import { createContext, useContext } from "react";
import { useSignalR } from "../hooks/helpers/use-signalr";

export type ClientActionDeclarationFunc = (
  signalRClient: HubConnection,
  endpointName: string,
  handler: (...args: unknown[]) => Promise<void> | void,
) => Promise<void>;

export type InvokeServerActionFunc = (
  signalRClient: HubConnection,
  methodName: string,
  ...args: unknown[]
) => Promise<unknown>;

export type DisconectFromEndpointFunc = (
  signalRClient: HubConnection,
  methodName: string,
) => Promise<void>;

export type SignalRContext = {
  registerEndpointFor: ClientActionDeclarationFunc;
  invokeAsyncFor: InvokeServerActionFunc;
  disconnectFromEndpointFor: DisconectFromEndpointFunc;
  signalRClient: HubConnection;
};

export const signalRContext = createContext<SignalRContext>(
  {} as SignalRContext,
);

type SignalRContextProps = {
  hubName: string;
  children: React.ReactNode;
};

export function SignalRContextProvider({
  hubName,
  children,
}: SignalRContextProps) {
  const {
    signalRClient,
    registerEndpointFor,
    invokeAsyncFor,
    disconnectFromEndpointFor,
  } = useSignalR(hubName);

  return (
    <signalRContext.Provider
      value={{
        invokeAsyncFor,
        registerEndpointFor,
        disconnectFromEndpointFor,
        signalRClient,
      }}
    >
      {children}
    </signalRContext.Provider>
  );
}

export const useSignalRContext = () => useContext(signalRContext);
