﻿using System.Collections.Generic;

namespace AAPTForNet.Models
{
    public class SDKInfo
    {
        public static readonly SDKInfo Unknown = new("0", "0", "0");

        // https://source.android.com/setup/start/build-numbers
        private static readonly string[] AndroidCodeNames = {
            "Unknown",
            "Unnamed",  // API level 1
            "Petit Four",
            "Cupcake",
            "Donut",
            "Eclair",
            "Eclair",
            "Eclair",
            "Froyo",
            "Gingerbread",
            "Gingerbread",
            "Honeycomb",
            "Honeycomb",
            "Honeycomb",
            "Ice Cream Sandwich",
            "Ice Cream Sandwich",
            "Jelly Bean",
            "Jelly Bean",
            "Jelly Bean",
            "KitKat",
            "KitKat, with wearable extensions",  // API level 20
            "Lollipop",
            "Lollipop",
            "Marshmallow",
            "Nougat",
            "Nougat",
            "Oreo",
            "Oreo",
            "Pie",
            "Q",
            "R",  // API level 30
            "S",
        };

        private static readonly string[] AndroidVersionCodes = {
            "Unknown",
            "1.0",  // API level 1
            "1.1",
            "1.5",
            "1.6",
            "2.0",
            "2.0",
            "2.1",
            "2.2",
            "2.3",
            "2.3",
            "3.0",
            "3.1",
            "3.2",
            "4.0",
            "4.0",
            "4.1",
            "4.2",
            "4.3",
            "4.4",
            "4.4 watch",  // API level 20
            "5.0",
            "5.1",
            "6.0",
            "7.0",
            "7.1",
            "8.0",
            "8.1",
            "9",
            "10",
            "11",    // API level 30
            "12"
        };

        public string APILever { get; }
        public string Version { get; }
        public string CodeName { get; }

        protected SDKInfo(string level, string ver, string code)
        {
            this.APILever = level;
            this.Version = ver;
            this.CodeName = code;
        }

        public static SDKInfo GetInfo(int sdkVer)
        {
            int index = (sdkVer < 1 || sdkVer > AndroidCodeNames.Length - 1) ? 0 : sdkVer;

            return new SDKInfo(sdkVer.ToString(),
                AndroidVersionCodes[index], AndroidCodeNames[index]);
        }

        public static SDKInfo GetInfo(string sdkVer)
        {
            int.TryParse(sdkVer, out int ver);
            return GetInfo(ver);
        }

        public override int GetHashCode() => 1008763889 + EqualityComparer<string>.Default.GetHashCode(this.APILever);

        public override bool Equals(object obj)
        {
            if (obj is SDKInfo another)
            {
                return this.APILever == another.APILever;
            }
            return false;
        }

        public override string ToString()
        {
            if (APILever.Equals("0") && Version.Equals("0") && CodeName.Equals("0"))
                return AndroidCodeNames[0];

            return $"API Level {this.APILever} " +
                $"{(this.Version == AndroidCodeNames[0] ? $"({AndroidCodeNames[0]} - " : $"(Android {this.Version} - ")}" +
                $"{this.CodeName})";
        }
    }
}
