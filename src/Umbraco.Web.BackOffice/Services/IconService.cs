using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Cms.Web.BackOffice.Services
{
    public class IconService : IIconService
    {
        private readonly IOptions<GlobalSettings> _globalSettings;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IAppPolicyCache _cache;

        public IconService(
            IOptions<GlobalSettings> globalSettings,
            IHostingEnvironment hostingEnvironment,
            AppCaches appCaches)
        {
            _globalSettings = globalSettings;
            _hostingEnvironment = hostingEnvironment;
            _cache = appCaches.RuntimeCache;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> GetIcons() => GetIconDictionary();

        /// <inheritdoc />
        public IconModel GetIcon(string iconName)
        {
            if (iconName.IsNullOrWhiteSpace())
            {
                return null;
            }

            var allIconModels = GetIconDictionary();
            if (allIconModels.ContainsKey(iconName))
            {
                return new IconModel
                {
                    Name = iconName,
                    SvgString = allIconModels[iconName]
                };
            }

            return null;
        }

        /// <summary>
        /// Gets an IconModel using values from a FileInfo model
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private IconModel GetIcon(FileInfo fileInfo)
        {
            return fileInfo == null || string.IsNullOrWhiteSpace(fileInfo.Name)
                ? null
                : CreateIconModel(fileInfo.Name.StripFileExtension(), fileInfo.FullName);
        }

        /// <summary>
        /// Gets an IconModel containing the icon name and SvgString
        /// </summary>
        /// <param name="iconName"></param>
        /// <param name="iconPath"></param>
        /// <returns></returns>
        private IconModel CreateIconModel(string iconName, string iconPath)
        {
            try
            {
                var svgContent = System.IO.File.ReadAllText(iconPath);

                var svg = new IconModel
                {
                    Name = iconName,
                    SvgString = svgContent
                };

                return svg;
            }
            catch
            {
                return null;
            }
        }

        private IEnumerable<FileInfo> GetAllIconsFiles()
        {
            var icons = new HashSet<FileInfo>(new CaseInsensitiveFileInfoComparer());

            // add icons from plugins
            var appPluginsDirectoryPath = _hostingEnvironment.MapPathContentRoot(Constants.SystemDirectories.AppPlugins);
            if (Directory.Exists(appPluginsDirectoryPath))
            {
                var appPlugins = new DirectoryInfo(appPluginsDirectoryPath);

                // iterate sub directories of app plugins
                foreach (var dir in appPlugins.EnumerateDirectories())
                {
                    // AppPluginIcons path was previoulsy the wrong case, so we first check for the prefered directory 
                    // and then check the legacy directory.
                    var iconPath = _hostingEnvironment.MapPathContentRoot($"{Constants.SystemDirectories.AppPlugins}/{dir.Name}{Constants.SystemDirectories.PluginIcons}");
                    var iconPathExists = Directory.Exists(iconPath);

                    if (!iconPathExists)
                    {
                        iconPath = _hostingEnvironment.MapPathContentRoot($"{Constants.SystemDirectories.AppPlugins}/{dir.Name}{Constants.SystemDirectories.AppPluginIcons}");
                        iconPathExists = Directory.Exists(iconPath);
                    }

                    if (iconPathExists)
                    {
                        var dirIcons = new DirectoryInfo(iconPath).EnumerateFiles("*.svg", SearchOption.TopDirectoryOnly);
                        icons.UnionWith(dirIcons);
                    }
                }
            }

            // add icons from IconsPath if not already added from plugins
            var coreIconsDirectory = new DirectoryInfo(_hostingEnvironment.MapPathWebRoot($"{_globalSettings.Value.IconsPath}/"));
            var coreIcons = coreIconsDirectory.GetFiles("*.svg");

            icons.UnionWith(coreIcons);

            return icons;
        }

        private class CaseInsensitiveFileInfoComparer : IEqualityComparer<FileInfo>
        {
            public bool Equals(FileInfo one, FileInfo two) => StringComparer.InvariantCultureIgnoreCase.Equals(one.Name, two.Name);

            public int GetHashCode(FileInfo item) => StringComparer.InvariantCultureIgnoreCase.GetHashCode(item.Name);
        }

        private IReadOnlyDictionary<string, string> GetIconDictionary() => _cache.GetCacheItem(
            $"{typeof(IconService).FullName}.{nameof(GetIconDictionary)}",
            () => GetAllIconsFiles()
                .Select(GetIcon)
                .Where(i => i != null)
                .GroupBy(i => i.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().SvgString, StringComparer.OrdinalIgnoreCase)
        );
    }
}
