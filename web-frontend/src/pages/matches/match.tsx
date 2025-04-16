import { useContext, useEffect } from "react";
import { useSignalRContext } from "../../contexts/signalr.context";
import { useParams } from "react-router";
import {
  SignalRMatchHubClientEndpoints,
  SignalRMatchHubServerEndpoints,
} from "../../consts/signalr/signalr-match-hub.endpoints";

export function MatchPage() {
  const { matchId } = useParams();

  useEffect(() => {}, []);

  return <h1>Hello World</h1>;
}
