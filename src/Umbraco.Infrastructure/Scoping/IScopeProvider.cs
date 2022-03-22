using System;
using System.Data;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Scoping;

namespace Umbraco.Cms
{
    namespace Core.Scoping
    {
        // TODO: v12 - remove Core.Scoping.IScopeProvider interface
        [Obsolete("Please use either Umbraco.Cms.Infrastructure.Scoping.IScopeProvider or Umbraco.Cms.Core.Scoping.ICoreScopeProvider instead. This interface will be removed in Umbraco 12.")]
        public interface IScopeProvider : Umbraco.Cms.Infrastructure.Scoping.IScopeProvider
        {
        }
    }

    namespace Infrastructure.Scoping
    {
        public interface IScopeProvider : ICoreScopeProvider
        {
            /// <summary>
            /// Creates an ambient scope.
            /// </summary>
            /// <param name="isolationLevel">The transaction isolation level.</param>
            /// <param name="repositoryCacheMode">The repositories cache mode.</param>
            /// <param name="notificationPublisher">An optional notification publisher.</param>
            /// <param name="scopeFileSystems">A value indicating whether to scope the filesystems.</param>
            /// <param name="callContext">A value indicating whether this scope should always be registered in the call context.</param>
            /// <param name="autoComplete">A value indicating whether this scope is auto-completed.</param>
            /// <returns>The created ambient scope.</returns>
            /// <remarks>
            /// <para>The created scope becomes the ambient scope.</para>
            /// <para>If an ambient scope already exists, it becomes the parent of the created scope.</para>
            /// <para>When the created scope is disposed, the parent scope becomes the ambient scope again.</para>
            /// <para>Parameters must be specified on the outermost scope, or must be compatible with the parents.</para>
            /// <para>Auto-completed scopes should be used for read-only operations ONLY. Do not use them if you do not
            /// understand the associated issues, such as the scope being completed even though an exception is thrown.</para>
            /// </remarks>
            IScope CreateScope(
                IsolationLevel isolationLevel = IsolationLevel.Unspecified,
                RepositoryCacheMode repositoryCacheMode = RepositoryCacheMode.Unspecified,
                IScopedNotificationPublisher notificationPublisher = null,
                bool? scopeFileSystems = null,
                bool callContext = false,
                bool autoComplete = false);

            /// <summary>
            /// Creates a detached scope.
            /// </summary>
            /// <returns>A detached scope.</returns>
            /// <param name="isolationLevel">The transaction isolation level.</param>
            /// <param name="repositoryCacheMode">The repositories cache mode.</param>
            /// <param name="scopedNotificationPublisher">An option notification publisher.</param>
            /// <param name="scopeFileSystems">A value indicating whether to scope the filesystems.</param>
            /// <remarks>
            /// <para>A detached scope is not ambient and has no parent.</para>
            /// <para>It is meant to be attached by <see cref="AttachScope"/>.</para>
            /// </remarks>
            // TODO: This is not actually used apart from unit tests - I'm assuming it's maybe used by Deploy?
            IScope CreateDetachedScope(
                IsolationLevel isolationLevel = IsolationLevel.Unspecified,
                RepositoryCacheMode repositoryCacheMode = RepositoryCacheMode.Unspecified,
                IScopedNotificationPublisher scopedNotificationPublisher = null,
                bool? scopeFileSystems = null);
        }
    }
}
