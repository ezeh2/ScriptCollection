export enum GameState {
  Title,
  Playing,
  Paused,
  LevelClear,
  GameOver,
}

export class StateMachine {
  state: GameState = GameState.Title;

  change(newState: GameState) {
    this.state = newState;
  }
}
