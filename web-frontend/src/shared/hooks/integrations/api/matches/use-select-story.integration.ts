import { useState } from "react";
import { Story } from "../../../../models/matches";
import { api } from "../axios.service";

export function useSelectStory() {
  const [isSelecting, setIsSelecting] = useState(false);

  function select(story: Story) {
    setIsSelecting(true);

    return api
      .patch(`/matches/match/${story.matchId}/story/${story.storyId}`)
      .finally(() => setIsSelecting(false));
  }

  return { select, isSelecting };
}
