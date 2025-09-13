import { Vec2 } from "../core/math";

export type BrickType = "normal" | "indestructible" | "powerup";

export class Brick {
  pos: Vec2;
  width: number;
  height: number;
  type: BrickType;
  hits: number;

  constructor(pos: Vec2, width: number, height: number, type: BrickType = "normal", hits = 1) {
    this.pos = pos;
    this.width = width;
    this.height = height;
    this.type = type;
    this.hits = hits;
  }

  render(ctx: CanvasRenderingContext2D) {
    ctx.fillRect(this.pos.x, this.pos.y, this.width, this.height);
  }
}
