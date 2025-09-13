# Specification for Browser-based Implementation of 'Bouncing Boubles' (Atari ST Inspired)

## Overview
This document describes the functional and technical specification for implementing a browser-based clone of the Atari ST
Public Domain game "Bouncing Boubles". The goal is to reimplement the game mechanics and structure for modern browsers.

## Gameplay
- Breakout/Arkanoid-style gameplay: paddle, bouncing ball(s), destructible bricks.
- Control paddle with keyboard (left/right) or mouse.
- Goal: Clear all destructible bricks to advance to the next level.
- HUD: Score, Level, and Lives (called "Ships" in original version).
- Difficulty increases with higher ball speeds, more balls, and harder brick layouts.

## Game Structure
The game is composed of distinct modules and systems:

### Core Systems
- **Loop**: Fixed timestep physics update + requestAnimationFrame render.
- **Input**: Keyboard & mouse abstraction for paddle movement and start/pause.
- **Math**: Vector operations, collision helpers, random number generator.

### Game Entities
- **Ball**: Position, velocity, update movement, detect collisions.
- **Paddle**: Position, size, reacts to input, controls reflection angle.
- **Brick**: Holds type (normal, multi-hit, indestructible, power-up), durability, and render function.

### Game World
- **Level**: 2D grid defining brick placement and properties.
- **Power-ups**: Spawn when special bricks are destroyed; alter game state (multi-ball, larger paddle, slow ball).

### Game States (Finite State Machine)
- **Title**: Initial splash, waits for start key.
- **Playing**: Normal gameplay update and rendering.
- **Paused**: Input frozen until resume command.
- **Level Clear**: Transition after destroying all bricks.
- **Game Over**: Final score display and restart option.

### Rendering
- Canvas-based renderer at 640x400 resolution (Atari ST mono look).
- Scaling logic for modern displays without smoothing.
- HUD showing score, level, lives.

## Physics
- Ball reflects at consistent speed off walls, paddle, and bricks.
- Paddle reflection depends on impact point relative to paddle center.
- Brick collision: circle vs. axis-aligned rectangle test.
- Multiple collision resolution order: walls → paddle → bricks.
- Swept collision or small sub-steps prevent tunneling at high ball speeds.

## Rendering
- Single HTML5 Canvas element for drawing.
- Atari ST style: monochrome palette or retro color scheme.
- Separate renderer module to decouple logic from graphics.
- Crisp scaling: disable image smoothing for pixel-art look.

## File Structure
Proposed project structure:

```
index.html           : entry point with canvas element
src/core/loop.js     : main loop (update/render, fixed timestep)
src/core/input.js    : input management (keyboard/mouse)
src/core/math.js     : vector, clamp, collision helpers
src/game/state.js    : finite state machine
src/game/ball.js     : ball class and logic
src/game/paddle.js   : paddle class and logic
src/game/brick.js    : brick class and variants
src/game/level.js    : level definitions and loader
src/render/canvas.js : all rendering code
src/main.js          : glue code (initialization)
```

## Estimated Code Size
- Minimal version: 180–300 lines
- With state machine, HUD, clean architecture: 350–600 lines
- With power-ups, multi-ball, sound: 700–1200 lines
- Recommended target: 500–800 lines for a maintainable implementation

## Conclusion
This specification includes gameplay mechanics, physics, rendering, file structure, and modular system breakdown. 
With these details, developers can implement 'Bouncing Boubles' in a browser using JavaScript and HTML5 Canvas while 
maintaining clarity and modularity.
