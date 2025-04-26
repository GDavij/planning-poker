import { useEffect, useRef, useState } from "react";

export function useListSettleDetector<T>(
  items: T[],
  timeout: number,
  onSettled: (stableItems: T[]) => void,
) {
  const [hasDetectedChanges, setHasDetectedChanges] = useState(false);

  // Store the timer reference for cleanup
  const timerRef = useRef<NodeJS.Timeout | null>(null);

  // Store the last items string representation to detect actual changes
  const lastItemsStringRef = useRef<string>("");

  const haveAppliedCallback = () => {
    setHasDetectedChanges(false);
    if (timerRef.current) {
      clearTimeout(timerRef.current);
    }
  };

  useEffect(() => {
    // Serialize items to compare content rather than reference
    const itemsString = JSON.stringify(items);

    if (lastItemsStringRef.current.length === 0) {
      lastItemsStringRef.current = itemsString;
      return;
    }

    // Only proceed if the content actually changed
    if (itemsString !== lastItemsStringRef.current) {
      setHasDetectedChanges(true);
      lastItemsStringRef.current = itemsString;

      // Clear any existing timer
      if (timerRef.current) {
        clearTimeout(timerRef.current);
      }

      // Set a new timer
      timerRef.current = setTimeout(() => {
        onSettled([...items]); // Pass a copy to prevent mutations
        setHasDetectedChanges(false);
      }, timeout);
    }

    // Cleanup on unmount or when dependencies change
    return () => {
      if (timerRef.current) {
        clearTimeout(timerRef.current);
        timerRef.current = null;
      }
    };
  }, [items, timeout, onSettled]);

  return { hasDetectedChanges, haveAppliedCallback };
}
