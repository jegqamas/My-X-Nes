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
using MyNes.Nes.Input;

namespace MyNes.Nes
{
    /// <summary>
    /// The memory of the nes
    /// </summary>

    public class CPUMemory
    {
        /// <summary>
        /// The memory of the nes
        /// </summary>
        public CPUMemory(NES nes)
        {
            this.nes = nes;
            for (uint i = 0; i < 8; i++)
                CRAM_PAGE[i] = i;
            for (uint i = 0; i < 8; i++)
                SRAM[i] = new byte[0x2000];
        }
        NES nes;

        /// <summary>
        /// The Work RAM
        /// </summary>
        public byte[] RAM = new byte[0x800];
        /// <summary>
        /// nes.Cartridge SRAM Area 8K
        /// </summary>
        public byte[][] SRAM = new byte[8][];//8 x 8KB, nes Mapper 5 may have 64K
        public byte JoyData1 = 0;
        public byte JoyData2 = 0;
        public byte JoyStrobe = 0;
        public bool IsSRAMReadOnly = false;
        public bool IsSoftResetTrigger = false;
        //VS Unisystem
        public byte CoinPresent = 0;
        public byte DIPSwitch1 = 0;
        public byte DIPSwitch2 = 0;
        public byte DIPSwitch3 = 0;
        public byte DIPSwitch4 = 0;
        public byte DIPSwitch5 = 0;
        public byte DIPSwitch6 = 0;
        public byte DIPSwitch7 = 0;
        public byte DIPSwitch8 = 0;

        public IInputManager InputManager;
        public IJoypad Joypad1;
        public IJoypad Joypad2;

        #region Mapping
        public uint[] PRG_PAGE = new uint[8];
        public uint[] CHR_PAGE = new uint[8];
        public uint[] CHRBG_PAGE = new uint[8];
        public uint[] CRAM_PAGE = new uint[8];
        public uint[] CHREX_page = new uint[8];
        public int SRAM_PAGE = 0;

        /// <summary>
        /// Switch 32k Prg Rom
        /// </summary>
        /// <param name="start">start * 8</param>
        public void Switch32kPrgRom(int start)
        {
            int i;
            switch (NES.Cartridge.PRG_PAGES)
            {
                case (2): start = (start & 0x7); break;
                case (4): start = (start & 0xf); break;
                case (8): start = (start & 0x1f); break;
                case (16): start = (start & 0x3f); break;
                case (32): start = (start & 0x7f); break;
                case (64): start = (start & 0xff); break;
                case (128): start = (start & 0x1ff); break;
            }
            for (i = 0; i < 8; i++)
            {
                PRG_PAGE[i] = (uint)(start + i);
            }
        }
        /// <summary>
        /// Switch 16k Prg Rom
        /// </summary>
        /// <param name="start">* 4</param>
        /// <param name="area">area 0,1</param>
        public void Switch16kPrgRom(int start, int area)
        {
            int i;
            switch (NES.Cartridge.PRG_PAGES)
            {
                case (2): start = (start & 0x7); break;
                case (4): start = (start & 0xf); break;
                case (8): start = (start & 0x1f); break;
                case (16): start = (start & 0x3f); break;
                case (32): start = (start & 0x7f); break;
                case (64): start = (start & 0xff); break;
                case (128): start = (start & 0x1ff); break;
            }
            for (i = 0; i < 4; i++)
            {
                PRG_PAGE[4 * area + i] = (uint)(start + i);
            }
        }
        /// <summary>
        /// Switch 8k Prg Rom
        /// </summary>
        /// <param name="start">* 2</param>
        /// <param name="area">area 0,1,2,3</param>
        public void Switch8kPrgRom(int start, int area)
        {
            int i;
            switch (NES.Cartridge.PRG_PAGES)
            {
                case (2): start = (start & 0x7); break;
                case (4): start = (start & 0xf); break;
                case (8): start = (start & 0x1f); break;
                case (16): start = (start & 0x3f); break;
                case (32): start = (start & 0x7f); break;
                case (64): start = (start & 0xff); break;
                case (128): start = (start & 0x1ff); break;
            }
            for (i = 0; i < 2; i++)
            {
                PRG_PAGE[2 * area + i] = (uint)(start + i);
            }
        }

        /// <summary>
        /// Switch 8k Chr Rom
        /// </summary>
        /// <param name="start">* 8</param>
        public void Switch8kChrRom(int start)
        {
            switch (NES.Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (7)://special case of mapper 204
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
                case (128): start = (start & 0x3ff); break;
                //case (236): start = (start & 0x3ff); break;
            }
            for (int i = 0; i < 8; i++)
            {
                CHR_PAGE[i] = (uint)(start + i);
            }
        }
        /// <summary>
        /// Switch 4k Chr Rom
        /// </summary>
        /// <param name="start">* 4</param>
        /// <param name="area">area 0,1</param>
        public void Switch4kChrRom(int start, int area)
        {
            int i;
            switch (NES.Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
                case (128): start = (start & 0x3ff); break;
            }
            for (i = 0; i < 4; i++)
            {
                CHR_PAGE[4 * area + i] = (uint)(start + i);
            }
        }
        /// <summary>
        /// Switch 2k Chr Rom
        /// </summary>
        /// <param name="start">* 2 </param>
        /// <param name="area">area 0,1,2,3</param>
        public void Switch2kChrRom(int start, int area)
        {
            int i;
            switch (NES.Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
                case (128): start = (start & 0x3ff); break;
            }
            for (i = 0; i < 2; i++)
            {
                CHR_PAGE[2 * area + i] = (uint)(start + i);
            }
        }
        /// <summary>
        /// Switch 1k Chr Rom
        /// </summary>
        /// <param name="start">* 1</param>
        /// <param name="area">area 0,1,2,3,4,5,6,7</param>
        public void Switch1kChrRom(int start, int area)
        {
            switch (NES.Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
                case (128): start = (start & 0x3ff); break;
            }
            CHR_PAGE[area] = (uint)(start);
        }
        public void Switch4kChrEXRom(int start, int area)
        {
            int i;
            switch (NES.Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
                case (128): start = (start & 0x3ff); break;
            }
            for (i = 0; i < 4; i++)
            {
                CHREX_page[4 * area + i] = (uint)(start + i);
            }
        }

        /// <summary>
        /// Switch 8k Chr Rom
        /// </summary>
        /// <param name="start">* 8</param>
        public void Switch8kBGChrRom(int start)
        {
            switch (NES.Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
                case (128): start = (start & 0x3ff); break;
                //case (236): start = (start & 0x3ff); break;
            }
            for (int i = 0; i < 8; i++)
            {
                CHRBG_PAGE[i] = (uint)(start + i);
            }
        }
        /// <summary>
        /// Switch 4k Chr Rom
        /// </summary>
        /// <param name="start">* 4</param>
        /// <param name="area">area 0,1</param>
        public void Switch4kBGChrRom(int start, int area)
        {
            int i;
            switch (NES.Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
                case (128): start = (start & 0x3ff); break;
            }
            for (i = 0; i < 4; i++)
            {
                CHRBG_PAGE[4 * area + i] = (uint)(start + i);
            }
        }
        /// <summary>
        /// Switch 2k Chr Rom
        /// </summary>
        /// <param name="start">* 2 </param>
        /// <param name="area">area 0,1,2,3</param>
        public void Switch2kBGChrRom(int start, int area)
        {
            int i;
            switch (NES.Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
                case (128): start = (start & 0x3ff); break;
            }
            for (i = 0; i < 2; i++)
            {
                CHRBG_PAGE[2 * area + i] = (uint)(start + i);
            }
        }
        /// <summary>
        /// Switch 1k Chr Rom
        /// </summary>
        /// <param name="start">* 1</param>
        /// <param name="area">area 0,1,2,3,4,5,6,7</param>
        public void Switch1kBGChrRom(int start, int area)
        {
            switch (NES.Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
                case (128): start = (start & 0x3ff); break;
            }
            CHRBG_PAGE[area] = (uint)(start);
        }

        /// <summary>
        /// Switch 8k Prg ROM to SRAM at 0x6000 to 0x7FFF
        /// </summary>
        /// <param name="start">* 2</param>
        public void Switch8kPrgRomToSRAM(int start)
        {
            switch (NES.Cartridge.PRG_PAGES)
            {
                case (2): start = (start & 0x7); break;
                case (4): start = (start & 0xf); break;
                case (8): start = (start & 0x1f); break;
                case (16): start = (start & 0x3f); break;
                case (32): start = (start & 0x7f); break;
                case (64): start = (start & 0xff); break;
                case (128): start = (start & 0x1ff); break;
            }
            NES.Cartridge.PRG[start].CopyTo(SRAM[SRAM_PAGE], 0);
            for (int i = 0x1000; i < 0x2000; i++)
            {
                SRAM[SRAM_PAGE][i] = NES.Cartridge.PRG[start + 1][i - 0x1000];
            }
        }
        /// <summary>
        /// Switch 8k sram Rom
        /// </summary>
        /// <param name="start">which bank you want to use ?, 0-7</param>
        public void Switch8kSRAM(int start)
        {
            SRAM_PAGE = start;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start">* 2, the sram bank</param>
        /// <param name="area">area 0,1,2,3</param>
        public void Switch8kSRAMToPRG(int start, int area)
        {
            // CONSOLE.WriteLine(this, "SRAM TO PRG SWITCH, start = " + start + ", area = " + area, DebugStatus.Warning);
        }

        //area 0,1,2,3,4,5,6,7
        public void Switch1kCRAM(int start, int area)
        {
            CRAM_PAGE[area] = (uint)(start - NES.Cartridge.CHR.Length);
            Switch1kChrRom(start, area);
        }
        //area 0,1,2,3,4,5,6,7
        public void Switch1kCRAMEX(int start, int area)
        {
            if (start >= NES.Cartridge.CHR.Length)
                start -= NES.Cartridge.CHR.Length;
            CRAM_PAGE[area] = (uint)start;
            Switch1kChrRom(start + NES.Cartridge.CHR.Length, area);
        }
        public void CloneCHRtoCRAM(int additional)
        {
            NES.PPUMemory.CRAM = new byte[NES.Cartridge.CHR.Length + additional][];
            for (int i = 0; i < NES.Cartridge.CHR.Length; i++)
                NES.PPUMemory.CRAM[i] = NES.Cartridge.CHR[i];
            for (int i = NES.Cartridge.CHR.Length; i < NES.PPUMemory.CRAM.Length; i++)
                NES.PPUMemory.CRAM[i] = new byte[1024];
        }
        public void CloneCHRtoCRAM()
        {
            CloneCHRtoCRAM(0);
        }
        public void Switch1kCHRToVRAM(int start, int area)
        {
            switch (NES.Cartridge.CHR_PAGES)
            {
                case (2): start = (start & 0xf); break;
                case (4): start = (start & 0x1f); break;
                case (8): start = (start & 0x3f); break;
                case (16): start = (start & 0x7f); break;
                case (32): start = (start & 0xff); break;
                case (64): start = (start & 0x1ff); break;
                case (128): start = (start & 0x1ff); break;
            }
            NES.PPUMemory.NameTableIndexes[area] = (byte)start;
        }
        public void SwitchVRAM(byte start, byte area)
        {
            NES.PPUMemory.NameTableIndexes[area] = start;
        }

        /// <summary>
        /// Call this only if VRAM
        /// </summary>
        /// <param name="Pages">1 K pages</param>
        public void FillCHR(int Pages)
        {
            NES.Cartridge.CHR = new byte[Pages][];
            for (int i = 0; i < Pages; i++)
            {
                NES.Cartridge.CHR[i] = new byte[1024];
            }
        }
        public void FillCRAM(int Pages)
        {
            NES.PPUMemory.CRAM = new byte[Pages][];
            for (int i = 0; i < Pages; i++)
            {
                NES.PPUMemory.CRAM[i] = new byte[1024];
            }
        }
        #endregion

        /// <summary>
        /// The memory of the nes
        /// </summary>
        /// <param name="Address">The address</param>
        /// <returns>Byte : the value</returns>
        public byte this[ushort Address]
        {
            get
            {
                /*Work RAM*/
                if (Address < 0x2000)
                { return RAM[Address & 0x07FF]; }
                /*nes.PPU Registers*/
                else if (Address < 0x4000)
                {
                    Address &= 7;//for mirroring
                    if (Address == 2)//0x2002
                        return NES.PPU.Read2002();
                    else if (Address == 4)//0x2004
                        return NES.PPU.Read2004();
                    else if (Address == 7)//0x2007
                        return NES.PPU.Read2007();
                }
                /*nes.APU Registers*/
                else if (Address < 0x4018)
                {
                    if (Address == 0x4015)
                    { return NES.APU.Peek4015(0x4015); }
                    else if (Address == 0x4016)
                    {
                        byte v = (byte)(0x40 | (JoyData1 & 1));
                        JoyData1 = (byte)((JoyData1 >> 1) | 0x80);
                        return v;
                    }
                    else if (Address == 0x4017)
                    {
                        byte v = (byte)(0x40 | (JoyData2 & 1));
                        JoyData2 = (byte)((JoyData2 >> 1) | 0x80);
                        return v;
                    }
                }
                /*nes.Cartridge Expansion Area almost 8K*/
                else if (Address < 0x6000)
                {
                    return NES.Mapper.Read(Address);
                }
                /*nes.Cartridge SRAM Area 8K*/
                else if (Address < 0x8000)
                {
                    return SRAM[SRAM_PAGE][Address & 0x1FFF];
                }
                /*nes.Cartridge PRG-ROM Area 32K*/
                else
                {
                    Address -= 0x8000;
                    return NES.Cartridge.PRG[PRG_PAGE[(Address & 0xF000) >> 12]][Address & 0x0FFF];
                }
                return 0;//make the compiler happy
            }
            set 
            {
                /*Work RAM*/
                if (Address < 0x2000)
                {
                    RAM[Address & 0x07FF] = value;
                }
                /*nes.PPU Registers*/
                else if (Address < 0x4000)
                {
                    Address &= 7;//for mirroring
                    if (Address == 0)//0x2000
                        NES.PPU.Write2000(value);
                    else if (Address == 1)//0x2001
                        NES.PPU.Write2001(value);
                    else if (Address == 3)//0x2003
                        NES.PPU.Write2003(value);
                    else if (Address == 4)//0x2004
                        NES.PPU.Write2004(value);
                    else if (Address == 5)//0x2005
                        NES.PPU.Write2005(value);
                    else if (Address == 6)//0x2006
                        NES.PPU.Write2006(value);
                    else if (Address == 7)//0x2007
                        NES.PPU.Write2007(value);
                }
                /*nes.APU Registers*/
                else if (Address < 0x4018)
                {
                    if (Address == 0x4000)
                        NES.APU.ChannelSq1.Poke1(0x4000, value);
                    else if (Address == 0x4001)
                        NES.APU.ChannelSq1.Poke2(0x4001, value);
                    else if (Address == 0x4002)
                        NES.APU.ChannelSq1.Poke3(0x4002, value);
                    else if (Address == 0x4003)
                        NES.APU.ChannelSq1.Poke4(0x4003, value);
                    else if (Address == 0x4004)
                        NES.APU.ChannelSq2.Poke1(0x4004, value);
                    else if (Address == 0x4005)
                        NES.APU.ChannelSq2.Poke2(0x4005, value);
                    else if (Address == 0x4006)
                        NES.APU.ChannelSq2.Poke3(0x4006, value);
                    else if (Address == 0x4007)
                        NES.APU.ChannelSq2.Poke4(0x4007, value);
                    else if (Address == 0x4008)
                        NES.APU.ChannelTri.Poke1(0x4008, value);
                    else if (Address == 0x4009)
                        NES.APU.ChannelTri.Poke2(0x4009, value);
                    else if (Address == 0x400A)
                        NES.APU.ChannelTri.Poke3(0x400A, value);
                    else if (Address == 0x400B)
                        NES.APU.ChannelTri.Poke4(0x400B, value);
                    else if (Address == 0x400C)
                        NES.APU.ChannelNoi.Poke1(0x400C, value);
                    else if (Address == 0x400D)
                        NES.APU.ChannelNoi.Poke2(0x400D, value);
                    else if (Address == 0x400E)
                        NES.APU.ChannelNoi.Poke3(0x400E, value);
                    else if (Address == 0x400F)
                        NES.APU.ChannelNoi.Poke4(0x400F, value);
                    else if (Address == 0x4010)
                        NES.APU.ChannelDpm.Poke1(0x4010, value);
                    else if (Address == 0x4011)
                        NES.APU.ChannelDpm.Poke2(0x4011, value);
                    else if (Address == 0x4012)
                        NES.APU.ChannelDpm.Poke3(0x4012, value);
                    else if (Address == 0x4013)
                        NES.APU.ChannelDpm.Poke4(0x4013, value);
                    else if (Address == 0x4014)//DMA
                        NES.PPU.Write4014(value);
                    else if (Address == 0x4015)
                        NES.APU.Poke4015(0x4015, value);
                    else if (Address == 0x4016)
                    {
                        if ((JoyStrobe == 1) && ((value & 1) == 0))
                        {
                            InputManager.Update();
                            JoyData1 = Joypad1.GetData();
                            JoyData2 = Joypad2.GetData();
                        }
                        JoyStrobe = (byte)(value & 1);
                    }
                    else if (Address == 0x4017)
                        NES.APU.Poke4017(0x4017, value);
                }
                /*nes.Cartridge Expansion Area almost 8K*/
                else if (Address < 0x6000)
                {
                    if (NES.Mapper.WriteUnder6000)
                        NES.Mapper.Write(Address, value);
                }
                /*nes.Cartridge SRAM Area */
                else if (Address < 0x8000)
                {
                    if (NES.Mapper.WriteUnder8000)
                        NES.Mapper.Write(Address, value);

                    if (!IsSRAMReadOnly)
                        SRAM[SRAM_PAGE][Address & 0x1FFF] = value;
                }
                /*nes.Cartridge PRG-ROM Area 32K*/
                else
                {
                    NES.Mapper.Write(Address, value);
                }
            }
        }
        /// <summary>
        /// Debug read, this will make a dump read for $2002 and $4015 (e.g: read $2002 will not clear sprite0hit flag)
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Value</returns>
        public byte DumpRead(ushort address)
        {
            if ((address != 0x2002) & (address != 0x4015))
                return this[address];
            else
                switch (address)
                {
                    case 0x2002: return NES.PPU.DumpRead2002();
                    case 0x4015: return NES.APU.Dump4015(0x4015);
                    default: return 0;
                }
        }
        /// <summary>
        /// Get the nes engine
        /// </summary>
        public NES NES
        { get { return nes; } }
        /// <summary>
        /// Get the nes cartridge
        /// </summary>
        public Cartridge cartridge
        { get { return NES.Cartridge; } }
        /// <summary>
        /// Get the nes apu
        /// </summary>
        public NesApu apu
        { get { return nes.APU; } }
        public PPU ppu
        { get { return nes.PPU; } }
        public IMapper MAPPER
        { get { return nes.Mapper; } }
        public CPU cpu
        { get { return nes.CPU; } }

        public void SaveState(System.IO.Stream stream)
        {
            //ram
            stream.Write(RAM, 0, RAM.Length);
            //sram
            if (nes.Cartridge.MAPPER == 5)//if mapper 5, save all sram otherwise save the first one only
            {
                stream.Write(SRAM[0], 0, SRAM[0].Length);
                stream.Write(SRAM[1], 0, SRAM[1].Length);
                stream.Write(SRAM[2], 0, SRAM[2].Length);
                stream.Write(SRAM[3], 0, SRAM[3].Length);
                stream.Write(SRAM[4], 0, SRAM[4].Length);
                stream.Write(SRAM[5], 0, SRAM[5].Length);
                stream.Write(SRAM[6], 0, SRAM[6].Length);
                stream.Write(SRAM[7], 0, SRAM[7].Length);
            }
            else
                stream.Write(SRAM[0], 0, SRAM[0].Length);
            //joypad
            stream.WriteByte(JoyData1);
            stream.WriteByte(JoyData2);
            stream.WriteByte(JoyStrobe);
            //mapping
            for (int i = 0; i < 8; i++)
            {
                stream.WriteByte((byte)((PRG_PAGE[i] & 0xFF000000) >> 24));
                stream.WriteByte((byte)((PRG_PAGE[i] & 0x00FF0000) >> 16));
                stream.WriteByte((byte)((PRG_PAGE[i] & 0x0000FF00) >> 8));
                stream.WriteByte((byte)((PRG_PAGE[i] & 0x000000FF)));
            }
            for (int i = 0; i < 8; i++)
            {
                stream.WriteByte((byte)((CHR_PAGE[i] & 0xFF000000) >> 24));
                stream.WriteByte((byte)((CHR_PAGE[i] & 0x00FF0000) >> 16));
                stream.WriteByte((byte)((CHR_PAGE[i] & 0x0000FF00) >> 8));
                stream.WriteByte((byte)((CHR_PAGE[i] & 0x000000FF)));
            }
            for (int i = 0; i < 8; i++)
            {
                stream.WriteByte((byte)((CHRBG_PAGE[i] & 0xFF000000) >> 24));
                stream.WriteByte((byte)((CHRBG_PAGE[i] & 0x00FF0000) >> 16));
                stream.WriteByte((byte)((CHRBG_PAGE[i] & 0x0000FF00) >> 8));
                stream.WriteByte((byte)((CHRBG_PAGE[i] & 0x000000FF)));
            }
            for (int i = 0; i < 8; i++)
            {
                stream.WriteByte((byte)((CRAM_PAGE[i] & 0xFF000000) >> 24));
                stream.WriteByte((byte)((CRAM_PAGE[i] & 0x00FF0000) >> 16));
                stream.WriteByte((byte)((CRAM_PAGE[i] & 0x0000FF00) >> 8));
                stream.WriteByte((byte)((CRAM_PAGE[i] & 0x000000FF)));
            }
            for (int i = 0; i < 8; i++)
            {
                stream.WriteByte((byte)((CHREX_page[i] & 0xFF000000) >> 24));
                stream.WriteByte((byte)((CHREX_page[i] & 0x00FF0000) >> 16));
                stream.WriteByte((byte)((CHREX_page[i] & 0x0000FF00) >> 8));
                stream.WriteByte((byte)((CHREX_page[i] & 0x000000FF)));
            }
            stream.WriteByte((byte)((SRAM_PAGE & 0xFF000000) >> 24));
            stream.WriteByte((byte)((SRAM_PAGE & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((SRAM_PAGE & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((SRAM_PAGE & 0x000000FF)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            //ram
            stream.Read(RAM, 0, RAM.Length);
            //sram
            if (nes.Cartridge.MAPPER == 5)//if mapper 5, save all sram otherwise save the first one only
            {
                stream.Read(SRAM[0], 0, SRAM[0].Length);
                stream.Read(SRAM[1], 0, SRAM[1].Length);
                stream.Read(SRAM[2], 0, SRAM[2].Length);
                stream.Read(SRAM[3], 0, SRAM[3].Length);
                stream.Read(SRAM[4], 0, SRAM[4].Length);
                stream.Read(SRAM[5], 0, SRAM[5].Length);
                stream.Read(SRAM[6], 0, SRAM[6].Length);
                stream.Read(SRAM[7], 0, SRAM[7].Length);
            }
            else
                stream.Read(SRAM[0], 0, SRAM[0].Length);
            //joypad
            JoyData1 = (byte)stream.ReadByte();
            JoyData2 = (byte)stream.ReadByte();
            JoyStrobe = (byte)stream.ReadByte();
            //mapping
            for (int i = 0; i < 8; i++)
            {
                PRG_PAGE[i] = (uint)(stream.ReadByte() << 24);
                PRG_PAGE[i] |= (uint)(stream.ReadByte() << 16);
                PRG_PAGE[i] |= (uint)(stream.ReadByte() << 8);
                PRG_PAGE[i] |= (uint)(stream.ReadByte());
            }
            for (int i = 0; i < 8; i++)
            {
                CHR_PAGE[i] = (uint)(stream.ReadByte() << 24);
                CHR_PAGE[i] |= (uint)(stream.ReadByte() << 16);
                CHR_PAGE[i] |= (uint)(stream.ReadByte() << 8);
                CHR_PAGE[i] |= (uint)(stream.ReadByte());
            }
            for (int i = 0; i < 8; i++)
            {
                CHRBG_PAGE[i] = (uint)(stream.ReadByte() << 24);
                CHRBG_PAGE[i] |= (uint)(stream.ReadByte() << 16);
                CHRBG_PAGE[i] |= (uint)(stream.ReadByte() << 8);
                CHRBG_PAGE[i] |= (uint)(stream.ReadByte());
            }
            for (int i = 0; i < 8; i++)
            {
                CRAM_PAGE[i] = (uint)(stream.ReadByte() << 24);
                CRAM_PAGE[i] |= (uint)(stream.ReadByte() << 16);
                CRAM_PAGE[i] |= (uint)(stream.ReadByte() << 8);
                CRAM_PAGE[i] |= (uint)(stream.ReadByte());
            }
            for (int i = 0; i < 8; i++)
            {
                CHREX_page[i] = (uint)(stream.ReadByte() << 24);
                CHREX_page[i] |= (uint)(stream.ReadByte() << 16);
                CHREX_page[i] |= (uint)(stream.ReadByte() << 8);
                CHREX_page[i] |= (uint)(stream.ReadByte());
            }
            SRAM_PAGE = (int)(stream.ReadByte() << 24);
            SRAM_PAGE |= (int)(stream.ReadByte() << 16);
            SRAM_PAGE |= (int)(stream.ReadByte() << 8);
            SRAM_PAGE |= stream.ReadByte();
        }
    }
}
