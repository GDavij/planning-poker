import { createContext, useContext } from "react";
import { useSignalR } from "../hooks/helpers/use-signalr";

export type ClientActionDeclarationFunc = (
  endpointName: string,
  handler: (...args: unknown[]) => Promise<void> | void,
) => Promise<void>;

export type InvokeServerActionFunc = (
  methodName: string,
  ...args: unknown[]
) => Promise<unknown>;

export type DisconectFromEndpointFunc = (methodName: string) => Promise<void>;

export type SignalRContext = {
  registerEndpointFor: ClientActionDeclarationFunc;
  invokeAsyncFor: InvokeServerActionFunc;
  disconnectFromEndpointFor: DisconectFromEndpointFunc;
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
  const { registerEndpointFor, invokeAsyncFor, disconnectFromEndpointFor } =
    useSignalR(hubName);

  return (
    <signalRContext.Provider
      value={{
        invokeAsyncFor,
        registerEndpointFor,
        disconnectFromEndpointFor,
      }}
    >
      {children}
    </signalRContext.Provider>
  );
}

export const useSignalRContext = () => useContext(signalRContext);
