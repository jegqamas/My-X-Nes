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
//DATE                      : By       : What ?
//Thursday, February 3, 2011: By AHD   : For cart above 512KB
namespace MyNes.Nes
{
   
    class Mapper01 : IMapper
    {
        CPUMemory MAP;
        public Mapper01(CPUMemory map)
        { MAP = map; }
        byte[] reg = new byte[4];
        byte SHIFT = 0;
        byte BUFFER = 0;
        bool MODE = false;

        public void Write(ushort address, byte data)
        {
            //Reset if it want to
            if ((data & 0x80) == 0x80)
            {
                reg[0] |= 0x0C;
                SHIFT = BUFFER = 0;
                return;
            }
            //Shift
            if ((data & 0x01) == 0x01)
                BUFFER |= (byte)(1 << SHIFT);
            if (++SHIFT < 5)
                return;

            address = (ushort)((address & 0x7FFF) >> 13);
            reg[address] = BUFFER;

            //Reset
            SHIFT = BUFFER = 0;

            if (!MODE)//Normal
            {
                switch (address)
                {
                    case 0:
                        if ((reg[0] & 0x02) == 0x02)
                        {
                            if ((reg[0] & 0x01) == 0x01)
                                MAP.cartridge.Mirroring = Mirroring.Horizontal;
                            else
                                MAP.cartridge.Mirroring = Mirroring.Vertical;
                        }
                        else
                        {
                            if ((reg[0] & 0x01) == 0x01)
                            {
                                MAP.cartridge.Mirroring = Mirroring.One_Screen;
                                MAP.cartridge.MirroringBase = 0x2400;
                            }
                            else
                            {
                                MAP.cartridge.Mirroring = Mirroring.One_Screen;
                                MAP.cartridge.MirroringBase = 0x2000;
                            }
                        }
                        break;
                    case 1:
                        // Register #1
                        if (!MAP.cartridge.IsVRAM)
                        {
                            if ((reg[0] & 0x10) == 0x10)
                            {
                                MAP.Switch4kChrRom(reg[1] * 4, 0);
                                MAP.Switch4kChrRom(reg[2] * 4, 1);
                            }
                            else
                            {
                                MAP.Switch8kChrRom((reg[1] >> 1) * 8);
                            }
                        }
                        else
                        {
                            if ((reg[0] & 0x10) == 0x10)
                            {
                                MAP.Switch4kChrRom(reg[1] * 4, 0);
                            }
                        }
                        break;
                    case 2:
                        // Register #2
                        if (!MAP.cartridge.IsVRAM)
                        {
                            if ((reg[0] & 0x10) == 0x10)
                            {

                                MAP.Switch4kChrRom(reg[1] * 4, 0);
                                MAP.Switch4kChrRom(reg[2] * 4, 1);
                            }
                            else
                            {
                                MAP.Switch8kChrRom((reg[1] >> 1) * 8);
                            }
                        }
                        else
                        {
                            if ((reg[0] & 0x10) == 0x10)
                            {
                                MAP.Switch4kChrRom(reg[2] * 4, 1);
                            }
                        }
                        break;
                    case 3:
                        if ((reg[0] & 0x08) == 0)
                        {
                            MAP.Switch32kPrgRom((reg[3] >> 1) * 8);
                        }
                        else
                        {
                            if ((reg[0] & 0x04) == 0x04)
                            {
                                MAP.Switch16kPrgRom(reg[3] * 4, 0);
                                MAP.Switch16kPrgRom((MAP.cartridge.PRG_PAGES - 1) * 4, 1);
                            }
                            else
                            {
                                MAP.Switch16kPrgRom(0, 0);
                                MAP.Switch16kPrgRom(reg[3] * 4, 1);
                            }
                        }
                        break;
                }
            }
            else//above 512 K
            {
                int BASE = reg[1] & 0x10;

                // Register #0
                if (address == 0)
                {
                    if ((reg[0] & 0x02) == 0x02)
                    {
                        if ((reg[0] & 0x01) == 0x01)
                            MAP.cartridge.Mirroring = Mirroring.Horizontal;
                        else
                            MAP.cartridge.Mirroring = Mirroring.Vertical;
                    }
                    else
                    {
                        if ((reg[0] & 0x01) == 0x01)
                        {
                            MAP.cartridge.Mirroring = Mirroring.One_Screen;
                            MAP.cartridge.MirroringBase = 0x2400;
                        }
                        else
                        {
                            MAP.cartridge.Mirroring = Mirroring.One_Screen;
                            MAP.cartridge.MirroringBase = 0x2000;
                        }
                    }
                }
                // Register #1
                if (!MAP.cartridge.IsVRAM)
                {
                    if ((reg[0] & 0x10) == 0x10)
                    {
                        MAP.Switch4kChrRom(reg[1] * 4, 0);
                    }
                    else
                    {
                        MAP.Switch8kChrRom((reg[1] >> 1) * 8);
                    }
                }
                else
                {
                    if ((reg[0] & 0x10) == 0x10)
                    {
                        MAP.Switch4kChrRom(reg[1] * 4, 0);
                    }
                }
                // Register #2
                if (!MAP.cartridge.IsVRAM)
                {
                    if ((reg[0] & 0x10) == 0x10)
                    {
                        MAP.Switch4kChrRom(reg[2] * 4, 1);
                    }
                }
                else
                {
                    if ((reg[0] & 0x10) == 0x10)
                    {
                        MAP.Switch4kChrRom(reg[2] * 4, 1);
                    }
                }
                // Register #3
                if ((reg[0] & 0x08) == 0)
                {
                    MAP.Switch32kPrgRom(((reg[3] & (0xF + BASE)) >> 1) * 8);
                }
                else
                {
                    if ((reg[0] & 0x04) == 0x04)
                    {
                        MAP.Switch16kPrgRom((BASE + (reg[3] & 0x0F)) * 4, 0);
                        MAP.Switch16kPrgRom((BASE + 16 - 1) * 4, 1);
                    }
                    else
                    {

                        MAP.Switch16kPrgRom(BASE * 4, 0);
                        MAP.Switch16kPrgRom((BASE + (reg[3] & 0x0F)) * 4, 1);
                    }
                }
            }
        }

        public void SetUpMapperDefaults()
        {
            reg[0] = 0x0C;// D3=1,D2=1
            reg[1] = reg[2] = reg[3] = 0;

            if (MAP.cartridge.PRG_PAGES < 32)
            {
                MAP.Switch16kPrgRom(0, 0);
                MAP.Switch16kPrgRom((MAP.cartridge.PRG_PAGES - 1) * 4, 1);
            }
            else
            {
                MAP.Switch16kPrgRom(0, 0);
                MAP.Switch16kPrgRom((15) * 4, 1); MODE = true;
            }

            if (MAP.cartridge.IsVRAM)
                MAP.FillCHR(16);
            MAP.Switch8kChrRom(0);
        }

        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer(int cycles)
        {
        }
        public void SoftReset()
        {
        }
        public bool WriteUnder8000
        {
            get { return false; }
        }
        public bool WriteUnder6000
        {
            get { return false; }
        }
        public byte Read(ushort Address)
        {
            return 0;
        }
        public bool ScanlineTimerNotPauseAtVBLANK
        {
            get { return false; }
        }

        public void SaveState(System.IO.Stream stream)
        {
            stream.WriteByte(reg[0]);
            stream.WriteByte(reg[1]);
            stream.WriteByte(reg[2]);
            stream.WriteByte(reg[3]);
            stream.WriteByte(SHIFT);
            stream.WriteByte(BUFFER);
        }
        public void LoadState(System.IO.Stream stream)
        {
            reg[0] = (byte)stream.ReadByte();
            reg[1] = (byte)stream.ReadByte();
            reg[2] = (byte)stream.ReadByte();
            reg[3] = (byte)stream.ReadByte();
            SHIFT = (byte)stream.ReadByte(); 
            BUFFER = (byte)stream.ReadByte();
        }
    }
}
