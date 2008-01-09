#region MIT License
/*
 * Copyright (c) 2005-2008 Jonathan Mark Porter. http://physics2d.googlepages.com/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
#endregion

#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet;
using Physics2DDotNet.Collections;
using Tao.OpenGl;
using SdlDotNet;
using SdlDotNet.Graphics;

namespace Graphics2DDotNet
{
    public class SettingsXML
    {
        public string DataDir;
        public string FontDir;
        public string ImageDir;
    }
    public static class Settings
    {
        public const string SettingsFile = "settings.xml";
        public static readonly string DataDir;
        public static readonly string FontDir;
        public static readonly string ImageDir;
        static Settings()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsXML));
            SettingsXML settings;
            if (File.Exists(SettingsFile))
            {
                try
                {
                    settings = LoadSettings(xmlSerializer);
                }
                catch
                {
                    settings = SaveDefault(xmlSerializer);
                }
            }
            else
            {
                settings = SaveDefault(xmlSerializer);
            }
            DataDir = settings.DataDir;
            FontDir = settings.FontDir;
            ImageDir = settings.ImageDir;
        }
        static SettingsXML LoadSettings(XmlSerializer xmlSerializer)
        {
            using (FileStream stream = File.OpenRead(SettingsFile))
            {
                return (SettingsXML)xmlSerializer.Deserialize(stream);
            }
        }
        static SettingsXML SaveDefault(XmlSerializer xmlSerializer)
        {
            SettingsXML settings = new SettingsXML();
            settings.DataDir = @"..|..|..|data".Replace('|', Path.DirectorySeparatorChar);
            settings.FontDir = settings.DataDir;
            settings.ImageDir = settings.DataDir;

            using (FileStream stream = File.Open(SettingsFile, FileMode.Create))
            {
                xmlSerializer.Serialize(stream, settings);
            }
            return settings;
        }
    }
}