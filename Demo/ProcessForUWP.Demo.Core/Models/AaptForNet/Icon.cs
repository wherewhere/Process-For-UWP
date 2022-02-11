using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace AAPTForNet.Models
{
    public class Icon
    {

        private const int hdpiWidth = 72;
        public const string DefaultName = "ic_launcher.png";

        public static readonly Icon Default = new(DefaultName);

        /// <summary>
        /// Return absolute path to package icon if @isImage is true,
        /// otherwise return empty string
        /// </summary>
        public string RealPath { get; set; }

        /// <summary>
        /// Determines whether icon of package is an image
        /// </summary>
        public bool isImage => !DefaultName.Equals(IconName) && !isMarkup;

        public bool isMarkup => this.IconName
            .EndsWith(".xml", StringComparison.OrdinalIgnoreCase);

        // Not real icon, it refer to another
        public bool isRefernce => this.IconName.StartsWith("0x");

        byte[] binary = null;
        public byte[] Binary
        {
            get
            {
                if(binary != null)
                {
                    return binary;
                }

                if (!isImage || !File.Exists(RealPath))
                {
                    return Array.Empty<byte>();
                }
                using Stream s = File.OpenRead(RealPath);
                using MemoryStream ms = new();
                s.CopyTo(ms);
                return ms.ToArray();
            }
            set
            {
                binary = value;
            }
        }

        public bool isHighDensity
        {
            get
            {
                if (!isImage || !File.Exists(RealPath))
                {
                    return false;
                }

                try
                {
                    // Load from unsupported format will throw an exception.
                    // But icon can be packed without extension
                    using Bitmap image = new(RealPath);
                    return image.Width > hdpiWidth;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Icon name can be an asset image (real icon image),
        /// markup file (actually it's image, but packed to xml)
        /// or reference to another
        /// </summary>
        public string IconName { get; set; }

        public Icon(string iconName)
        {
            this.IconName = iconName;
            this.RealPath = string.Empty;
        }

        public override string ToString() => this.IconName;

        public override bool Equals(object obj)
        {
            if (obj is Icon ic)
            {
                return this.IconName == ic.IconName;
            }
            return false;
        }

        public override int GetHashCode() => -489061483 + EqualityComparer<string>.Default.GetHashCode(this.IconName);
    }
}
