import { Stack, Typography } from "@mui/material";
import { useSpring, animated } from "@react-spring/web";
import { useState, useEffect, memo } from "react";
import { AppCard } from "../../components/app-card";
import { HighlightPaper } from "../../components/app-paper";
import { useSnackbar } from "../../components/snackbar";
import { useSignalRContext } from "../../contexts/signalr.context";
import { useDebounce } from "../../hooks/debounce";
import { Story } from "../../models/matches";
import { PartyParticipantsViewer } from "../../components/party-participants-viewer";
import { useMatch } from "../../stores/match-store";

interface CurrentlyShowingStoryViewerProps {
  stories: Story[];
}

export function CurrenlyShowingStoryViewer({
  stories,
}: CurrentlyShowingStoryViewerProps) {
  const { signalRClient, registerEndpointFor, disconnectFromEndpointFor } =
    useSignalRContext();
  const { showInfo } = useSnackbar();

  const { showStory, currentShowingStory } = useMatch();

  const handleSelectStory = (storyId: number) => {
    const storyToAnalyze = stories.find((s) => s.storyId == storyId) || null;

    showStory(storyToAnalyze);
    api.start({
      from: { transform: "translateX(-100%)" },
      to: { transform: "translateX(0%)" },
      reset: true,
      config: { tension: 200, friction: 20 },
    });

    if (storyToAnalyze !== null) {
      showInfo(`Someone selected story "${storyToAnalyze.name}" to be voted`);
    }
  };

  const { debouncedFn } = useDebounce<number, (storyId: number) => void>(
    handleSelectStory,
    400,
  );

  const [animationProps, api] = useSpring(
    {
      from: { transform: "translateX(-100%)" },
      to: { transform: "translateX(0%)" },
      config: { tension: 200, friction: 20 },
    },
    [currentShowingStory?.storyId],
  );

  useEffect(() => {
    disconnectFromEndpointFor(signalRClient, "SelectStoryToVoteAs").then(() => {
      registerEndpointFor(signalRClient, "SelectStoryToVoteAs", (storyId) =>
        debouncedFn(storyId as number),
      );
    });

    return () => {
      disconnectFromEndpointFor(signalRClient, "SelectStoryToVoteAs");
    };
  }, [stories]);

  useEffect(() => {
    api.start({
      from: { transform: "translateX(-100%)" },
      to: { transform: "translateX(0%)" },
      reset: true,
      config: { tension: 200, friction: 20 },
    });
  }, []);

  return (
    <Stack spacing={4}>
      <PartyParticipantsViewer />
      <animated.div style={animationProps}>
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
