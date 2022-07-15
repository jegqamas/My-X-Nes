/*********************************************************************\
*This file is part of My X Nes                                        *
*A Nintendo Entertainment System Emulator.                            *
*                                                                     *
*Copyright (C) 2010 - 2011 Ala Hadid                                  *
*E-mail: mailto:ahdsoftwares@hotmail.com                              *
*                                                                     *
*My X Nes is free software: you can redistribute it and/or modify     *
*it under the terms of the GNU General Public License as published by *
*the Free Software Foundation, either version 3 of the License, or    *
*(at your option) any later version.                                  *
*                                                                     *
*My X Nes is distributed in the hope that it will be useful,          *
*but WITHOUT ANY WARRANTY; without even the implied warranty of       *
*MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
*GNU General Public License for more details.                         *
*                                                                     *
*You should have received a copy of the GNU General Public License    *
*along with My X Nes.  If not, see <http://www.gnu.org/licenses/>.    *
\*********************************************************************/
using System;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;

namespace MyXNes
{
#if WINDOWS || XBOX
    static class Program
    {
        static Core _Core;
        static Settings _Settings;
        static string _Version = "";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            _Settings = (Settings)LoadSerialized("Settings.mxn", "Settings",
                typeof(Settings));
            if (_Settings == null)
                _Settings = new Settings();

            Assembly asm = Assembly.LoadFrom(Path.GetFullPath("Nes.dll"));
            Version version = asm.GetName().Version;

            _Version = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString()
            +"\nNes core version: " + version.ToString();

            using (_Core = new Core())
            {
                _Core.Run();
            }
        }
        public static Core CORE
        { get { return _Core; } }
        public static Settings Settings
        { get { return _Settings; } }
        /// <summary>
        /// Get the version
        /// </summary>
        public static string Version
        {
            get { return _Version; }
        }
        /// <summary>
        /// Save a file into the save unit
        /// </summary>
        /// <param name="FileName">The file name</param>
        /// <param name="FolderName">The folder name</param>
        /// <param name="ObjectToSerialize">The object you want to serilize</param>
        /// <returns>True if saved successfuly</returns>
        public static bool SaveSerialize(string FileName, string FolderName, object ObjectToSerialize)
        {
            //try
            {
                IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                if (result.IsCompleted)
                {
                    StorageDevice device = StorageDevice.EndShowSelector(result);
                    if (device != null && device.IsConnected)
                    {
                        result = device.BeginOpenContainer(FolderName, null, null);
                        result.AsyncWaitHandle.WaitOne();

                        StorageContainer container = device.EndOpenContainer(result);

                        if (container.FileExists(FileName))
                            // Delete it so that we can create one fresh.
                            container.DeleteFile(FileName);

                        Stream stream = container.CreateFile(FileName);

                        if (stream.Length < device.FreeSpace)
                        {
                            XmlSerializer ser = new XmlSerializer(ObjectToSerialize.GetType());
                            ser.Serialize(stream, ObjectToSerialize);
                            stream.Close();
                            container.Dispose();
                            return true;
                        }
                        else
                        {
                            _Core.WriteText("SAVE FAILED: THERE'S NO ENOUGH SPACE ON THE STORAGE UNIT !!", 120, TextStatus.ERROR);
                            return false;
                        }
                    }
                    else
                    {
                        _Core.WriteText("SAVE FAILED: STORAGE UNIT IS NOT CONNECTED OR DAMAGED !!", 120, TextStatus.ERROR);
                        return false;
                    }
                }
                return false;
            }
            //catch
            {
                //   _Core.WriteText(@"SAVE FAILED: /!\ ERROR /!\", 120);
                //    return false;
            }
        }
        /// <summary>
        /// Loaf a file from the save unit
        /// </summary>
        /// <param name="FileName">The file name</param>
        /// <param name="FolderName">The folder name</param>
        /// <param name="TypeOfSerialized">The type of object you want to deserilize</param>
        /// <returns>object</returns>
        public static object LoadSerialized(string FileName, string FolderName, Type TypeOfSerialized)
        {
            try
            {
                IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                if (result.IsCompleted)
                {
                    StorageDevice device = StorageDevice.EndShowSelector(result);
                    if (device != null && device.IsConnected)
                    {
                        result = device.BeginOpenContainer(FolderName, null, null);
                        result.AsyncWaitHandle.WaitOne();

                        StorageContainer container = device.EndOpenContainer(result);

                        if (container.FileExists(FileName))
                        {

                            Stream stream = container.OpenFile(FileName, FileMode.Open, FileAccess.Read);

                            XmlSerializer ser = new XmlSerializer(TypeOfSerialized);
                            object sero = ser.Deserialize(stream);
                            stream.Close();
                            container.Dispose();
                            return sero;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        _Core.WriteText("READ FAILED: STORAGE UNIT IS NOT CONNECTED OR DAMAGED !!", 120, TextStatus.ERROR);
                        return null;
                    }
                }
                return null;
            }
            catch
            {
                _Core.WriteText(@"READ FAILED: /!\ ERROR /!\", 120, TextStatus.ERROR);
                return null;
            }
        }
        /// <summary>
        /// Open a file in the storage media
        /// </summary>
        /// <param name="FileName">The file name with extension</param>
        /// <param name="FolderName">The folder name</param>
        /// <param name="FileMode">The open file mode, open or create, if open it will be read only otherwise it will be writable</param>
        /// <returns></returns>
        public static Stream OpenFile(string FileName, string FolderName,FileMode mode)
        {
            try
            {
                IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                if (result.IsCompleted)
                {
                    Stream stream = null;
                    StorageDevice device = StorageDevice.EndShowSelector(result);
                    if (device != null && device.IsConnected)
                    {
                        result = device.BeginOpenContainer(FolderName, null, null);
                        result.AsyncWaitHandle.WaitOne();

                        StorageContainer container = device.EndOpenContainer(result);

                        if (mode == FileMode.Open)
                        {
                            if (container.FileExists(FileName))
                            {
                                return stream = container.OpenFile(FileName, FileMode.Open, FileAccess.Read);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return stream = container.OpenFile(FileName, FileMode.Create, FileAccess.Write);
                        }
                    }
                    else
                    {
                        //_Core.WriteText("READ FAILED: STORAGE UNIT IS NOT CONNECTED OR DAMAGED !!", 120, TextStatus.ERROR);
                        return null;
                    }
                }
                return null;
            }
            catch
            {
                _Core.WriteText(@"READ FAILED: /!\ ERROR /!\"+"\nCould not open stream for file:" + FileName, 120, TextStatus.ERROR);
                return null;
            }
        }
    }
#endif
    public enum CurrentRoom
    {
        About, MainMenu, Browser, Options, GamePlay, Loading, SaveManager,
        StartUp, KeysConfiguration
    }
}

