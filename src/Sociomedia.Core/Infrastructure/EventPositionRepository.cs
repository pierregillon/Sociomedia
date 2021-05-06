﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sociomedia.Core.Application;

namespace Sociomedia.Core.Infrastructure
{
    public class EventPositionRepository : IEventPositionRepository
    {
        private readonly ILogger _logger;
        private const string FILE_NAME = ".StreamPosition";

        private static string FilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FILE_NAME);

        public EventPositionRepository(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<long?> GetLastProcessedPosition()
        {
            if (File.Exists(FilePath)) {
                var fileContent = await File.ReadAllTextAsync(FilePath);
                return long.Parse(fileContent);
            }
            return null;
        }

        public async Task Save(long position)
        {
            try {
                await File.WriteAllTextAsync(FilePath, position.ToString());
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"[{this.GetType().DisplayableName()}] Unable to save the position {position}.");
            }
        }
    }
}