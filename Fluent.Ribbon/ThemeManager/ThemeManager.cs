﻿// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Security;
    using System.Windows;
    using JetBrains.Annotations;
    using Microsoft.Win32;

    /// <summary>
    /// A class that allows for the detection and alteration of a theme and accent.
    /// </summary>
    public static class ThemeManager
    {
        private const string AppThemeLight = "BaseLight";
        private const string AppThemeDark = "BaseDark";

#pragma warning disable SA1309 // Field names must not begin with underscore
        private static IList<Accent> _accents;
        private static IList<AppTheme> _appThemes;
#pragma warning restore SA1309 // Field names must not begin with underscore

        /// <summary>
        /// Gets a list of all of default accents.
        /// </summary>
        public static IEnumerable<Accent> Accents
        {
            get
            {
                if (_accents != null)
                {
                    return _accents;
                }

                var colors = new[]
                {
                                       "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt",
                                       "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna"
                                   };

                _accents = new List<Accent>(colors.Length);

                try
                {
                    foreach (var color in colors)
                    {
                        var resourceAddress = new Uri($"pack://application:,,,/Fluent;component/Themes/Accents/{color}.xaml");
                        _accents.Add(new Accent(color, resourceAddress));
                    }
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("This exception happens because you are maybe running that code out of the scope of a WPF application. Most likely because you are testing your configuration inside a unit test.", e);
                }

                return _accents;
            }
        }

        /// <summary>
        /// Gets a list of all default themes.
        /// </summary>
        public static IEnumerable<AppTheme> AppThemes
        {
            get
            {
                if (_appThemes != null)
                {
                    return _appThemes;
                }

                var themes = new[] { AppThemeLight, AppThemeDark };

                _appThemes = new List<AppTheme>(themes.Length);

                try
                {
                    foreach (var color in themes)
                    {
                        var resourceAddress = new Uri($"pack://application:,,,/Fluent;component/Themes/Colors/{color}.xaml");
                        _appThemes.Add(new AppTheme(color, resourceAddress));
                    }
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("This exception happens because you are maybe running that code out of the scope of a WPF application. Most likely because you are testing your configuration inside a unit test.", e);
                }

                return _appThemes;
            }
        }

        /// <summary>
        /// Adds an accent with the given name.
        /// </summary>
        /// <returns>true if the accent does not exists and can be added.</returns>
        public static bool AddAccent([NotNull] string name, [NotNull] Uri resourceAddress)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (resourceAddress == null)
            {
                throw new ArgumentNullException(nameof(resourceAddress));
            }

            var accentExists = GetAccent(name) != null;
            if (accentExists)
            {
                return false;
            }

            _accents.Add(new Accent(name, resourceAddress));
            return true;
        }

        /// <summary>
        /// Adds an accent with the given name and resource dictionary.
        /// </summary>
        /// <param name="name">The name of the new Accent.</param>
        /// <param name="resourceDictionary">The ResourceDictionary of the accent.</param>
        /// <returns>true if the accent does not exists and can be added.</returns>
        public static bool AddAccent([NotNull] string name, [NotNull] ResourceDictionary resourceDictionary)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (resourceDictionary == null)
            {
                throw new ArgumentNullException(nameof(resourceDictionary));
            }

            var accentExists = GetAccent(name) != null;
            if (accentExists)
            {
                return false;
            }

            _accents.Add(new Accent(name, resourceDictionary));
            return true;
        }

        /// <summary>
        /// Adds an app theme with the given name.
        /// </summary>
        /// <returns>true if the app theme does not exists and can be added.</returns>
        public static bool AddAppTheme([NotNull] string name, [NotNull] Uri resourceAddress)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (resourceAddress == null)
            {
                throw new ArgumentNullException(nameof(resourceAddress));
            }

            var appThemeExists = GetAppTheme(name) != null;
            if (appThemeExists)
            {
                return false;
            }

            _appThemes.Add(new AppTheme(name, resourceAddress));
            return true;
        }

        /// <summary>
        /// Adds an app theme with the given name.
        /// </summary>
        /// <param name="name">The name of the new AppTheme.</param>
        /// <param name="resourceDictionary">The ResourceDictionary of the accent.</param>
        /// <returns>true if the app theme does not exists and can be added.</returns>
        public static bool AddAppTheme([NotNull] string name, [NotNull] ResourceDictionary resourceDictionary)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (resourceDictionary == null)
            {
                throw new ArgumentNullException(nameof(resourceDictionary));
            }

            var appThemeExists = GetAppTheme(name) != null;
            if (appThemeExists)
            {
                return false;
            }

            _appThemes.Add(new AppTheme(name, resourceDictionary));
            return true;
        }

        /// <summary>
        /// Gets app theme with the given resource dictionary.
        /// </summary>
        /// <param name="resources"><see cref="ResourceDictionary"/> from which the theme should be retrieved.</param>
        /// <returns>AppTheme</returns>
        public static AppTheme GetAppTheme([NotNull] ResourceDictionary resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            return AppThemes.FirstOrDefault(x => AreResourceDictionarySourcesEqual(x.Resources, resources));
        }

        /// <summary>
        /// Gets app theme with the given name and theme type (light or dark).
        /// </summary>
        /// <returns>AppTheme</returns>
        public static AppTheme GetAppTheme([NotNull] string appThemeName)
        {
            if (appThemeName == null)
            {
                throw new ArgumentNullException(nameof(appThemeName));
            }

            return AppThemes.FirstOrDefault(x => x.Name.Equals(appThemeName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the inverse <see cref="AppTheme" /> of the given <see cref="AppTheme"/>.
        /// This method relies on the "Dark" or "Light" affix to be present.
        /// </summary>
        /// <param name="appTheme">The app theme.</param>
        /// <returns>The inverse <see cref="AppTheme"/> or <c>null</c> if it couldn't be found.</returns>
        /// <remarks>
        /// Returns BaseLight, if BaseDark is given or vice versa.
        /// Custom Themes must end with "Dark" or "Light" for this to work, for example "CustomDark" and "CustomLight".
        /// </remarks>
        public static AppTheme GetInverseAppTheme([NotNull] AppTheme appTheme)
        {
            if (appTheme == null)
            {
                throw new ArgumentNullException(nameof(appTheme));
            }

            if (appTheme.Name.EndsWith("dark", StringComparison.OrdinalIgnoreCase))
            {
                return GetAppTheme(appTheme.Name.ToLower().Replace("dark", string.Empty) + "light");
            }

            if (appTheme.Name.EndsWith("light", StringComparison.OrdinalIgnoreCase))
            {
                return GetAppTheme(appTheme.Name.ToLower().Replace("light", string.Empty) + "dark");
            }

            return null;
        }

        /// <summary>
        /// Gets the <see cref="Accent"/> with the given name.
        /// </summary>
        /// <returns>The <see cref="Accent"/> or <c>null</c>, if the app theme wasn't found</returns>
        public static Accent GetAccent([NotNull] string accentName)
        {
            if (accentName == null)
            {
                throw new ArgumentNullException(nameof(accentName));
            }

            return Accents.FirstOrDefault(x => x.Name.Equals(accentName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the <see cref="Accent"/> with the given resource dictionary.
        /// </summary>
        /// <param name="resources"><see cref="ResourceDictionary"/> from which the accent should be retrieved.</param>
        /// <returns>The <see cref="Accent"/> or <c>null</c>, if the accent wasn't found.</returns>
        public static Accent GetAccent([NotNull] ResourceDictionary resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            var builtInAccent = Accents.FirstOrDefault(x => AreResourceDictionarySourcesEqual(x.Resources, resources));
            if (builtInAccent != null)
            {
                return builtInAccent;
            }

            // support dynamically created runtime resource dictionaries
            if (resources.Source == null)
            {
                if (IsAccentDictionary(resources))
                {
                    return new Accent
                    {
                        Name = "Runtime accent",
                        Resources = resources,
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether the specified resource dictionary represents an <see cref="Accent"/>.
        /// <para />
        /// This might include runtime accents which do not have a resource uri.
        /// </summary>
        /// <param name="resources">The resources.</param>
        /// <returns><c>true</c> if the resource dictionary is an <see cref="Accent"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">resources</exception>
        public static bool IsAccentDictionary([NotNull] ResourceDictionary resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            // Note: add more checks if these keys aren't sufficient
            var styleKeys = new List<string>(new[]
                                             {
                                                 "Fluent.Ribbon.Colors.HighlightColor",
                                                 "Fluent.Ribbon.Colors.AccentBaseColor",
                                                 "Fluent.Ribbon.Colors.AccentColor",
                                                 "Fluent.Ribbon.Colors.AccentColor20",
                                                 "Fluent.Ribbon.Colors.AccentColor40",
                                                 "Fluent.Ribbon.Colors.AccentColor60",
                                                 "Fluent.Ribbon.Colors.AccentColor80",
                                                 "Fluent.Ribbon.Brushes.HighlightBrush",
                                                 "Fluent.Ribbon.Brushes.AccentBaseColorBrush",
                                                 "Fluent.Ribbon.Brushes.AccentColorBrush",
                                                 "Fluent.Ribbon.Brushes.AccentColorBrush20",
                                                 "Fluent.Ribbon.Brushes.AccentColorBrush40",
                                                 "Fluent.Ribbon.Brushes.AccentColorBrush60",
                                                 "Fluent.Ribbon.Brushes.AccentColorBrush80",
                                             });

            foreach (var styleKey in styleKeys)
            {
                // Note: do not use contains, because that will look in all merged dictionaries as well. We need to check
                // out the actual keys of the current resource dictionary
                if (!(from object resourceKey in resources.Keys
                     select resourceKey as string).Any(keyAsString => string.Equals(keyAsString, styleKey)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a resource from the detected AppStyle.
        /// </summary>
        /// <param name="window">The window to check. If this is null, the Application's sources will be checked.</param>
        /// <param name="key">The key to check against.</param>
        /// <returns>The resource object or null, if the resource wasn't found.</returns>
        public static object GetResourceFromAppStyle(Window window, string key)
        {
            var appStyle = window != null ? DetectAppStyle(window) : DetectAppStyle(Application.Current);
            if (appStyle == null && window != null)
            {
                appStyle = DetectAppStyle(Application.Current); //no resources in the window's resources.
            }

            if (appStyle == null)
            {
                // nothing to do here, we can't found an app style (make sure all custom themes are added!)
                return null;
            }

            object resource = appStyle.Item1.Resources[key]; //check the theme first

            //next check the accent
            var accentResource = appStyle.Item2.Resources[key];
            if (accentResource != null)
            {
                return accentResource;
            }

            return resource;
        }

        /// <summary>
        /// Change the theme for the whole application.
        /// </summary>
        [SecurityCritical]
        public static void ChangeAppTheme([NotNull] Application app, [NotNull] string themeName)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (themeName == null)
            {
                throw new ArgumentNullException(nameof(themeName));
            }

            var oldTheme = DetectAppStyle(app);
            AppTheme matched;
            if ((matched = GetAppTheme(themeName)) != null)
            {
                ChangeAppStyle(app.Resources, oldTheme, oldTheme.Item2, matched);
            }
        }

        /// <summary>
        /// Change theme for the given window.
        /// </summary>
        [SecurityCritical]
        public static void ChangeAppTheme([NotNull] Window window, [NotNull] string themeName)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (themeName == null)
            {
                throw new ArgumentNullException(nameof(themeName));
            }

            var oldTheme = DetectAppStyle(window);
            AppTheme matched;
            if ((matched = GetAppTheme(themeName)) != null)
            {
                ChangeAppStyle(window.Resources, oldTheme, oldTheme.Item2, matched);
            }
        }

        /// <summary>
        /// Change accent and theme for the whole application.
        /// </summary>
        /// <param name="app">The instance of Application to change.</param>
        /// <param name="newAccent">The accent to apply.</param>
        /// <param name="newTheme">The theme to apply.</param>
        [SecurityCritical]
        public static void ChangeAppStyle([NotNull] Application app, [NotNull] Accent newAccent, [NotNull] AppTheme newTheme)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (newAccent == null)
            {
                throw new ArgumentNullException(nameof(newAccent));
            }

            if (newTheme == null)
            {
                throw new ArgumentNullException(nameof(newTheme));
            }

            var oldTheme = DetectAppStyle(app);
            ChangeAppStyle(app.Resources, oldTheme, newAccent, newTheme);
        }

        /// <summary>
        /// Change accent and theme for the given window.
        /// </summary>
        /// <param name="window">The Window to change.</param>
        /// <param name="newAccent">The accent to apply.</param>
        /// <param name="newTheme">The theme to apply.</param>
        [SecurityCritical]
        public static void ChangeAppStyle([NotNull] Window window, [NotNull] Accent newAccent, [NotNull] AppTheme newTheme)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (newAccent == null)
            {
                throw new ArgumentNullException(nameof(newAccent));
            }

            if (newTheme == null)
            {
                throw new ArgumentNullException(nameof(newTheme));
            }

            var oldTheme = DetectAppStyle(window);
            ChangeAppStyle(window.Resources, oldTheme, newAccent, newTheme);
        }

        [SecurityCritical]
        private static void ChangeAppStyle(ResourceDictionary resources, Tuple<AppTheme, Accent> oldThemeInfo, Accent newAccent, AppTheme newTheme)
        {
            var themeChanged = false;
            if (oldThemeInfo != null)
            {
                var oldAccent = oldThemeInfo.Item2;
                if (oldAccent != null && oldAccent.Name != newAccent.Name)
                {
                    var oldAccentResource = resources.MergedDictionaries.FirstOrDefault(d => AreResourceDictionarySourcesEqual(d, oldAccent.Resources));

                    resources.MergedDictionaries.Add(newAccent.Resources);

                    if (oldAccentResource != null)
                    {
                        resources.MergedDictionaries.Remove(oldAccentResource);
                    }

                    themeChanged = true;
                }

                var oldTheme = oldThemeInfo.Item1;
                if (oldTheme != null && oldTheme != newTheme)
                {
                    var oldThemeResource = resources.MergedDictionaries.FirstOrDefault(d => AreResourceDictionarySourcesEqual(d, oldTheme.Resources));

                    resources.MergedDictionaries.Add(newTheme.Resources);

                    if (oldThemeResource != null)
                    {
                        resources.MergedDictionaries.Remove(oldThemeResource);
                    }

                    themeChanged = true;
                }
            }
            else
            {
                ChangeAppStyle(resources, newAccent, newTheme);

                themeChanged = true;
            }

            if (themeChanged)
            {
                OnThemeChanged(newAccent, newTheme);
            }
        }

        /// <summary>
        /// Changes the accent and theme of a ResourceDictionary directly.
        /// </summary>
        /// <param name="resources">The ResourceDictionary to modify.</param>
        /// <param name="newAccent">The accent to apply to the ResourceDictionary.</param>
        /// <param name="newTheme">The theme to apply to the ResourceDictionary.</param>
        [SecurityCritical]
        public static void ChangeAppStyle([NotNull] ResourceDictionary resources, [NotNull] Accent newAccent, [NotNull] AppTheme newTheme)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            if (newAccent == null)
            {
                throw new ArgumentNullException(nameof(newAccent));
            }

            if (newTheme == null)
            {
                throw new ArgumentNullException(nameof(newTheme));
            }

            ApplyResourceDictionary(newAccent.Resources, resources);
            ApplyResourceDictionary(newTheme.Resources, resources);
        }

        [SecurityCritical]
        private static void ApplyResourceDictionary(ResourceDictionary newRd, ResourceDictionary oldRd)
        {
            oldRd.BeginInit();

            foreach (DictionaryEntry r in newRd)
            {
                if (oldRd.Contains(r.Key))
                {
                    oldRd.Remove(r.Key);
                }

                oldRd.Add(r.Key, r.Value);
            }

            oldRd.EndInit();
        }

        /// <summary>
        /// Copies all resource keys from one resource to another.
        /// </summary>
        /// <param name="fromRD">The source resource dictionary.</param>
        /// <param name="toRD">The destination resource dictionary.</param>
        /// <exception cref="System.ArgumentNullException">
        /// fromRD
        /// or
        /// toRD
        /// </exception>
        internal static void CopyResource([NotNull] ResourceDictionary fromRD, [NotNull] ResourceDictionary toRD)
        {
            if (fromRD == null)
            {
                throw new ArgumentNullException(nameof(fromRD));
            }

            if (toRD == null)
            {
                throw new ArgumentNullException(nameof(toRD));
            }

            ApplyResourceDictionary(fromRD, toRD);
            foreach (var rd in fromRD.MergedDictionaries)
            {
                CopyResource(rd, toRD);
            }
        }

        /// <summary>
        /// Scans the window resources and returns it's accent and theme.
        /// </summary>
        public static Tuple<AppTheme, Accent> DetectAppStyle()
        {
            try
            {
                var style = DetectAppStyle(Application.Current.MainWindow);

                if (style != null)
                {
                    return style;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Failed to detect app style on main window.{Environment.NewLine}{ex}");
            }

            return DetectAppStyle(Application.Current);
        }

        /// <summary>
        /// Scans the window resources and returns it's accent and theme.
        /// </summary>
        /// <param name="window">The Window to scan.</param>
        public static Tuple<AppTheme, Accent> DetectAppStyle([NotNull] Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            var detectedStyle = DetectAppStyle(window.Resources);
            if (detectedStyle == null)
            {
                detectedStyle = DetectAppStyle(Application.Current.Resources);
            }

            return detectedStyle;
        }

        /// <summary>
        /// Scans the application resources and returns it's accent and theme.
        /// </summary>
        /// <param name="app">The Application instance to scan.</param>
        public static Tuple<AppTheme, Accent> DetectAppStyle([NotNull] Application app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return DetectAppStyle(app.Resources);
        }

        /// <summary>
        /// Scans a resources and returns it's accent and theme.
        /// </summary>
        /// <param name="resources">The ResourceDictionary to check.</param>
        private static Tuple<AppTheme, Accent> DetectAppStyle([NotNull] ResourceDictionary resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            AppTheme currentTheme = null;
            Tuple<AppTheme, Accent> detectedAccentTheme = null;

            if (DetectThemeFromResources(ref currentTheme, resources))
            {
                if (GetThemeFromResources(currentTheme, resources, ref detectedAccentTheme))
                {
                    return new Tuple<AppTheme, Accent>(detectedAccentTheme.Item1, detectedAccentTheme.Item2);
                }
            }

            return null;
        }

        internal static bool DetectThemeFromAppResources(out AppTheme detectedTheme)
        {
            detectedTheme = null;

            return DetectThemeFromResources(ref detectedTheme, Application.Current.Resources);
        }

        private static bool DetectThemeFromResources(ref AppTheme detectedTheme, ResourceDictionary dict)
        {
            var enumerator = dict.MergedDictionaries.Reverse().GetEnumerator();
            while (enumerator.MoveNext())
            {
                var currentRd = enumerator.Current;

                AppTheme matched;
                if ((matched = GetAppTheme(currentRd)) != null)
                {
                    detectedTheme = matched;
                    enumerator.Dispose();
                    return true;
                }

                if (DetectThemeFromResources(ref detectedTheme, currentRd))
                {
                    return true;
                }
            }

            enumerator.Dispose();
            return false;
        }

        internal static bool GetThemeFromResources(AppTheme presetTheme, ResourceDictionary dict, ref Tuple<AppTheme, Accent> detectedAccentTheme)
        {
            AppTheme currentTheme = presetTheme;

            Accent matched;
            if ((matched = GetAccent(dict)) != null)
            {
                detectedAccentTheme = Tuple.Create(currentTheme, matched);
                return true;
            }

            foreach (ResourceDictionary rd in dict.MergedDictionaries.Reverse())
            {
                if (GetThemeFromResources(presetTheme, rd, ref detectedAccentTheme))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This event fires if accent color and theme was changed
        /// this should be using the weak event pattern, but for now it's enough
        /// </summary>
        public static event EventHandler<OnThemeChangedEventArgs> IsThemeChanged;

        /// <summary>
        /// Invalidates global colors and resources.
        /// Sometimes the ContextMenu is not changing the colors, so this will fix it.
        /// </summary>
        [SecurityCritical]
        private static void OnThemeChanged(Accent newAccent, AppTheme newTheme)
        {
            IsThemeChanged?.Invoke(Application.Current, new OnThemeChangedEventArgs(newTheme, newAccent));
        }

        private static bool AreResourceDictionarySourcesEqual(ResourceDictionary first, ResourceDictionary second)
        {
            if (first == null
                || second == null)
            {
                return false;
            }

            if (first.Source == null
                || second.Source == null)
            {
                try
                {
                    foreach (var key in first.Keys)
                    {
                        var isTheSame = second.Contains(key)
                                        && Equals(first[key], second[key]);
                        if (!isTheSame)
                        {
                            return false;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Trace.TraceError($"Could not compare resource dictionaries: {exception} {Environment.NewLine} {exception.StackTrace}");
                    return false;
                }

                return true;
            }

            return Uri.Compare(first.Source, second.Source, UriComponents.Host | UriComponents.Path, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0;
        }

        #region WindowsAppModeSetting

        /// <summary>
        /// Synchronizes the current <see cref="AppTheme"/> with the "app mode" setting from windows.
        /// </summary>
        public static void SyncAppThemeWithWindowsAppModeSetting()
        {
            var appTheme = AppsUseLightTheme()
                               ? AppThemeLight
                               : AppThemeDark;

            ChangeAppTheme(Application.Current, appTheme);
        }

        private static bool isAutomaticWindowsAppModeSettingSyncEnabled;

        /// <summary>
        /// Gets or sets wether changes to the "app mode" setting from windows should be detected at runtime and the current <see cref="AppTheme"/> be changed accordingly.
        /// </summary>
        public static bool IsAutomaticWindowsAppModeSettingSyncEnabled
        {
            get { return isAutomaticWindowsAppModeSettingSyncEnabled; }

            set
            {
                if (value == isAutomaticWindowsAppModeSettingSyncEnabled)
                {
                    return;
                }

                isAutomaticWindowsAppModeSettingSyncEnabled = value;

                if (isAutomaticWindowsAppModeSettingSyncEnabled)
                {
                    SystemEvents.UserPreferenceChanged += HandleUserPreferenceChanged;
                }
                else
                {
                    SystemEvents.UserPreferenceChanged -= HandleUserPreferenceChanged;
                }
            }
        }

        private static void HandleUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                SyncAppThemeWithWindowsAppModeSetting();
            }
        }

        private static bool AppsUseLightTheme()
        {
            try
            {
                return Convert.ToBoolean(Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", true));
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
            }

            return true;
        }

        #endregion WindowsAppModeSetting
    }

    /// <summary>
    /// Class which is used as argument for an event to signal theme changes.
    /// </summary>
    public class OnThemeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public OnThemeChangedEventArgs(AppTheme appTheme, Accent accent)
        {
            this.AppTheme = appTheme;
            this.Accent = accent;
        }

        /// <summary>
        /// The new theme.
        /// </summary>
        public AppTheme AppTheme { get; set; }

        /// <summary>
        /// The new accent
        /// </summary>
        public Accent Accent { get; set; }
    }
}