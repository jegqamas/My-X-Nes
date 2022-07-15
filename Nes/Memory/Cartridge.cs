/*********************************************************************\
*This file is part of My Nes                                          *
*A Nintendo Entertainment System Emulator.                            *
*                                                                     *
*Copyright © Ala Hadid 2009 - 2011                                    *
*E-mail: mailto:ahdsoftwares@hotmail.com                              *
*                                                                     *
*My Nes is free software: you can redistribute it and/or modify       *
*it under the terms of the GNU General Public License as published by *
*the Free Software Foundation, either version 3 of the License, or    *
*(at your option) any later version.                                  *
*                                                                     *
*My Nes is distributed in the hope that it will be useful,            *
*but WITHOUT ANY WARRANTY; without even the implied warranty of       *
*MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
*GNU General Public License for more details.                         *
*                                                                     *
*You should have received a copy of the GNU General Public License    *
*along with this program.  If not, see <http://www.gnu.org/licenses/>.*
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
        public Cartridge(NES nes)
        {
            this.nes = nes;
        }
        NES nes;

        public string RomPath;
        public string SRAMFileName;
        /*ADD YOUR NEW MAPPER # HERE*/
        public static int[] SupportedMappers = 
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 21, 22, 23, 24, 25, 
            26, 32, 33, 34, 40, 41, 42, 43, 44, 45, 46, 47,48, 49, 50, 51, 57, 58, 60, 61, 62, 64, 65,
            66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 82, 83, 85, 86, 87, 88,
            89, 90, 91, 92, 93, 94, 95, 97, 105, 112, 113, 114, 115, 117, 118, 119, 133, 140, 
            142,159, 180, 182, 184, 185, 187, 188, 189, 193, 200, 201, 202,203,204,205, 212,213, 225, 226, 228, 
            229, 230,  231, 232, 233, 240, 241, 242, 243, 244, 245, 246, 248, 249, 251, 255
        };
        /*MAKE YOUR NEW MAPPER INITIALIZE HERE*/
        void InitializeMapper()
        {
            switch (this.MAPPER)
            {
                case 0: nes.Mapper= new Mapper00(nes.Memory); break;
                case 1: nes.Mapper= new Mapper01(nes.Memory); break;
                case 2: nes.Mapper= new Mapper02(nes.Memory); break;
                case 3: nes.Mapper= new Mapper03(nes.Memory); break;
                case 4: nes.Mapper= new Mapper04(nes.Memory); break;
                case 5: nes.Mapper= new Mapper05(nes.Memory); break;
                case 6: nes.Mapper= new Mapper06(nes.Memory); break;
                case 7: nes.Mapper= new Mapper07(nes.Memory); break;
                case 8: nes.Mapper= new Mapper08(nes.Memory); break;
                case 9: nes.Mapper= new Mapper09(nes.Memory); break;
                case 10: nes.Mapper= new Mapper10(nes.Memory); break;
                case 11: nes.Mapper= new Mapper11(nes.Memory); break;
                case 12: nes.Mapper = new Mapper12(nes.Memory); break;
                case 13: nes.Mapper= new Mapper13(nes.Memory); break;
                case 15: nes.Mapper= new Mapper15(nes.Memory); break;
                case 16: nes.Mapper= new Mapper16(nes.Memory); break;
                case 17: nes.Mapper= new Mapper17(nes.Memory); break;
                case 18: nes.Mapper= new Mapper18(nes.Memory); break;
                case 19: nes.Mapper= new Mapper19(nes.Memory); break;
                case 21: nes.Mapper= new Mapper21(nes.Memory); break;
                case 22: nes.Mapper= new Mapper22(nes.Memory); break;
                case 23: nes.Mapper= new Mapper23(nes.Memory); break;
                case 24: nes.Mapper= new Mapper24(nes.Memory); break;
                case 25: nes.Mapper= new Mapper25(nes.Memory); break;
                case 26: nes.Mapper= new Mapper26(nes.Memory); break;
                case 32: nes.Mapper= new Mapper32(nes.Memory); break;
                case 33: nes.Mapper= new Mapper33(nes.Memory); break;
                case 34: nes.Mapper= new Mapper34(nes.Memory); break;
                case 40: nes.Mapper= new Mapper40(nes.Memory); break;
                case 41: nes.Mapper= new Mapper41(nes.Memory); break;
                case 42: nes.Mapper= new Mapper42(nes.Memory); break;
                case 43: nes.Mapper= new Mapper43(nes.Memory); break;
                case 44: nes.Mapper= new Mapper44(nes.Memory); break;
                case 45: nes.Mapper= new Mapper45(nes.Memory); break;
                case 46: nes.Mapper= new Mapper46(nes.Memory); break;
                case 47: nes.Mapper= new Mapper47(nes.Memory); break;
                case 48: nes.Mapper= new Mapper33(nes.Memory); break;
                case 49: nes.Mapper = new Mapper49(nes.Memory); break;
                case 50: nes.Mapper= new Mapper50(nes.Memory); break;
                case 51: nes.Mapper= new Mapper51(nes.Memory); break;
                case 57: nes.Mapper= new Mapper57(nes.Memory); break;
                case 58: nes.Mapper= new Mapper58(nes.Memory); break;
                case 60: nes.Mapper= new Mapper60(nes.Memory); break;
                case 61: nes.Mapper= new Mapper61(nes.Memory); break;
                case 62: nes.Mapper= new Mapper62(nes.Memory); break;
                case 64: nes.Mapper= new Mapper64(nes.Memory); break;
                case 65: nes.Mapper= new Mapper65(nes.Memory); break;
                case 66: nes.Mapper= new Mapper66(nes.Memory); break;
                case 67: nes.Mapper= new Mapper67(nes.Memory); break;
                case 68: nes.Mapper= new Mapper68(nes.Memory); break;
                case 69: nes.Mapper= new Mapper69(nes.Memory); break;
                case 70: nes.Mapper= new Mapper70(nes.Memory); break;
                case 71: nes.Mapper= new Mapper71(nes.Memory); break;
                case 72: nes.Mapper= new Mapper72(nes.Memory); break;
                case 73: nes.Mapper= new Mapper73(nes.Memory); break;
                case 74: nes.Mapper= new Mapper74(nes.Memory); break;
                case 75: nes.Mapper= new Mapper75(nes.Memory); break;
                case 76: nes.Mapper= new Mapper76(nes.Memory); break;
                case 77: nes.Mapper= new Mapper77(nes.Memory); break;
                case 78: nes.Mapper= new Mapper78(nes.Memory); break;
                case 79: nes.Mapper= new Mapper79(nes.Memory); break;
                case 80: nes.Mapper= new Mapper80(nes.Memory); break;
                case 82: nes.Mapper= new Mapper82(nes.Memory); break;
                case 83: nes.Mapper= new Mapper83(nes.Memory); break;
                case 85: nes.Mapper= new Mapper85(nes.Memory); break;
                case 86: nes.Mapper= new Mapper86(nes.Memory); break;
                case 87: nes.Mapper= new Mapper87(nes.Memory); break;
                case 88: nes.Mapper= new Mapper88(nes.Memory); break;
                case 89: nes.Mapper= new Mapper89(nes.Memory); break;
                case 90: nes.Mapper= new Mapper90(nes.Memory); break;
                case 91: nes.Mapper= new Mapper91(nes.Memory); break;
                case 92: nes.Mapper= new Mapper92(nes.Memory); break;
                case 93: nes.Mapper= new Mapper93(nes.Memory); break;
                case 94: nes.Mapper= new Mapper94(nes.Memory); break;
                case 95: nes.Mapper= new Mapper95(nes.Memory); break;
                case 97: nes.Mapper= new Mapper97(nes.Memory); break;
                case 105: nes.Mapper= new Mapper105(nes.Memory); break;
                case 112: nes.Mapper= new Mapper112(nes.Memory); break;
                case 113: nes.Mapper= new Mapper113(nes.Memory); break;
                case 114: nes.Mapper= new Mapper114(nes.Memory); break;
                case 115: nes.Mapper= new Mapper115(nes.Memory); break;
                case 117: nes.Mapper= new Mapper117(nes.Memory); break;
                case 118: nes.Mapper= new Mapper118(nes.Memory); break;
                case 119: nes.Mapper= new Mapper119(nes.Memory); break;
                case 133: nes.Mapper= new Mapper133(nes.Memory); break;
                case 140: nes.Mapper= new Mapper140(nes.Memory); break;
                case 142: nes.Mapper= new Mapper142(nes.Memory); break;
                case 159: nes.Mapper = new Mapper16(nes.Memory); break;
                case 180: nes.Mapper= new Mapper180(nes.Memory); break;
                case 182: nes.Mapper= new Mapper182(nes.Memory); break;
                case 184: nes.Mapper= new Mapper184(nes.Memory); break;
                case 185: nes.Mapper= new Mapper185(nes.Memory); break;
                case 187: nes.Mapper= new Mapper187(nes.Memory); break;
                case 188: nes.Mapper= new Mapper188(nes.Memory); break;
                case 189: nes.Mapper= new Mapper189(nes.Memory); break;
                case 193: nes.Mapper= new Mapper193(nes.Memory); break;
                case 200: nes.Mapper = new Mapper200(nes.Memory); break;
                case 201: nes.Mapper = new Mapper201(nes.Memory); break;
                case 202: nes.Mapper = new Mapper202(nes.Memory); break;
                case 203: nes.Mapper = new Mapper203(nes.Memory); break;
                case 204: nes.Mapper = new Mapper204(nes.Memory); break;
                case 205: nes.Mapper = new Mapper205(nes.Memory); break;
                case 212: nes.Mapper= new Mapper212(nes.Memory); break;
                case 213: nes.Mapper = new Mapper213(nes.Memory); break;
                case 226: nes.Mapper= new Mapper226(nes.Memory); break;
                case 228: nes.Mapper= new Mapper228(nes.Memory); break;
                case 229: nes.Mapper= new Mapper229(nes.Memory); break;
                case 230: nes.Mapper= new Mapper230(nes.Memory); break;
                case 231: nes.Mapper= new Mapper231(nes.Memory); break;
                case 232: nes.Mapper= new Mapper232(nes.Memory); break;
                case 233: nes.Mapper= new Mapper233(nes.Memory); break;
                case 240: nes.Mapper= new Mapper240(nes.Memory); break;
                case 241: nes.Mapper= new Mapper241(nes.Memory); break;
                case 242: nes.Mapper= new Mapper242(nes.Memory); break;
                case 243: nes.Mapper= new Mapper243(nes.Memory); break;
                case 244: nes.Mapper= new Mapper244(nes.Memory); break;
                case 245: nes.Mapper= new Mapper245(nes.Memory); break;
                case 246: nes.Mapper= new Mapper246(nes.Memory); break;
                case 248: nes.Mapper= new Mapper248(nes.Memory); break;
                case 249: nes.Mapper= new Mapper249(nes.Memory); break;
                case 251: nes.Mapper= new Mapper251(nes.Memory); break;
                case 225:
                case 255: nes.Mapper= new Mapper225_255(nes.Memory); break;
            }
            if (nes.Mapper!= null)
                nes.Mapper.SetUpMapperDefaults();
        }

        /// <summary>
        /// The prg pages
        /// </summary>
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

        ushort mirroringBase = 0x2000;

        Mirroring mirroring = Mirroring.Vertical;
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
        /// If this rom is PC10 game (arcade machine with additional 8K Z80-ROM)
        /// </summary>
        public bool IsPC10 = false;
        /// <summary>
        /// If this rom is VS Unisystem game (arcade machine with different palette)
        /// </summary>
        public bool IsVSUnisystem = false;
        /// <summary>
        /// If this rom is pal or ntsc
        /// </summary>
        public bool IsPAL = false;
        public int PRGSizeInKB = 0;
        public int CHRSizeInKB = 0;
        public int CRC32 = 0;

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
        /// <param name="ROMstream">The stream which should be rom file stream, will be used for file reading</param>
        /// <param name="SRAMstream">The stream which should be SRAM file stream, will be used for file reading</param>
        /// <param name="HeaderOnly">True=load header only, false=load the prg, chr and trainer</param>
        /// <returns>The status of the load operation</returns>
        public LoadRomStatus Load(string FileName, Stream ROMstream, Stream SRAMstream, bool HeaderOnly)
        {
            try
            {
                //Read the header
                byte[] header = new byte[16];
                ROMstream.Read(header, 0, 16);
                //Check out
                if (header[0] != 0x4E | header[1] != 0x45 | header[2] != 0x53 | header[3] != 0x1A)
                {
                    ROMstream.Dispose();
                    return LoadRomStatus.NotINES;
                }
                //Flags
                PRG_PAGES = header[4];
                CHR_PAGES = header[5];
                if ((header[6] & 0x1) == 0)
                    this.mirroring = Nes.Mirroring.Horizontal;
                else
                    this.mirroring = Nes.Mirroring.Vertical;

                if ((header[6] & 0x8) != 0)
                    this.mirroring = Nes.Mirroring.Four_Screen;
                IsBatteryBacked = (header[6] & 0x2) != 0x0;
                IsTrainer = (header[6] & 0x4) != 0x0;

                if ((header[7] & 0x0F) == 0)
                    MAPPER = (byte)((header[7] & 0xF0) | (header[6] & 0xF0) >> 4);
                else
                    MAPPER = (byte)((header[6] & 0xF0) >> 4);

                IsVSUnisystem = ((header[7] & 0x1) == 0x1);
                IsPC10 = ((header[7] & 0x2) == 0x2);

                IsPAL = CheckForPal(FileName);
                if (!SupportedMapper())
                {
                    ROMstream.Dispose();
                    return LoadRomStatus.UnsupportedMapper;
                }
                //Load the cart pages
                if (!HeaderOnly)
                {
                    //calculate crc32
                    /*ROMstream.Position = 0;
                    Crc32 crc = new Crc32();
                    String hash = String.Empty;
                    foreach (byte b in crc.ComputeHash(ROMstream))
                        hash += b.ToString("x2").ToLower();
                    CRC32 = Convert.ToInt32(hash, 16);
                    ROMstream.Position = 16;*/
                    //CONSOLE.WriteLine(this, "CRC32 = "+CRC32.ToString()+" [$"+string.Format("{0:X}",CRC32)+"]", DebugStatus.None);
                    //Trainer
                    if (IsTrainer)
                    {
                        ROMstream.Read(nes.Memory.SRAM[0], 0x1000, 512);
                        CONSOLE.WriteLine(this, "TRAINER !!", DebugStatus.None);
                    }
                    //PRG
                    int prg_roms = PRG_PAGES * 4;
                    PRGSizeInKB = prg_roms * 4;
                    PRG = new byte[prg_roms][];
                    for (int i = 0; i < prg_roms; i++)
                    {
                        PRG[i] = new byte[4096];
                        ROMstream.Read(PRG[i], 0, 4096);
                    }
                    //CHR
                    int chr_roms = CHR_PAGES * 8;
                    CHRSizeInKB = chr_roms;
                    if (CHR_PAGES != 0)
                    {
                        CHR = new byte[chr_roms][];
                        for (int i = 0; i < (chr_roms); i++)
                        {
                            CHR[i] = new byte[1024];
                            ROMstream.Read(CHR[i], 0, 1024);
                        }
                        IsVRAM = false;
                    }
                    else
                    {
                        IsVRAM = true;//Mapper will fill up the chr
                    }
                    CONSOLE.WriteLine(this, "ROM SIZE = " + (ROMstream.Length / 1024) + " KB", DebugStatus.None);
                    CONSOLE.WriteLine(this, "PRG PAGES = " + PRG_PAGES.ToString() + " (" + (prg_roms * 4) + " KB)", DebugStatus.None);
                    CONSOLE.WriteLine(this, "PRG ROMS = " + prg_roms, DebugStatus.None);
                    CONSOLE.WriteLine(this, "CHR PAGES = " + CHR_PAGES.ToString() + " (" + chr_roms + " KB)", DebugStatus.None);
                    CONSOLE.WriteLine(this, "MAPPER # " + MAPPER, DebugStatus.Cool);
                    if (IsPAL)
                        CONSOLE.WriteLine(this, "PAL rom", DebugStatus.Warning);
                    else
                        CONSOLE.WriteLine(this, "NTSC rom", DebugStatus.Warning);

                    //SRAM
                    if (IsBatteryBacked)
                    {
                        CONSOLE.WriteLine(this, "Trying to read SRAM from file : " + SRAMFileName, DebugStatus.None);
                        //if (File.Exists(SRAMFileName))
                        {
                            try
                            {
                                SRAMstream.Read(nes.Memory.SRAM[0], 0, 0x2000);
                            }
                            catch (Exception ex)
                            { CONSOLE.WriteLine(this, "Faild to read SRAM\nex: " + ex.Message, DebugStatus.Error); }
                        }
                     //   else
                        {
                       //     CONSOLE.WriteLine(this, "No SRAM file found for this rom", DebugStatus.Warning);
                        }
                    }
                    RomPath = FileName;
                    InitializeMapper();
                    nes.PPUMemory.ApplyMirroring();
                }
                //Finish
                ROMstream.Dispose();
                return LoadRomStatus.LoadSuccessed;
            }
            catch (Exception Ex)
            {
                CONSOLE.WriteLine(this, "Faild to read cart, ex: " + Ex.Message, DebugStatus.Error);
                CONSOLE.WriteLine(this, "Line " + Ex.StackTrace, DebugStatus.Error);
            }
            return LoadRomStatus.LoadFaild;
        }

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
        public void SaveState(System.IO.Stream stream)
        {
            byte status = 0;
            switch (mirroring)
            {
                case Nes.Mirroring.Four_Screen: status = 0; break;
                case Nes.Mirroring.Horizontal: status = 1; break;
                case Nes.Mirroring.One_Screen: status = 2; break;
                case Nes.Mirroring.Vertical: status = 3; break;
            }
            status |= (byte)((mirroringBase == 0x2400) ? 0x10 : 0x00);
            stream.WriteByte(status);
            //save chr if this rom is VRAM
            if (IsVRAM)
            {
                for (int i = 0; i < CHR.Length; i++)
                {
                    stream.Write(CHR[i], 0, CHR[i].Length);
                }
            }
        }
        public void LoadState(System.IO.Stream stream)
        {
            byte status = (byte)stream.ReadByte();
            switch (status & 0x3)
            {
                case 0: Mirroring = Nes.Mirroring.Four_Screen; break;
                case 1: Mirroring = Nes.Mirroring.Horizontal; break;
                case 2: Mirroring = Nes.Mirroring.One_Screen; break;
                case 3: Mirroring = Nes.Mirroring.Vertical; break;
            }
            MirroringBase = (ushort)((status & 0x10) == 0x10 ? 0x2400 : 0x2000);
            //load chr if this rom is VRAM
            if (IsVRAM)
            {
                for (int i = 0; i < CHR.Length; i++)
                {
                    stream.Read(CHR[i], 0, CHR[i].Length);
                }
            }
        }
        //properties
        /// <summary>
        /// The mirroring
        /// </summary>
        public Mirroring Mirroring
        {
            get { return mirroring; }
            set { mirroring = value; nes.PPUMemory.ApplyMirroring(); }
        }
        /// <summary>
        /// The VRAM mirroring base, used for One_Screen mirroring
        /// </summary>
        public ushort MirroringBase
        { get { return mirroringBase; } set { mirroringBase = value; nes.PPUMemory.ApplyMirroring(); } }

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
