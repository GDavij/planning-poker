import { useRef, useEffect, useCallback, useState } from "react";

/**
 * Options for the debounce hook
 */
interface DebounceOptions {
  /**
   * If true, the function will be called immediately on the first call,
   * then wait for the specified delay before it can be called again
   */
  leading?: boolean;
  /**
   * If true, guarantees that the function will be called at least once
   * after the specified delay (trailing call)
   */
  trailing?: boolean;
  /**
   * Maximum time to wait before the function is forced to be called
   * even if continuous calls are made
   */
  maxWait?: number;
}

export function useDebounce<TArg, T extends (arg: TArg) => any>(
  fn: T,
  delay: number,
  options: DebounceOptions = { leading: false, trailing: true },
) {
  const { leading = false, trailing = true, maxWait } = options;

  // Refs for tracking state between renders
  const timeoutRef = useRef<NodeJS.Timeout | null>(null);
  const fnRef = useRef<T>(fn);
  const lastCallTimeRef = useRef<number | null>(null);
  const lastArgsRef = useRef<Parameters<T> | null>(null);
  const maxWaitTimeoutRef = useRef<NodeJS.Timeout | null>(null);

  // Update function reference if it changes
  useEffect(() => {
    fnRef.current = fn;
  }, [fn]);

  // Cancel all pending debounced calls
  const cancel = useCallback(() => {
    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current);
      timeoutRef.current = null;
    }
    if (maxWaitTimeoutRef.current) {
      clearTimeout(maxWaitTimeoutRef.current);
      maxWaitTimeoutRef.current = null;
    }
    lastCallTimeRef.current = null;
    lastArgsRef.current = null;
  }, []);

  // Clean up on unmount
  useEffect(() => {
    return cancel;
  }, [cancel]);

  // The debounced function
  const debouncedFn = useCallback(
    (...args: Parameters<T>) => {
      const currentTime = Date.now();
      lastArgsRef.current = args;

      // If we haven't called yet or it's been longer than delay since last call
      const isFirstCall = lastCallTimeRef.current === null;
      const timeSinceLastCall = isFirstCall
        ? Infinity
        : currentTime - lastCallTimeRef.current;

      const shouldCallNow =
        leading && (isFirstCall || timeSinceLastCall >= delay);

      // Update the last call time
      lastCallTimeRef.current = currentTime;

      // Execute immediately if leading is true and it's the first call
      if (shouldCallNow) {
        if (timeoutRef.current) {
          clearTimeout(timeoutRef.current);
          timeoutRef.current = null;
        }
        fnRef.current(...args);
        lastCallTimeRef.current = currentTime;
      } else if (trailing) {
        // Schedule a trailing call
        if (timeoutRef.current) {
          clearTimeout(timeoutRef.current);
        }

        timeoutRef.current = setTimeout(() => {
          if (lastArgsRef.current) {
            fnRef.current(...lastArgsRef.current);
            lastCallTimeRef.current = Date.now();
            timeoutRef.current = null;
          }
        }, delay);
      }

      // Handle maxWait by forcing execution after maxWait time
      if (maxWait !== undefined && !maxWaitTimeoutRef.current && trailing) {
        maxWaitTimeoutRef.current = setTimeout(() => {
          if (lastArgsRef.current) {
            fnRef.current(...lastArgsRef.current);
            lastCallTimeRef.current = Date.now();
            maxWaitTimeoutRef.current = null;

            // Clear the normal timeout since we've forced execution
            if (timeoutRef.current) {
              clearTimeout(timeoutRef.current);
              timeoutRef.current = null;
            }
          }
        }, maxWait);
      }
    },
    [delay, leading, trailing, maxWait],
  );

  return { debouncedFn, cancel };
}
