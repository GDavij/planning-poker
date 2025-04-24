import { HubConnection } from "@microsoft/signalr";
import { createContext, useContext } from "react";
import { useSignalR } from "../hooks/use-signalr";

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

export type SignalRContext = {
  registerEndpointFor: ClientActionDeclarationFunc;
  invokeAsyncFor: InvokeServerActionFunc;
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
  const { signalRClient, registerEndpointFor, invokeAsyncFor } =
    useSignalR(hubName);

  return (
    <signalRContext.Provider
      value={{ invokeAsyncFor, registerEndpointFor, signalRClient }}
    >
      {children}
    </signalRContext.Provider>
  );
}

export const useSignalRContext = () => useContext(signalRContext);
