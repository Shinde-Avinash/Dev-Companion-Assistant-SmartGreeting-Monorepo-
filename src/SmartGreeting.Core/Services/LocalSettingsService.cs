using System;
using System.IO;
using System.Text.Json;
using SmartGreeting.Core.Models;

namespace SmartGreeting.Core.Services
{
    public interface ISettingsService
    {
        GreetingSettings CurrentSettings { get; }
        GreetingSettings LoadSettings();
        void SaveSettings(GreetingSettings settings);
        DateTime? GetLastGreetingTimeUtc();
        void RecordGreetingShown(string messageId);
        string? GetLastMessageId();
    }

    public class LocalSettingsService : ISettingsService
    {
        private readonly string _settingsFilePath;
        private readonly string _stateFilePath;
        private readonly ILoggerService _logger;
        private GreetingSettings _cachedSettings;
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { WriteIndented = true };

        public LocalSettingsService(ILoggerService? logger = null, string? baseDirectory = null)
        {
            _logger = logger ?? new FileLoggerService();
            string rootDir = baseDirectory ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SmartGreeting");

            _settingsFilePath = Path.Combine(rootDir, "settings.json");
            _stateFilePath = Path.Combine(rootDir, "state.json");
            _cachedSettings = LoadSettings();
        }

        public GreetingSettings CurrentSettings => _cachedSettings;

        public GreetingSettings LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    string json = File.ReadAllText(_settingsFilePath);
                    var loaded = JsonSerializer.Deserialize<GreetingSettings>(json);
                    if (loaded != null)
                    {
                        ValidateSettings(loaded);
                        _cachedSettings = loaded;
                        return _cachedSettings;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to load settings file, using defaults.", ex);
            }

            _cachedSettings = new GreetingSettings();
            SaveSettings(_cachedSettings);
            return _cachedSettings;
        }

        public void SaveSettings(GreetingSettings settings)
        {
            try
            {
                ValidateSettings(settings);
                _cachedSettings = settings.Clone();

                string dir = Path.GetDirectoryName(_settingsFilePath)!;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                string json = JsonSerializer.Serialize(settings, JsonOptions);
                File.WriteAllText(_settingsFilePath, json);
                _logger.LogInfo("Settings successfully persisted.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to persist settings.", ex);
            }
        }

        public DateTime? GetLastGreetingTimeUtc()
        {
            try
            {
                if (File.Exists(_stateFilePath))
                {
                    string json = File.ReadAllText(_stateFilePath);
                    var state = JsonSerializer.Deserialize<LocalState>(json);
                    return state?.LastShownUtc;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to read state file: {ex.Message}");
            }
            return null;
        }

        public string? GetLastMessageId()
        {
            try
            {
                if (File.Exists(_stateFilePath))
                {
                    string json = File.ReadAllText(_stateFilePath);
                    var state = JsonSerializer.Deserialize<LocalState>(json);
                    return state?.LastMessageId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to read last message id: {ex.Message}");
            }
            return null;
        }

        public void RecordGreetingShown(string messageId)
        {
            try
            {
                string dir = Path.GetDirectoryName(_stateFilePath)!;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var state = new LocalState
                {
                    LastShownUtc = DateTime.UtcNow,
                    LastMessageId = messageId
                };

                string json = JsonSerializer.Serialize(state, JsonOptions);
                File.WriteAllText(_stateFilePath, json);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to record greeting state.", ex);
            }
        }

        private static void ValidateSettings(GreetingSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Name))
            {
                settings.Name = "Developer";
            }

            if (settings.AutoDismissSeconds < 3)
            {
                settings.AutoDismissSeconds = 3;
            }
            else if (settings.AutoDismissSeconds > 60)
            {
                settings.AutoDismissSeconds = 60;
            }
        }

        private class LocalState
        {
            public DateTime LastShownUtc { get; set; }
            public string? LastMessageId { get; set; }
        }
    }
}
