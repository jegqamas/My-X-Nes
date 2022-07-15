/*********************************************************************\
*This file is part of My X Nes                                        *
*A Nintendo Entertainment System Emulator.                            *
*                                                                     *
*Copyright (C) 2010 Ala Hadid                                         *
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MyNes.Nes
{
    /// <summary>
    /// Class represents the nes cartridge
    /// </summary>
    public class Cartridge
    {
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public string RomPath;
        public string SRAMFileName;
        /*ADD YOUR NEW MAPPER # HERE*/
        public static int[] SupportedMappers = 
        {
            0,  1,  2,  3,  4,  6,  7,  8,  9,  10,  11,  13,  15, 
            16, 17, 18, 19, 21, 22, 23, 24, 25, 26,  32,  33,  34, 
            41, 48, 61, 64, 65, 66, 69, 71, 73, 75,  78,  79,  80, 
            82, 85, 90, 91, 92, 93, 94, 95, 97, 112, 113, 114, 212,
            225,255
        };
        /// <summary>
        /// The prg pages
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public byte[][] PRG;
        /// <summary>
        /// The chr pages
        /// </summary>
        public byte[][] CHR;
        /// <summary>
        /// PRG pages count
        /// </summary>
        public byte PRG_PAGES;
        /// <summary>
        /// CHR pages count
        /// </summary>
        public byte CHR_PAGES;
        /// <summary>
        /// The mapper numper
        /// </summary>
        public byte MAPPER;
        /// <summary>
        /// The VRAM mirroring base, used for One_Screen mirroring
        /// </summary>
        public ushort MirroringBase = 0x2000;
        /// <summary>
        /// The mirroring
        /// </summary>
        public Mirroring Mirroring = Mirroring.Vertical;
        /// <summary>
        /// Is 512-byte trainer/patch at 7000h-71FFh
        /// </summary>
        public bool IsTrainer = false;
        /// <summary>
        /// Is Battery-backed SRAM at 6000h-7FFFh, set only if battery-backed
        /// </summary>
        public bool IsBatteryBacked = false;
        /// <summary>
        /// True=no chr found at the cart, False=chrs loaded
        /// </summary>
        public bool IsVRAM = false;
        /// <summary>
        /// If this rom is pal or ntsc
        /// </summary>
        public bool IsPAL = false;
        public bool StateLoaded = false;
        public bool SupportedMapper()
        {
            for (int i = 0; i < SupportedMappers.Length; i++)
            {
                if (SupportedMappers[i] == MAPPER)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Load a carttidage file
        /// </summary>
        /// <param name="FileName">The complete rom path</param>
        /// <param name="HeaderOnly">True=load header only, false=load the prg, chr and trainer</param>
        /// <returns>The status of the load operation</returns>
        public LoadRomStatus Load(string FileName, bool HeaderOnly)
        {
            try
            {

                //Create our stream
                Stream STR = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                //Read the header
                byte[] header = new byte[16];
                STR.Read(header, 0, 16);
                //Check out
                if (header[0] != 0x4E | header[1] != 0x45 |
                    header[2] != 0x53 | header[3] != 0x1A)
                {
                    STR.Close();
                    return LoadRomStatus.NotINES;
                }
                //Flags
                PRG_PAGES = header[4];
                CHR_PAGES = header[5];
                if ((header[6] & 0x1) != 0x0)
                    this.Mirroring = Nes.Mirroring.Vertical;
                else
                    this.Mirroring = Nes.Mirroring.Horizontal;
                IsBatteryBacked = (header[6] & 0x2) != 0x0;
                IsTrainer = (header[6] & 0x4) != 0x0;
                if ((header[6] & 0x8) != 0x0)
                    this.Mirroring = Nes.Mirroring.Four_Screen;
                if ((header[7] & 0x0F) == 0)
                    MAPPER = (byte)((header[7] & 0xF0) | (header[6] & 0xF0) >> 4);
                else
                    MAPPER = (byte)((header[6] & 0xF0) >> 4);


                IsPAL = CheckForPal(FileName);

                if (!SupportedMapper())
                {
                    STR.Close();
                    return LoadRomStatus.UnsupportedMapper;
                }
                //Load the cart pages
                if (!HeaderOnly)
                {
                }
                //Finish
                STR.Close();
                return LoadRomStatus.LoadSuccessed;
            }
            catch
            {
            }
            return LoadRomStatus.LoadFaild;
        }
        /// <summary>
        /// Load a carttidage file
        /// </summary>
        /// <param name="FileName">The complete rom path</param>
        /// <returns>True=loaded successful, false=loaded faild</returns>
        public LoadRomStatus Load(string FileName)
        { return Load(FileName, false); }
        bool CheckForPal(string rompath)
        {
            if (rompath.Length >= 3)
            {
                for (int i = 0; i < rompath.Length - 3; i++)
                {
                    if (rompath.Substring(i, 3).ToLower() == "(e)")
                        return true;
                }
            }
            return false;
        }
    }
    /// <summary>
    /// The status of the load rom operation
    /// </summary>
    public enum LoadRomStatus
    {
        /// <summary>
        /// This rom is not INES format
        /// </summary>
        NotINES,
        /// <summary>
        /// Unsupported mapper
        /// </summary>
        UnsupportedMapper,
        /// <summary>
        /// Load Faild
        /// </summary>
        LoadFaild,
        /// <summary>
        /// Load Successed
        /// </summary>
        LoadSuccessed
    }
    public enum Mirroring
    {
        Vertical, Horizontal, One_Screen, Four_Screen
    }
}
