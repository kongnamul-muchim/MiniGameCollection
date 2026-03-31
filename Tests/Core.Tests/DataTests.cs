using Core.Data;
using Xunit;

namespace Core.Tests;

public class DataTests
{
    public class GameSettingsTests
    {
        [Fact]
        public void GameSettings_ShouldInitializeWithDefaults()
        {
            var settings = new GameSettings();
            
            Assert.NotNull(settings.HighScores);
            Assert.NotNull(settings.GameProgress);
            Assert.Empty(settings.HighScores);
            Assert.Empty(settings.GameProgress);
            Assert.Equal(0, settings.TotalPlayTime);
        }
    }

    public class JsonSerializerTests
    {
        private readonly JsonSerializer _serializer = new();

        [Fact]
        public void JsonSerializer_ShouldSerializeObject()
        {
            var data = new TestObject { Name = "Test", Value = 42 };
            
            var json = _serializer.Serialize(data);
            
            Assert.Contains("\"Name\"", json);
            Assert.Contains("\"Test\"", json);
            Assert.Contains("\"Value\"", json);
            Assert.Contains("42", json);
        }

        [Fact]
        public void JsonSerializer_ShouldDeserializeObject()
        {
            var json = "{\"Name\":\"Test\",\"Value\":42}";
            
            var result = _serializer.Deserialize<TestObject>(json);
            
            Assert.Equal("Test", result.Name);
            Assert.Equal(42, result.Value);
        }

        private class TestObject
        {
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }
    }

    public class SaveSystemTests
    {
        private class MockStorage : ISaveStorage
        {
            private readonly Dictionary<string, string> _storage = new();

            public Task SaveAsync(string key, string data)
            {
                _storage[key] = data;
                return Task.CompletedTask;
            }

            public Task<string?> LoadAsync(string key)
            {
                return Task.FromResult(_storage.TryGetValue(key, out var value) ? value : null);
            }

            public Task<bool> ExistsAsync(string key)
            {
                return Task.FromResult(_storage.ContainsKey(key));
            }

            public Task DeleteAsync(string key)
            {
                _storage.Remove(key);
                return Task.CompletedTask;
            }
        }

        private readonly SaveSystem _saveSystem;
        private readonly MockStorage _storage;

        public SaveSystemTests()
        {
            _storage = new MockStorage();
            _saveSystem = new SaveSystem(new JsonSerializer(), _storage);
        }

        [Fact]
        public async Task SaveSystem_ShouldSaveAndLoadGame()
        {
            var gameState = new TestGameState { Score = 100, Level = 5 };
            
            await _saveSystem.SaveGameAsync("test", gameState);
            var loaded = await _saveSystem.LoadGameAsync<TestGameState>("test");
            
            Assert.NotNull(loaded);
            Assert.Equal(100, loaded.Score);
            Assert.Equal(5, loaded.Level);
        }

        [Fact]
        public async Task SaveSystem_ShouldReturnNullWhenNoSavedGame()
        {
            var loaded = await _saveSystem.LoadGameAsync<TestGameState>("nonexistent");
            
            Assert.Null(loaded);
        }

        [Fact]
        public async Task SaveSystem_ShouldDetectSavedGame()
        {
            Assert.False(await _saveSystem.HasSavedGameAsync("test"));
            
            await _saveSystem.SaveGameAsync("test", new TestGameState());
            
            Assert.True(await _saveSystem.HasSavedGameAsync("test"));
        }

        private class TestGameState
        {
            public int Score { get; set; }
            public int Level { get; set; }
        }
    }
}