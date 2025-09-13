export type UpdateFn = (dt: number) => void;
export type RenderFn = () => void;

/**
 * Creates a basic fixed timestep loop.
 * @param update called with a fixed timestep in ms
 * @param render called once per animation frame after updates
 * @param timestep desired timestep in ms (default 16.666ms ~60Hz)
 */
export function createLoop(update: UpdateFn, render: RenderFn, timestep = 1000 / 60) {
  let last = 0;
  let accumulator = 0;

  function frame(time: number) {
    if (!last) last = time;
    const delta = time - last;
    last = time;
    accumulator += delta;

    while (accumulator >= timestep) {
      update(timestep);
      accumulator -= timestep;
    }

    render();
    requestAnimationFrame(frame);
  }

  requestAnimationFrame(frame);
}
