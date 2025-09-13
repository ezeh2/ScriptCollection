export class Input {
  left = false;
  right = false;
  mouseX = 0;

  constructor(private canvas: HTMLCanvasElement) {
    window.addEventListener('keydown', (e) => {
      if (e.key === 'ArrowLeft') this.left = true;
      if (e.key === 'ArrowRight') this.right = true;
    });
    window.addEventListener('keyup', (e) => {
      if (e.key === 'ArrowLeft') this.left = false;
      if (e.key === 'ArrowRight') this.right = false;
    });

    canvas.addEventListener('mousemove', (e) => {
      const rect = canvas.getBoundingClientRect();
      this.mouseX = e.clientX - rect.left;
    });
  }
}
