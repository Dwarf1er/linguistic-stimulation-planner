using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace LinguisticStimulationPlanner.Services
{
    public class TempFileService
    {
        private static readonly string HashFileName = "app_hash.txt";
        private static readonly string CurrentHash = GetCurrentHash();
        private readonly ILogger<TempFileService> _logger;

        public TempFileService(ILogger<TempFileService> logger)
        {
            _logger = logger;
        }

        public void CleanTemporaryFiles()
        {
            string tempDirectory = GetTempDirectory();
            string hashFilePath = Path.Combine(tempDirectory, HashFileName);

            try
            {
                if (File.Exists(hashFilePath))
                {
                    string previousHash = File.ReadAllText(hashFilePath);

                    if (previousHash == CurrentHash)
                    {
                        return;
                    }

                    if (Directory.Exists(tempDirectory))
                    {
                        Directory.Delete(tempDirectory, true);
                    }

                    Directory.CreateDirectory(tempDirectory);
                    File.WriteAllText(hashFilePath, CurrentHash);
                }
                else
                {
                    if (!Directory.Exists(tempDirectory))
                    {
                        Directory.CreateDirectory(tempDirectory);
                    }

                    File.WriteAllText(hashFilePath, CurrentHash);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while cleaning temporary files: {ex.Message}");
            }
        }

        private static string GetTempDirectory()
        {
            string tempDir = "";

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                tempDir = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), ".net", Assembly.GetExecutingAssembly().GetName().Name);
            }

            else if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                tempDir = Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".net", Assembly.GetExecutingAssembly().GetName().Name);
            }

            return tempDir;
        }

        private static string GetCurrentHash()
        {
            using var sha256 = SHA256.Create();
            var exePath = Assembly.GetExecutingAssembly().Location;

            using var stream = File.OpenRead(exePath);
            var hash = sha256.ComputeHash(stream);

            return Convert.ToBase64String(hash);
        }
    }
}
