import { Stack, Step, StepLabel, Stepper, Typography } from "@mui/material";
import { useState } from "react";

type Step = {
  name: string;
  description: string;
  done: boolean;
};

const steps: Step[] = [
  {
    name: "User Info",
    description: "Who you are",
    done: false,
  },
  {
    name: "Password definition",
    description: "Secure your login",
    done: false,
  },
  {
    name: "Done",
    description: "You are ready to rock",
    done: false,
  },
];
export function NewAccountForm() {
  const [stepIndex, setStepIndex] = useState(0);

  const goToPrevious = () => setStepIndex((v) => (v <= 0 ? 0 : --v));
  const goToNext = () =>
    setStepIndex((v) => (v >= steps.length - 1 ? steps.length - 1 : ++v));

  const isOnStep = (indexToShow: number, mapStepIndex: number) => {
    return indexToShow == stepIndex && mapStepIndex == indexToShow;
  };

  return (
    <Stack>
      <Stack paddingX={2}>
        <Stepper activeStep={stepIndex} orientation="vertical">
          {steps.map((step, index) => (
            <Step key={step} completed={step.done}>
              <StepLabel
                optional={
                  <Typography variant="caption">{step.description}</Typography>
                }
              >
                {step.name}
              </StepLabel>
              {isOnStep(0, index) && <Typography> Hey</Typography>}

              {isOnStep(1, index) && <Typography> Hey</Typography>}

              {isOnStep(2, index) && <Typography> Hey</Typography>}
            </Step>
          ))}
        </Stepper>
      </Stack>
    </Stack>
  );
}
