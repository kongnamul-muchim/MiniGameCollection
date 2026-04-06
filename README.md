# 🎮 Mini Game Collection

> **Blazor WebAssembly 기반 미니게임 컬렉션**  
> SOLID 원칙과 DI 를 준수한 순수 C# Core 게임 로직으로 구성된 포트폴리오 프로젝트입니다.

### 🔗 Links

| | |
|---|---|
| **🌐 Live Demo** | [배포 URL 입력 Pending](#) |
| **📂 GitHub** | https://github.com/kongnamul-muchim/MiniGameCollection |
| **📄 Portfolio** | [docs/PORTFOLIO.md](./docs/PORTFOLIO.md) |

---

## 🎯 Quick View

| Feature | Description |
|---------|-------------|
| **Games** | 6 개의 미니게임 (패턴기억, 지뢰찾기, 스도쿠, 테트리스, 오목, 체스) |
| **Tech** | Blazor WebAssembly, .NET 8.0, C# 12 |
| **Architecture** | SOLID, DI, Event-Driven, State Machine |
| **AI** | Heuristic (오목), Minimax + Alpha-Beta Pruning (체스) |
| **Testing** | xUnit, Moq 단위 테스트 |

---

## 🎮 Games

| Game | Description | AI | Status |
|------|-------------|----|--------|
| Pattern Memory | 패턴 기억 게임 | ❌ | ✅ |
| Minesweeper | 지뢰찾기 | ❌ | ✅ |
| Sudoku | 스도쿠 퍼즐 | ❌ | ✅ |
| Tetris | 테트리스 | ❌ | ✅ |
| Gomoku | 오목 | ✅ Heuristic | ✅ |
| Chess | 체스 | ✅ Minimax | ✅ |

---

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

---

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

---

## 📋 Design Principles

### SOLID

| Principle | Application |
|-----------|-------------|
| **SRP** | Logic, Validator, Evaluator 분리 |
| **OCP** | 새 게임 추가 시 Core 수정 불필요 |
| **LSP** | 모든 IGame 구현체 상호 교체 가능 |
| **ISP** | IGame 을 IGameInfo, IGameControllable, IGameSerializable, IGameEventEmitter 로 분리 |
| **DIP** | 모든 의존성은 인터페이스를 통해 주입 |

### State Machine
```
None → Ready → Playing → Paused → Playing
                    ↓         ↓
                GameOver   Victory
                    ↓         ↓
                  Ready ←── Ready
```

---

## 🤖 AI Systems

### Gomoku (오목)
- **Heuristic-based AI** (15x15 보드에서 Minimax 비실용적)
- 패턴 기반 점수 평가 (4 연석, 3 연석, open-end)
- 공격/방어 우선순위: 승리 > 차단 > 확장
- 난이도: Easy / Normal / Hard (depth 1~3)

### Chess (체스)
- **Minimax with Alpha-Beta Pruning**
- 난이도: Easy (depth 1) / Normal (depth 3) / Hard (depth 5)
- Piece-square table + material evaluation

---

## 🎯 Key Features

- **Ghost Piece** (Tetris) - 블록 낙하 위치 미리보기
- **Hard Drop** (Tetris) - 더블다운으로 즉시 낙하
- **AI Thinking Indicator** (Gomoku) - AI 계산 중 표시
- **Last Move Highlight** (Gomoku, Chess) - 마지막 수 표시
- **Responsive Design** - HD / Mobile 대응
- **Notes Mode** (Sudoku) - 메모 기능
- **Difficulty Selector** (Sudoku, Gomoku, Chess)

---

## 🛠️ Tech Stack

| Category | Technology |
|----------|------------|
| **Frontend** | Blazor WebAssembly |
| **Language** | C# 12, .NET 8.0 |
| **Architecture** | Component-based, DI |
| **State Management** | Custom StateManager |
| **Testing** | xUnit, Moq |
| **CI/CD** | GitHub Actions, Vercel |

---

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

---

## 📄 License

MIT
