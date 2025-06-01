import { Stack, Typography } from "@mui/material";
import { useSpring, animated } from "@react-spring/web";
import { useEffect, memo, useCallback } from "react";
import { AppCard } from "../../shared/ui/app-card";
import { HighlightPaper } from "../../shared/ui/app-paper";
import { useSnackbar } from "../../shared/ui/snackbar";
import { useDebounce } from "../../shared/hooks/helpers/debounce";
import { Story } from "../../shared/models/matches";
import { PartyParticipantsViewer } from "../../shared/ui/party-participants-viewer";
import { useMatchStore } from "../../shared/stores/match-store";
import { useSignalRContext } from "../../shared/contexts/signalr.context";
import { SignalRHooks } from "../../shared/consts/signalRHooks";

export function CurrenlyShowingStoryViewer() {
  const { registerEndpointFor } = useSignalRContext();
  const { showInfo } = useSnackbar();
  const { showStory, currentShowingStory, stories } = useMatchStore();

  const [springAnimationProps, springAnimation] = useSpring(
    {
      from: { transform: "translateX(-100%)" },
      to: { transform: "translateX(0%)" },
      config: { tension: 200, friction: 20 },
    },
    [stories],
  );

  const AnimateMovingStoryWithId = useCallback(
    (storyId: number) => {
      const storyToAnalyze = stories.find((s) => s.storyId == storyId) || null;

      showStory(storyToAnalyze);
      springAnimation.start({
        from: { transform: "translateX(-100%)" },
        to: { transform: "translateX(0%)" },
        reset: true,
        config: { tension: 200, friction: 20 },
      });

      if (storyToAnalyze !== null) {
        showInfo(`Someone selected story "${storyToAnalyze.name}" to be voted`);
      }
    },
    [stories],
  );

  const { debouncedFn: tryAnimateMovingStoryWithId } = useDebounce<
    number,
    (storyId: number) => void
  >(AnimateMovingStoryWithId, 400);

  useEffect(() => {
    registerEndpointFor(SignalRHooks.OnSelectedStoryToVote, (storyId) =>
      tryAnimateMovingStoryWithId(storyId as number),
    );
  }, [stories]);

  useEffect(() => {
    showStory(null);

    springAnimation.start({
      from: { transform: "translateX(-100%)" },
      to: { transform: "translateX(0%)" },
      reset: true,
      config: { tension: 200, friction: 20 },
    });
  }, []);

  return (
    <Stack spacing={4}>
      <PartyParticipantsViewer />
      {/* @ts-ignore */}
      <animated.div
        style={{
          ...springAnimationProps,
        }}
      >
        <AppCard
          sx={{
            paddingX: 4,
            paddingY: 2,
            padding: 3,
          }}
        >
          {currentShowingStory !== null ? (
            <AnimatedStoryCard story={currentShowingStory} />
          ) : (
            <Stack>
              <Typography> No Story Selected To Analyze yet...</Typography>
            </Stack>
          )}
        </AppCard>
      </animated.div>
    </Stack>
  );
}

interface AnimatedStoryCardProps {
  story: Story;
}

const AnimatedStoryCard = memo(({ story }: AnimatedStoryCardProps) => {
  return (
    <Stack spacing={3}>
      <Typography variant="h5" sx={{ fontWeight: "bold" }}>
        Story Details
      </Typography>
      <HighlightPaper sx={{ padding: 2 }}>
        <Stack direction="row" justifyContent="space-between">
          <Stack>
            <Typography variant="subtitle2" color="textSecondary">
              Name
            </Typography>
            <Typography variant="h6" sx={{ fontWeight: "bold" }}>
              {story?.name || "N/A"}
            </Typography>
          </Stack>
          <Stack>
            <Typography variant="subtitle2" color="textSecondary">
              Story Number
            </Typography>
            <Typography variant="h6" sx={{ fontWeight: "bold" }}>
              {story?.storyNumber || "N/A"}
            </Typography>
          </Stack>
        </Stack>
      </HighlightPaper>
    </Stack>
  );
});
