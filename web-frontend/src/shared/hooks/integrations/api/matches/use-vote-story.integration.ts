import { useState } from "react";
import { Story } from "../../../../models/matches";
import { api } from "../axios.service";

export function useVoteStory() {
  const [isVoting, setIsVoting] = useState(false);

  function vote(story: Story, points: number) {
    setIsVoting(true);

    return api
      .patch<void>(
        `/matches/match/${story.matchId}/story/${story.storyId}/vote/${points}`,
      )
      .finally(() => setIsVoting(false));
  }

  return { vote, isVoting };
}
