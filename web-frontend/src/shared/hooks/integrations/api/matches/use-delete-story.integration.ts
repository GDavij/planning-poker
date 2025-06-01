import { useState } from "react";
import { Story } from "../../../../models/matches";
import { api } from "../axios.service";

export function useDeleteStory() {
  const [isDeleting, setIsDeleting] = useState(false);

  function deleteStory(story: Story) {
    setIsDeleting(true);

    return api
      .delete<void>(`/matches/match/${story.matchId}/story/${story.storyId}`)
      .finally(() => setIsDeleting(false));
  }

  return { deleteStory, isDeleting };
}
