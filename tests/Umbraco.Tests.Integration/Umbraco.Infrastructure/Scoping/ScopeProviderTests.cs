using NUnit.Framework;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Tests.Common.Testing;
using Umbraco.Cms.Tests.Integration.Testing;

namespace Umbraco.Cms.Tests.Integration.Umbraco.Infrastructure.Scoping;

[TestFixture]
[UmbracoTest(Database = UmbracoTestOptions.Database.None)]
public class ScopeProviderTests : UmbracoIntegrationTest
{
    [Test]
    public void ScopeProvider_WithMultipleRegistrations_AllCanBeResolvedToSameInstance()
    {
        var scopeProvider = GetRequiredService<ScopeProvider>();
        var coreScopeProvider = GetRequiredService<ICoreScopeProvider>();
        var legacyScopeProvider = GetRequiredService<Cms.Core.Scoping.IScopeProvider>();
        var infrastructureScopeProvider = GetRequiredService<global::Umbraco.Cms.Infrastructure.Scoping.IScopeProvider>();

        Assert.Multiple(() =>
        {
            Assert.AreSame(scopeProvider, coreScopeProvider);
            Assert.AreSame(scopeProvider, legacyScopeProvider);
            Assert.AreSame(scopeProvider, infrastructureScopeProvider);
        });
    }
}
