import { useState } from "react";
import { Story } from "../../../../models/matches";
import { api } from "../axios.service";
import { ApiResponse } from "../../../../models/base";

export function useUpdateStory() {
  const [isUpdating, setIsUpdating] = useState(false);

  const updateStory = (story: Story) => {
    setIsUpdating(true);

    return api
      .put<
        ApiResponse<unknown>
      >(`/matches/match/${story.matchId}/story/${story.storyId}/update`, story)
      .finally(() => setIsUpdating(false));
  };

  return { updateStory, isUpdating };
}
