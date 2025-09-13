import { createLoop } from './core/loop';
import { Input } from './core/input';
import { Ball } from './game/ball';
import { Paddle } from './game/paddle';
import { createRenderer } from './render/canvas';

const canvas = document.getElementById('game') as HTMLCanvasElement;
const renderer = createRenderer(canvas);
const input = new Input(canvas);
const paddle = new Paddle(canvas, input);
const ball = new Ball({ x: 320, y: 200 }, { x: 120, y: -120 });

function update(dt: number) {
  paddle.update(dt);
  ball.update(dt);

  // wall collisions
  if (ball.pos.x < ball.radius || ball.pos.x > canvas.width - ball.radius) {
    ball.vel.x *= -1;
  }
  if (ball.pos.y < ball.radius || ball.pos.y > canvas.height - ball.radius) {
    ball.vel.y *= -1;
  }
}

function render() {
  renderer.clear();
  const ctx = renderer.ctx;
  paddle.render(ctx);
  ball.render(ctx);
}

createLoop(update, render);
