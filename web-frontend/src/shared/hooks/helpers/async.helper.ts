export async function suspendV8ExecutionAsyncFor(milliseconds: number) {
  return new Promise((resolve) => {
    setTimeout(resolve, milliseconds);
  });
}

export async function suspendWhileTrueAsyncFor(
  conditionCheck: () => boolean,
  checkFunc: () => void = () => undefined,
  poolingMilliseconds: number = 500,
) {
  while (conditionCheck()) {
    await suspendV8ExecutionAsyncFor(poolingMilliseconds);

    checkFunc();
  }
}
