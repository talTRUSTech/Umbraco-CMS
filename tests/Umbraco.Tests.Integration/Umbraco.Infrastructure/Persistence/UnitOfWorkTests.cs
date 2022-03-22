// Copyright (c) Umbraco.
// See LICENSE for more details.

using System;
using NUnit.Framework;
using Umbraco.Cms.Core.DistributedLocking;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Persistence.Sqlite.Services;
using Umbraco.Cms.Persistence.SqlServer.Services;
using Umbraco.Cms.Tests.Common.Testing;
using Umbraco.Cms.Tests.Integration.Testing;
using Constants = Umbraco.Cms.Core.Constants;

namespace Umbraco.Cms.Tests.Integration.Umbraco.Infrastructure.Persistence
{
    [TestFixture]
    [UmbracoTest(Database = UmbracoTestOptions.Database.NewSchemaPerTest)]
    public class UnitOfWorkTests : UmbracoIntegrationTest
    {
        [Test]
        public void ReadLockNonExisting()
        {
            var lockingMechanism = GetRequiredService<IDistributedLockingMechanismFactory>().DistributedLockingMechanism;
            if (lockingMechanism is SqliteDistributedLockingMechanism)
            {
                Assert.Ignore("SqliteDistributedLockingMechanism doesn't query the umbracoLock table for read locks.");
            }

            ICoreScopeProvider provider = ScopeProvider;
            Assert.Throws<ArgumentException>(() =>
            {
                using (ICoreScope scope = provider.CreateCoreScope())
                {
                    scope.EagerReadLock(-666);
                    scope.Complete();
                }
            });
        }

        [Test]
        public void ReadLockExisting()
        {
            ICoreScopeProvider provider = ScopeProvider;
            using (ICoreScope scope = provider.CreateCoreScope())
            {
                scope.EagerReadLock(Constants.Locks.Servers);
                scope.Complete();
            }
        }

        [Test]
        public void WriteLockNonExisting()
        {
            ICoreScopeProvider provider = ScopeProvider;
            Assert.Throws<ArgumentException>(() =>
            {
                using (ICoreScope scope = provider.CreateCoreScope())
                {
                    scope.EagerWriteLock(-666);
                    scope.Complete();
                }
            });
        }

        [Test]
        public void WriteLockExisting()
        {
            ICoreScopeProvider provider = ScopeProvider;
            using (ICoreScope scope = provider.CreateCoreScope())
            {
                scope.EagerWriteLock(Constants.Locks.Servers);
                scope.Complete();
            }
        }
    }
}
