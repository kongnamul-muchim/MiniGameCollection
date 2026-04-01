## ADDED Requirements

### Requirement: Game State Service
The system SHALL provide a scoped service that manages the current game instance and state.

#### Scenario: Game instance creation
- **WHEN** a game page loads
- **THEN** a new instance of the selected game is created and initialized

#### Scenario: State subscription
- **WHEN** a game component subscribes to state changes
- **THEN** it receives notifications when the game state changes

#### Scenario: Component disposal
- **WHEN** user navigates away from a game page
- **THEN** the game instance is properly disposed and events are unsubscribed

### Requirement: Event Bus Integration
The system SHALL integrate with the existing IEventBus for game events.

#### Scenario: Score change event
- **WHEN** a game emits a ScoreChangedEvent
- **THEN** the UI updates to display the new score

#### Scenario: Game over event
- **WHEN** a game emits a GameOverEvent
- **THEN** the UI displays the game over screen with results

#### Scenario: State change event
- **WHEN** a game emits a GameStateChangedEvent
- **THEN** the UI re-renders to reflect the new state

### Requirement: Browser Storage Integration
The system SHALL use browser localStorage for persisting game settings.

#### Scenario: Settings save
- **WHEN** user changes game settings
- **THEN** settings are saved to localStorage

#### Scenario: Settings load
- **WHEN** user returns to a game
- **THEN** previously saved settings are loaded from localStorage
