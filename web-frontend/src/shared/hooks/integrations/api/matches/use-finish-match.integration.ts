import { useState } from "react";
import { api } from "../axios.service";

export function useFinishMatch() {
  const [isFinishing, setIsFinishing] = useState(false);

  function finish(matchId: number) {
    setIsFinishing(true);

    return api
      .patch<void>(`matches/match/${matchId}/finish`)
      .finally(() => setIsFinishing(false));
  }

  return { finish, isFinishing };
}
