import { useEffect } from "react";
import { useParams } from "react-router";
import { useSignalR } from "../../hooks/use-signalr";
import { SignalRHubs } from "../../consts/signalr/signalRHubs";

export function MatchGamePage() {
  const { matchId } = useParams();

  const { invokeAsyncFor, registerEndpointFor, signalRClient } = useSignalR(
    SignalRHubs.MatchHub,
  );

  useEffect(() => {}, [matchId]);
  return <></>;
}
