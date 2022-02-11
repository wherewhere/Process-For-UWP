﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AAPTForNet.Models
{
    public class ApkInfo
    {
        public string AppName { get; set; }
        public string PackageName { get; set; }
        public string VersionName { get; set; }
        public string VersionCode { get; set; }
        /// <summary>
        /// Absolute path to apk file
        /// </summary>
        public string FullPath { get; set; }
        public Icon Icon { get; set; }
        public SDKInfo MinSDK { get; set; }
        public SDKInfo TargetSDK { get; set; }
        public List<string> Permissions { get; set; }
        /// <summary>
        /// Supported application binary interfaces
        /// </summary>
        public List<string> SupportedABIs { get; set; }
        public List<string> SupportScreens { get; set; }
        /// <summary>
        /// Size of package, in bytes
        /// </summary>
        public long PackageSize
        {
            get
            {
                try
                {
                    return new FileInfo(FullPath).Length;
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Determines whether this package is filled or not
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return AppName == string.Empty && PackageName == string.Empty;
            }
        }

        public ApkInfo()
        {
            AppName = string.Empty;
            PackageName = string.Empty;
            VersionName = string.Empty;
            VersionCode = string.Empty;
            FullPath = string.Empty;
            Icon = Icon.Default;
            MinSDK = SDKInfo.Unknown;
            TargetSDK = SDKInfo.Unknown;
            Permissions = new List<string>();
            SupportedABIs = new List<string>();
            SupportScreens = new List<string>();
        }

        public ApkInfo megre(params ApkInfo[] apks)
        {
            if (apks.Any(a => a == null))
                throw new ArgumentNullException();

            return ApkInfo.Merge(this, apks);
        }

        public static ApkInfo Merge(IEnumerable<ApkInfo> apks)
        {
            return ApkInfo.Merge(null, apks);
        }

        public static ApkInfo Merge(ApkInfo init, IEnumerable<ApkInfo> apks)
        {
            if (init == null)
                init = new ApkInfo();

            var appApk = apks.FirstOrDefault(a => a.AppName.Length > 0);
            if (appApk != null)
                init.AppName = appApk.AppName;

            var pckApk = apks.FirstOrDefault(a => a.PackageName.Length > 0);
            if (pckApk != null)
            {
                init.VersionName = pckApk.VersionName;
                init.VersionCode = pckApk.VersionCode;
                init.PackageName = pckApk.PackageName;
            }

            var sdkApk = apks.FirstOrDefault(a => !SDKInfo.Unknown.Equals(a.MinSDK));
            if (sdkApk != null)
            {
                init.MinSDK = sdkApk.MinSDK;
                init.TargetSDK = sdkApk.TargetSDK;
            }

            var perApk = apks.FirstOrDefault(a => a.Permissions.Count > 0);
            if (perApk != null)
                init.Permissions = perApk.Permissions;

            var abiApk = apks.FirstOrDefault(a => a.SupportedABIs.Count > 0);
            if (abiApk != null)
                init.SupportedABIs = abiApk.SupportedABIs;

            var scrApk = apks.FirstOrDefault(a => a.SupportScreens.Count > 0);
            if (scrApk != null)
                init.SupportScreens = scrApk.SupportScreens;

            var iconApk = apks.FirstOrDefault(a => !Icon.Default.Equals(a.Icon));
            if (iconApk != null)
                init.Icon = iconApk.Icon;

            var pathApk = apks.FirstOrDefault(a => a.FullPath.Length > 0);
            if (pathApk != null)
                init.FullPath = pathApk.FullPath;

            return init;
        }
    }
}
