## Context

The MiniGameCollection project currently consists of 6 console-based games sharing a common IGame interface in the Core project. Each game is fully implemented with game logic, state management, and event emission. The goal is to create a web frontend without duplicating game logic.

**Current Architecture:**
- Core project: Interfaces, state management, events, DI container, AI, save system
- Games projects: 6 independent game implementations, each referencing Core
- All games target .NET 8.0

**Constraints:**
- Reuse existing game logic without modification
- Support Blazor WebAssembly (client-side only, no server required)
- Must work on desktop and mobile browsers

## Goals / Non-Goals

**Goals:**
- Create a Blazor WebAssembly application hosting all 6 games
- Reuse 100% of existing game logic from Core and Games projects
- Provide responsive UI that works on desktop and mobile
- Enable direct game access via URL (deep linking)
- Maintain clean separation between game logic and UI

**Non-Goals:**
- Multiplayer functionality (all games remain single-player)
- Server-side state persistence (use browser localStorage only)
- Modifying existing game logic or Core interfaces
- Progressive Web App (PWA) features in initial implementation

## Decisions

### Decision 1: Blazor WebAssembly over Blazor Server
**Choice:** Blazor WebAssembly (client-side)
**Rationale:** 
- No server infrastructure required - can be hosted on any static file host
- Game state remains in browser memory, reducing latency
- Existing .NET code compiles directly to WebAssembly
- Lower operational cost (no server to maintain)
**Alternatives Considered:**
- Blazor Server: Requires SignalR connection and server resources
- React/Angular + API: Would require rewriting game logic in JavaScript

### Decision 2: One Component Per Game over Generic Renderer
**Choice:** Create dedicated Blazor component for each game
**Rationale:**
- Each game has unique rendering requirements (grid, board, falling pieces)
- Easier to maintain and optimize game-specific UI
- Allows custom touch/click handling per game type
**Alternatives Considered:**
- Generic board renderer: Too abstract, would need complex configuration

### Decision 3: Scoped Game State Service
**Choice:** Use Blazor scoped DI for game instances
**Rationale:**
- Each game session gets its own instance
- Automatic disposal when navigating away
- Integrates with existing DI patterns in Core
**Alternatives Considered:**
- Singleton: Would share state across users/sessions (incorrect)
- Manual instantiation: Loses DI benefits and lifecycle management

### Decision 4: CSS Grid for Responsive Layout
**Choice:** CSS Grid and Flexbox for all layouts
**Rationale:**
- Native browser support, no framework dependency
- Excellent for game board layouts
- Media queries handle mobile/desktop transitions
**Alternatives Considered:**
- Bootstrap: Adds unnecessary dependency weight
- CSS frameworks: Overkill for simple responsive needs

## Risks / Trade-offs

| Risk | Mitigation |
|------|------------|
| WebAssembly load time on slow connections | Pre-compress WASM files, use CDN for static assets |
| Touch input precision on small screens | Implement touch-friendly hit areas, add zoom for detailed games |
| Memory usage in browser (WASM + game state) | Dispose game instances on navigation, limit saved state size |
| Browser compatibility (older browsers) | Document minimum browser requirements, provide graceful degradation |
| Tetris animation performance | Use CSS animations where possible, limit redraw frequency |

## Migration Plan

**Phase 1: Project Setup**
1. Create Blazor WebAssembly project structure
2. Add project references to Core and all Games projects
3. Configure build and publish profiles

**Phase 2: Core Components**
1. Implement game state service with DI integration
2. Create base game component with common functionality
3. Implement navigation and routing

**Phase 3: Game Components**
1. Implement each game's UI component (6 total)
2. Wire up input handling and event subscriptions
3. Test each game in isolation

**Phase 4: Polish**
1. Add responsive CSS for mobile/desktop
2. Implement browser storage for settings
3. Testing across browsers and devices

**Rollback Strategy:**
- Web project is independent - can be removed from solution without affecting existing code
- No database or API changes to rollback

## Open Questions

1. Should we add a loading screen while WASM initializes?
2. Do we need analytics/tracking for game usage?
3. Should high scores be persisted (and if so, where)?
