import { Vec2 } from "../core/math";

export class Ball {
  pos: Vec2;
  vel: Vec2;
  radius: number;

  constructor(pos: Vec2, vel: Vec2, radius = 4) {
    this.pos = { ...pos };
    this.vel = { ...vel };
    this.radius = radius;
  }

  update(dt: number) {
    // dt in ms, convert to seconds for velocity
    const sec = dt / 1000;
    this.pos.x += this.vel.x * sec;
    this.pos.y += this.vel.y * sec;
  }

  render(ctx: CanvasRenderingContext2D) {
    ctx.beginPath();
    ctx.arc(this.pos.x, this.pos.y, this.radius, 0, Math.PI * 2);
    ctx.fill();
  }
}
