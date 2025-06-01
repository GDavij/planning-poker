import { useState } from "react";
import { Story } from "../../../../models/matches";
import { api } from "../axios.service";
import { ApiResponse } from "../../../../models/base";

export function useSaveStory() {
  const [isSaving, setIsSaving] = useState(false);

  const saveStory = (story: Story) => {
    setIsSaving(true);

    if (story.storyId) {
      return api
        .put<
          ApiResponse<unknown>
        >(`/matches/match/${story.matchId}/story/${story.storyId}/update`, story)
        .finally(() => setIsSaving(false));
    }

    return api
      .post<void>(`/matches/match/${story.matchId}/story/add`, story)
      .finally(() => setIsSaving(false));
  };

  return { saveStory, isSaving };
}
