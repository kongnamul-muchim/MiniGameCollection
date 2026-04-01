using Core.Interfaces;
using Core.State;
using Xunit;

namespace Core.Tests;

public class StateTests
{
    public class StateManagerTests
    {
        [Fact]
        public void StateManager_ShouldInitializeWithNone()
        {
            var rule = new DefaultStateTransitionRule();
            var manager = new StateManager(rule);
            
            Assert.Equal(GameState.None, manager.CurrentState);
        }

        [Fact]
        public void StateManager_ShouldTransitionState()
        {
            var rule = new DefaultStateTransitionRule();
            var manager = new StateManager(rule);
            
            manager.ChangeState(GameState.Ready);
            Assert.Equal(GameState.Ready, manager.CurrentState);
            
            manager.ChangeState(GameState.Playing);
            Assert.Equal(GameState.Playing, manager.CurrentState);
        }

        [Fact]
        public void StateManager_ShouldFireOnStateChanged()
        {
            var rule = new DefaultStateTransitionRule();
            var manager = new StateManager(rule);
            
            GameState? previousState = null;
            GameState? newState = null;
            var eventFired = false;
            
            manager.OnStateChanged += (prev, next) =>
            {
                previousState = prev;
                newState = next;
                eventFired = true;
            };
            
            manager.ChangeState(GameState.Ready);
            
            Assert.True(eventFired);
            Assert.Equal(GameState.None, previousState);
            Assert.Equal(GameState.Ready, newState);
        }

        [Fact]
        public void StateManager_ShouldNotAllowInvalidTransition()
        {
            var rule = new DefaultStateTransitionRule();
            var manager = new StateManager(rule);
            
            Assert.False(manager.CanTransitionTo(GameState.Playing));
            
            manager.ChangeState(GameState.Playing);
            
            Assert.Equal(GameState.None, manager.CurrentState);
        }

        [Fact]
        public void StateManager_ShouldRequireTransitionRule()
        {
            Assert.Throws<ArgumentNullException>(() => new StateManager(null!));
        }
    }

    public class DefaultStateTransitionRuleTests
    {
        private readonly DefaultStateTransitionRule _rule = new();

        [Theory]
        [InlineData(GameState.None, GameState.Ready)]
        [InlineData(GameState.Ready, GameState.Playing)]
        [InlineData(GameState.Playing, GameState.Paused)]
        [InlineData(GameState.Playing, GameState.GameOver)]
        [InlineData(GameState.Playing, GameState.Victory)]
        [InlineData(GameState.Paused, GameState.Playing)]
        [InlineData(GameState.Paused, GameState.Ready)]
        [InlineData(GameState.GameOver, GameState.Ready)]
        [InlineData(GameState.Victory, GameState.Ready)]
        public void DefaultRule_ShouldAllowValidTransitions(GameState from, GameState to)
        {
            Assert.True(_rule.CanTransition(from, to));
        }

        [Theory]
        [InlineData(GameState.None, GameState.Playing)]
        [InlineData(GameState.None, GameState.Paused)]
        [InlineData(GameState.None, GameState.GameOver)]
        [InlineData(GameState.None, GameState.Victory)]
        [InlineData(GameState.Ready, GameState.Paused)]
        [InlineData(GameState.Ready, GameState.GameOver)]
        [InlineData(GameState.Ready, GameState.Victory)]
        [InlineData(GameState.Playing, GameState.None)]
        [InlineData(GameState.Paused, GameState.GameOver)]
        [InlineData(GameState.Paused, GameState.Victory)]
        [InlineData(GameState.Paused, GameState.None)]
        [InlineData(GameState.GameOver, GameState.Playing)]
        [InlineData(GameState.GameOver, GameState.Paused)]
        [InlineData(GameState.GameOver, GameState.Victory)]
        [InlineData(GameState.GameOver, GameState.None)]
        [InlineData(GameState.Victory, GameState.Playing)]
        [InlineData(GameState.Victory, GameState.Paused)]
        [InlineData(GameState.Victory, GameState.GameOver)]
        [InlineData(GameState.Victory, GameState.None)]
        public void DefaultRule_ShouldDenyInvalidTransitions(GameState from, GameState to)
        {
            Assert.False(_rule.CanTransition(from, to));
        }
    }
}