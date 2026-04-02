# Mini Game Collection

Blazor WebAssembly 기반 미니게임 컬렉션. 6개의 독립적인 미니게임을 포함하며, SOLID 원칙과 DI를 준수한 순수 C# Core 게임 로직으로 구성됨.

## 🎮 Games

| Game | Description | AI | Status |
|------|-------------|----|--------|
| Pattern Memory | 패턴 기억 게임 | No | ✅ Complete |
| Minesweeper | 지뢰찾기 | No | ✅ Complete |
| Sudoku | 스도쿠 퍼즐 | No | ✅ Complete |
| Tetris | 테트리스 | No | ✅ Complete |
| Gomoku | 오목 | Yes (Heuristic) | ✅ Complete |
| Chess | 체스 | Yes (Minimax) | ✅ Complete |

## 🏗️ Architecture

```
MiniGameCollection/
├── Core/                          # 공통 핵심 모듈
│   ├── Interfaces/                # IGame, IGameState 등 공통 인터페이스
│   ├── State/                     # 상태 관리 (StateManager, TransitionRule)
│   ├── Events/                    # 이벤트 시스템 (EventBus, GameEvent)
│   ├── Data/                      # 저장 시스템 (SaveSystem, Serializer)
│   ├── DI/                        # 의존성 주입 (ServiceContainer)
│   └── AI/                        # AI 모듈 (MinimaxAI, Evaluator)
│
├── Games/                         # 개별 게임 모듈
│   ├── PatternMemory/             # 패턴 기억 게임
│   ├── Minesweeper/               # 지뢰찾기
│   ├── Sudoku/                    # 스도쿠
│   ├── Tetris/                    # 테트리스
│   ├── Gomoku/                    # 오목 (AI 포함)
│   └── Chess/                     # 체스 (AI 포함)
│
├── MiniGameCollection.Web/        # Blazor WebAssembly 프론트엔드
│   ├── Pages/                     # 각 게임 페이지 (.razor)
│   ├── Layout/                    # 메인 레이아웃, 네비게이션
│   └── wwwroot/css/               # 게임별 CSS 스타일
│
└── Tests/                         # 단위 테스트
    ├── Core.Tests/
    └── Games.*.Tests/
```

## 🚀 Quick Start

### Requirements
- .NET 8.0 SDK
- 브라우저 (Chrome, Edge, Firefox 등)

### Build & Run
```bash
cd MiniGameCollection.Web
dotnet run
```

브라우저에서 `http://localhost:5259` 접속.

### Build All
```bash
dotnet build MiniGameCollection.sln
```

### Run Tests
```bash
dotnet test MiniGameCollection.sln
```

## 📋 Design Principles

### SOLID
| Principle | Application |
|-----------|-------------|
| **SRP** | Logic, Validator, Evaluator 분리 |
| **OCP** | 새 게임 추가 시 Core 수정 불필요 |
| **LSP** | 모든 IGame 구현체 상호 교체 가능 |
| **ISP** | IGame을 IGameInfo, IGameControllable, IGameSerializable, IGameEventEmitter로 분리 |
| **DIP** | 모든 의존성은 인터페이스를 통해 주입 |

### State Machine
```
None → Ready → Playing → Paused → Playing
                    ↓         ↓
                GameOver   Victory
                    ↓         ↓
                  Ready ←── Ready
```

## 🤖 AI Systems

### Gomoku (오목)
- **Heuristic-based AI** (15x15 보드에서 Minimax 비실용적)
- 패턴 기반 점수 평가 (4연속, 3연속, open-end)
- 공격/방어 우선순위: 승리 > 차단 > 확장
- 난이도: Easy / Normal / Hard (depth 1~3)

### Chess (체스)
- **Minimax with Alpha-Beta Pruning**
- 난이도: Easy (depth 1) / Normal (depth 3) / Hard (depth 5)
- Piece-square table + material evaluation

## 🎯 Key Features

- **Ghost Piece** (Tetris) - 블록 낙하 위치 미리보기
- **Hard Drop** (Tetris) - 더블다운으로 즉시 낙하
- **AI Thinking Indicator** (Gomoku) - AI 계산 중 표시
- **Last Move Highlight** (Gomoku, Chess) - 마지막 수 표시
- **Responsive Design** - HD / Mobile 대응
- **Notes Mode** (Sudoku) - 메모 기능
- **Difficulty Selector** (Sudoku, Gomoku, Chess)

## 📁 Project Structure Details

### Core Module
- `Interfaces/` - IGame (composite), IGameInfo, IGameControllable, IGameSerializable, IGameEventEmitter
- `State/` - StateManager, IStateManager, IStateTransitionRule, DefaultStateTransitionRule
- `Events/` - EventBus, GameEvent, ScoreChangedEvent, GameOverEvent, GameStateChangedEvent
- `Data/` - SaveSystem, ISaveSystem, ISerializer, ISaveStorage, JsonSerializer, GameSettings
- `DI/` - ServiceContainer, IServiceContainer
- `AI/` - MinimaxAI, IAIPlayer, IGameStateEvaluator, AIPlayerFactory

### Game Modules
각 게임은 다음 구조를 따름:
```
Games/[GameName]/
├── [GameName]Game.cs          # IGame 구현체
├── Logic/                     # 게임 로직
├── Models/                    # 데이터 모델
└── AI/                        # AI (있는 경우)
```

### Web Module
- `Pages/` - 각 게임별 Blazor 페이지
- `Layout/` - MainLayout, NavMenu
- `wwwroot/css/` - games.css (전체 게임 스타일), game-grid.css (홈 화면)

## 📝 Development Phases

| Phase | Content | Status |
|-------|---------|--------|
| Phase 0 | Core Architecture | ✅ |
| Phase 1 | Pattern Memory | ✅ |
| Phase 2 | Minesweeper | ✅ |
| Phase 3 | Sudoku | ✅ |
| Phase 4 | Tetris | ✅ |
| Phase 5 | Gomoku + AI | ✅ |
| Phase 6 | Chess + AI | ✅ |
| Phase 7 | Blazor UI | ✅ |

## 📄 License

MIT
