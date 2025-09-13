import { Vec2 } from "../core/math";
import { Input } from "../core/input";

export class Paddle {
  pos: Vec2;
  width: number;
  height: number;
  speed: number;
  private bounds: { w: number; h: number };

  constructor(private canvas: HTMLCanvasElement, private input: Input) {
    this.width = 60;
    this.height = 10;
    this.speed = 300; // pixels per second
    this.pos = { x: canvas.width / 2, y: canvas.height - 30 };
    this.bounds = { w: canvas.width, h: canvas.height };
  }

  update(dt: number) {
    const sec = dt / 1000;
    if (this.input.left) this.pos.x -= this.speed * sec;
    if (this.input.right) this.pos.x += this.speed * sec;
    // mouse steering
    if (!this.input.left && !this.input.right) {
      this.pos.x = this.input.mouseX;
    }
    // clamp
    const half = this.width / 2;
    if (this.pos.x - half < 0) this.pos.x = half;
    if (this.pos.x + half > this.bounds.w) this.pos.x = this.bounds.w - half;
  }

  render(ctx: CanvasRenderingContext2D) {
    ctx.fillRect(this.pos.x - this.width / 2, this.pos.y - this.height / 2, this.width, this.height);
  }
}
