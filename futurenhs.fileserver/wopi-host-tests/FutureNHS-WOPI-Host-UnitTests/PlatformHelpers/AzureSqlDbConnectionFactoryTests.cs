﻿using FutureNHS.WOPIHost.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS_WOPI_Host_UnitTests.PlatformHelpers
{
    [TestClass]
    public sealed class AzureSqlDbConnectionFactoryTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetReadOnlyConnectionAsync_ThrowsIfInvalidStructureForConnectionString()
        {
            var cancellationToken = CancellationToken.None;

            var logger = new Moq.Mock<ILogger<AzureSqlDbConnectionFactory>>().Object;

            IAzureSqlDbConnectionFactory azureSqlDbConnectionFactory = new AzureSqlDbConnectionFactory("read-write", "read-only", logger);

            _ = await azureSqlDbConnectionFactory.GetReadOnlyConnectionAsync(cancellationToken);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task GetReadOnlyConnectionAsync_ThrowsIfCancelled()
        {
            var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            var logger = new Moq.Mock<ILogger<AzureSqlDbConnectionFactory>>().Object;

            cts.Cancel();

            IAzureSqlDbConnectionFactory azureSqlDbConnectionFactory = new AzureSqlDbConnectionFactory("read-write", "read-only", logger);

            _ = await azureSqlDbConnectionFactory.GetReadOnlyConnectionAsync(cancellationToken);
        }

        [TestMethod]
        public async Task GetReadOnlyConnectionAsync_DoesNotThrowIfInvalidConnectionString_ThrowsOnOpen()
        {
            var cancellationToken = CancellationToken.None;

            var logger = new Moq.Mock<ILogger<AzureSqlDbConnectionFactory>>().Object;

            var INVALID_READONLY_CONNECTIONSTRING = "Server=tcp:" + Guid.NewGuid().ToString() + ".database.windows.net,1433;Initial Catalog=initial-catalog;Persist Security Info=False;User ID=my-user-id;Password=my-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;ApplicationIntent=ReadOnly";

            IAzureSqlDbConnectionFactory azureSqlDbConnectionFactory = new AzureSqlDbConnectionFactory("read-write", INVALID_READONLY_CONNECTIONSTRING, logger);

            _ = await azureSqlDbConnectionFactory.GetReadOnlyConnectionAsync(cancellationToken);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetReadWriteConnectionAsync_ThrowsIfInvalidStructureForConnectionString()
        {
            var cancellationToken = CancellationToken.None;

            var logger = new Moq.Mock<ILogger<AzureSqlDbConnectionFactory>>().Object;

            IAzureSqlDbConnectionFactory azureSqlDbConnectionFactory = new AzureSqlDbConnectionFactory("read-write", "read-only", logger);

            _ = await azureSqlDbConnectionFactory.GetReadWriteConnectionAsync(cancellationToken);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task GetReadWriteConnectionAsync_ThrowsIfCancelled()
        {
            var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            var logger = new Moq.Mock<ILogger<AzureSqlDbConnectionFactory>>().Object;

            cts.Cancel();

            IAzureSqlDbConnectionFactory azureSqlDbConnectionFactory = new AzureSqlDbConnectionFactory("read-write", "read-only", logger);

            _ = await azureSqlDbConnectionFactory.GetReadWriteConnectionAsync(cancellationToken);
        }

        [TestMethod]
        public async Task GetReadWriteConnectionAsync_DoesNotThrowIfInvalidConnectionString_ThrowsOnOpen()
        {
            var cancellationToken = CancellationToken.None;

            var logger = new Moq.Mock<ILogger<AzureSqlDbConnectionFactory>>().Object;

            var INVALID_READWRITE_CONNECTIONSTRING = "Server=tcp:" + Guid.NewGuid().ToString() + ".database.windows.net,1433;Initial Catalog=initial-catalog;Persist Security Info=False;User ID=my-user-id;Password=my-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            IAzureSqlDbConnectionFactory azureSqlDbConnectionFactory = new AzureSqlDbConnectionFactory(INVALID_READWRITE_CONNECTIONSTRING, "read-only", logger);

            _ = await azureSqlDbConnectionFactory.GetReadWriteConnectionAsync(cancellationToken);
        }
    }
}
