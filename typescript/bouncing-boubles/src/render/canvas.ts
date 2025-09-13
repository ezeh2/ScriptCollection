export function createRenderer(canvas: HTMLCanvasElement) {
  const ctx = canvas.getContext('2d');
  if (!ctx) throw new Error('Cannot get 2D context');
  ctx.imageSmoothingEnabled = false;
  return {
    ctx,
    clear() {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
    },
  };
}
