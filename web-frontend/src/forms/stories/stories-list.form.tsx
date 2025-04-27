import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useSignalRContext } from "../../contexts/signalr.context";
import { Story } from "../../models/matches";
import {
  deleteStory,
  finishMatch,
  listMatchStories,
  selectStoryToAnalyze,
  updateStory,
} from "../../services/match.service";
import { useNavigate, useParams } from "react-router";
import {
  Button,
  CircularProgress,
  Paper,
  Stack,
  Tooltip,
  Typography,
} from "@mui/material";
import { styled } from "@mui/material/styles";
import { DndProvider, useDrag, useDrop } from "react-dnd";
import { HTML5Backend } from "react-dnd-html5-backend";
import { animated, useSpring, useTransition } from "@react-spring/web";
import { useCreateStoryFormModal } from "./create-story.modal.form";
import { useListSettleDetector } from "../../hooks/use-list-settle-detector";
import { useSnackbar } from "../../components/snackbar";
import { Delete, Edit, Visibility } from "@mui/icons-material";
import { useConfirmation } from "../../components/confirmation-dialog";
import { AppCard } from "../../components/app-card";

const StoriesContainer = styled(Stack)(({ theme }) => ({
  borderRight: "2px solid #eef",
  flex: 1,
  overflow: "auto",
  margin: 4,
}));

const StoryItem = styled(Stack)(({ theme }) => ({
  minHeight: 100,
  background: "#fefeff",
  border: "1px solid #fff",
  borderRadius: 6,
  padding: "6px 12px",
}));

const AnimatedStoryItem = animated(StoryItem);

const StoryActions = styled(Stack)(({ theme }) => ({
  minHeight: 120,
  background: "#ddf",
  borderTop: "2px solid #fff",
  padding: 6,
}));

interface StoryCardProps {
  story: Story;
  index: number;
  moveStory: (dragIndex: number, hoverIndex: number) => void;
}

interface DragItem {
  index: number;
  id: string;
  type: string;
}

export function StoryCard({ story, index, moveStory }: StoryCardProps) {
  const ref = useRef<HTMLDivElement>(null);

  const { open } = useCreateStoryFormModal();
  const { showSuccess, showError } = useSnackbar();

  const [{ handlerId }, drop] = useDrop({
    accept: "story",
    collect(monitor) {
      return {
        handlerId: monitor.getHandlerId(),
      };
    },
    hover(item: DragItem, monitor) {
      if (!ref.current) {
        return;
      }
      const dragIndex = item.index;
      const hoverIndex = index;

      // Don't replace items with themselves
      if (dragIndex === hoverIndex) {
        return;
      }

      // Determine rectangle on screen
      const hoverBoundingRect = ref.current?.getBoundingClientRect();

      // Get vertical middle
      const hoverMiddleY =
        (hoverBoundingRect.bottom - hoverBoundingRect.top) / 2;

      // Determine mouse position
      const clientOffset = monitor.getClientOffset();

      // Get pixels to the top
      const hoverClientY = clientOffset!.y - hoverBoundingRect.top;

      // Only perform the move when the mouse has crossed half of the items height
      // When dragging downwards, only move when the cursor is below 50%
      // When dragging upwards, only move when the cursor is above 50%

      // Dragging downwards
      if (dragIndex < hoverIndex && hoverClientY < hoverMiddleY) {
        return;
      }

      // Dragging upwards
      if (dragIndex > hoverIndex && hoverClientY > hoverMiddleY) {
        return;
      }

      // Time to actually perform the action
      moveStory(dragIndex, hoverIndex);

      // Note: we're mutating the monitor item here!
      // Generally it's better to avoid mutations,
      // but it's good here for the sake of performance
      // to avoid expensive index searches.
      item.index = hoverIndex;
    },
  });

  const [{ isDragging }, drag] = useDrag({
    type: "story",
    item: () => {
      return { id: story.storyId, index };
    },
    collect: (monitor) => ({
      isDragging: monitor.isDragging(),
    }),
  });

  const springProps = useSpring({
    scale: isDragging ? 1.05 : 1,
    boxShadow: isDragging
      ? "0 10px 20px rgba(0,0,0,0.19), 0 6px 6px rgba(0,0,0,0.23)"
      : "0 1px 3px rgba(0,0,0,0.12), 0 1px 2px rgba(0,0,0,0.24)",
    opacity: isDragging ? 0.8 : 1,
    immediate: (key) => key === "opacity" && !isDragging,
    config: { mass: 1, tension: 500, friction: 40 },
  });

  const opacity = isDragging ? 0.4 : 1;
  drag(drop(ref));

  const handleEdit = useCallback(
    (story: Story) => {
      open(story);
    },
    [story],
  );

  const { confirm } = useConfirmation();

  const handleSelect = useCallback(
    (story: Story) => {
      selectStoryToAnalyze(story).catch(() => {
        showError("Could not select story for analysis for now...");
      });
    },
    [story],
  );

  const handleDelete = useCallback(
    (story: Story) => {
      confirm({
        title: "Delete Story",
        message: `Are you sure you want to delete "${story.name}"? This action cannot be undone.`,
        confirmText: "Delete",
        cancelText: "Cancel",
        severity: "error",
      }).then((shouldDelete) => {
        if (shouldDelete) {
          deleteStory(story)
            .then(() => showSuccess("Story deleted with Success"))
            .catch(() => showError("Could not delete Story with Success"));
        }
      });
    },
    [story],
  );

  return (
    <AnimatedStoryItem ref={ref} data-handler-id={handlerId} spacing={2}>
      <Stack
        direction={"row"}
        spacing={2}
        justifyContent={"space-between"}
        flexWrap="wrap"
      >
        <Stack
          width="100%"
          direction={"row"}
          justifyContent={"space-between"}
          flexWrap="wrap"
          gap={1}
        >
          <Paper
            sx={{
              paddingX: 1,
              paddingY: 0.5,
              background: "#ddf",
              maxWidth: "70%",
              minWidth: "150px",
              display: "flex",
              alignItems: "center",
            }}
          >
            <Typography
              fontWeight={700}
              color="#333"
              sx={{
                wordBreak: "break-word",
                whiteSpace: "normal",
                overflowWrap: "break-word",
              }}
            >
              {story.name}
            </Typography>
          </Paper>

          <Typography sx={{ flexShrink: 0 }}>
            {story.storyNumber || "None"}
          </Typography>
        </Stack>
      </Stack>
      <Stack direction={"row"} spacing={1}>
        <Stack
          justifyContent={"space-between"}
          width={"100%"}
          direction={"row"}
        >
          <Button
            variant="text"
            color="error"
            onClick={() => handleDelete(story)}
          >
            <Delete />
          </Button>

          <Stack direction={"row"} spacing={1}>
            <Button
              variant="text"
              color="warning"
              onClick={() => handleEdit(story)}
            >
              <Edit />
            </Button>
            <Button
              variant="text"
              color="primary"
              onClick={() => handleSelect(story)}
            >
              <Visibility />
            </Button>
          </Stack>
        </Stack>
      </Stack>
    </AnimatedStoryItem>
  );
}

export function StoriesListForm() {
  const { registerEndpointFor, disconnectFromEndpointFor, signalRClient } =
    useSignalRContext();

  const navigate = useNavigate();

  const matchId = Number(useParams()?.matchId);
  const syncFetchStories = useMemo(() => listMatchStories(matchId), [matchId]);
  const { open } = useCreateStoryFormModal();
  const { showSuccess, showError } = useSnackbar();

  useEffect(() => {
    syncFetchStories.then((stories) => setStories(stories));
  }, [syncFetchStories]);

  useEffect(() => {
    registerEndpointFor(
      signalRClient,
      "UpdateStoriesOfMatchWith",
      (serverStories) => setStories(serverStories as Story[]),
    );

    registerEndpointFor(signalRClient, "MatchClosed", () => {
      navigate("/dashboard");
    });

    return () => {
      disconnectFromEndpointFor(signalRClient, "UpdateStoriesOfMatchWith");
      disconnectFromEndpointFor(signalRClient, "MatchClosed");
    };
  });

  const [stories, setStories] = useState<Story[]>([]);

  // Use a ref to measure actual story heights
  const storyRefs = useRef<Record<string, HTMLDivElement | null>>({});
  const [storyHeights, setStoryHeights] = useState<Record<string, number>>({});

  // Calculate positions based on actual measured heights
  const getStoryPosition = useCallback(
    (index: number) => {
      let position = 0;
      for (let i = 0; i < index; i++) {
        const storyId = stories[i]?.storyId;
        position += storyHeights[storyId] || 116; // Default height + spacing
      }
      return position;
    },
    [stories, storyHeights],
  );

  // Update transitions to use dynamic heights
  const transitions = useTransition(
    stories.map((story, index) => ({
      ...story,
      key: story.storyId,
      y: getStoryPosition(index),
    })),
    {
      from: { opacity: 0, y: -20 },
      enter: ({ y }) => ({ opacity: 1, y }),
      update: ({ y }) => ({ y }),
      leave: { opacity: 0, height: 0 },
      keys: (item) => item.storyId,
      config: { mass: 1, tension: 280, friction: 30 },
    },
  );

  // Measure story heights after render
  useEffect(() => {
    const newHeights: Record<string, number> = {};
    stories.forEach((story) => {
      const element = storyRefs.current[story.storyId];
      if (element) {
        newHeights[story.storyId] = element.offsetHeight + 16; // height + spacing
      }
    });
    setStoryHeights(newHeights);
  }, [stories]);

  const moveStory = useCallback((dragIndex: number, hoverIndex: number) => {
    setStories((prevStories) => {
      const newStories = [...prevStories];
      // Remove the dragged item
      const draggedItem = newStories.splice(dragIndex, 1)[0];
      // Insert it at the new position
      newStories.splice(hoverIndex, 0, draggedItem);

      // Update order property for each story
      return newStories.map((story, index) => ({
        ...story,
        order: index + 1,
      }));
    });
  }, []);

  const saveStoriesOrder = useCallback(
    async (stories: Story[]) => {
      try {
        for (const story of stories) {
          await updateStory(story);
        }

        return Promise.resolve().then(() => {
          showSuccess("Update Stories with Success");
          haveAppliedCallback();
        });
      } catch {
        return Promise.reject(() => {
          showError("An Error occurred while Updating Stories");
        });
      }
    },
    [stories, matchId],
  );

  const { hasDetectedChanges, haveAppliedCallback } = useListSettleDetector(
    stories,
    1000,
    (stories) => {
      saveStoriesOrder(stories);
    },
  );

  const forceUpdateStories = () => {
    saveStoriesOrder(stories);
  };

  // Calculate total height of all stories for container sizing
  const totalListHeight = useMemo(() => {
    let height = 0;
    stories.forEach((story) => {
      height += storyHeights[story.storyId] || 116; // Default height + spacing
    });
    return height || 100; // Minimum height
  }, [stories, storyHeights]);

  const closeMatch = () => {
    finishMatch(matchId)
      .then(() => showSuccess("Match is being finished"))
      .catch(() => showError("Match has been Finished"));
  };

  return (
    <>
      <AppCard
        sx={{
          borderTopRightRadius: 12,
          borderBottomRightRadius: 12,
          display: "flex",
          flexDirection: "column",
          height: "100%",
        }}
      >
        <StoriesContainer
          spacing={2}
          sx={{
            flex: 1,
            display: "flex",
            flexDirection: "column",
          }}
        >
          <Tooltip title="Click to update now">
            <Button
              variant="outlined"
              disabled={!hasDetectedChanges}
              onClick={forceUpdateStories}
              fullWidth
              sx={{ alignSelf: "flex-start" }}
            >
              <Stack direction="row" alignItems={"center"} spacing={1}>
                <Typography>
                  {hasDetectedChanges
                    ? "Waiting to persist stories changes"
                    : "Stories"}
                </Typography>
                {hasDetectedChanges && <CircularProgress size={16} />}
              </Stack>
            </Button>
          </Tooltip>

          <Stack
            spacing={2}
            sx={{
              overflow: "auto", // Changed from "scroll" to "auto"
              height: {
                xs: "calc(100vh - 250px)", // More space on small screens
                sm: "calc(100vh - 220px)",
                md: "calc(100vh - 200px)",
              },
              flexGrow: 1,
              position: "relative",
              width: "100%",
            }}
          >
            <DndProvider backend={HTML5Backend}>
              <div
                style={{
                  position: "relative",
                  width: "100%",
                  height: totalListHeight,
                  minHeight: "100%",
                }}
              >
                {transitions((style, story) => (
                  <animated.div
                    style={{
                      ...style,
                      position: "absolute",
                      left: 0,
                      right: 0,
                      width: "100%",
                    }}
                    ref={(el) => {
                      // Store ref to measure height
                      storyRefs.current[story.storyId] = el;
                    }}
                  >
                    <StoryCard
                      key={story.storyId}
                      story={story}
                      index={story.order - 1}
                      moveStory={moveStory}
                    />
                  </animated.div>
                ))}
              </div>
            </DndProvider>
          </Stack>
        </StoriesContainer>

        <StoryActions sx={{ flexShrink: 0 }}>
          <Button
            variant="contained"
            onClick={() => open(null)}
            sx={{
              width: { xs: "100%", sm: "auto" },
            }}
          >
            Create Story
          </Button>

          <Button
            variant="outlined"
            color="error"
            sx={{
              width: { xs: "100%", sm: "auto" },
            }}
            onClick={closeMatch}
          >
            End Match
          </Button>
        </StoryActions>
      </AppCard>
    </>
  );
}
