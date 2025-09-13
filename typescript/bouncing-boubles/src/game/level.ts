import { Brick, BrickType } from "./brick";

export type LevelDefinition = (BrickType | 0)[][];

export function createLevel(def: LevelDefinition, brickWidth = 32, brickHeight = 16): Brick[] {
  const bricks: Brick[] = [];
  for (let y = 0; y < def.length; y++) {
    for (let x = 0; x < def[y].length; x++) {
      const type = def[y][x];
      if (type) {
        bricks.push(new Brick({ x: x * brickWidth, y: y * brickHeight }, brickWidth, brickHeight, type));
      }
    }
  }
  return bricks;
}
