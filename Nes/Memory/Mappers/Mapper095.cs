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

namespace MyNes.Nes
{

    class Mapper95 : IMapper
    {
        CPUMemory Map;
        byte reg = 0x00;
        byte prg0 = 0;
        byte prg1 = 1;
        byte chr01 = 0;
        byte chr23 = 2;
        byte chr4 = 4;
        byte chr5 = 5;
        byte chr6 = 6;
        byte chr7 = 7;

        public Mapper95(CPUMemory map)
        { Map = map; }
        public void Write(ushort address, byte data)
        {
            switch (address & 0xE001)
            {
                case 0x8000:
                    reg = data;
                    SetBank_CPU();
                    SetBank_PPU();
                    break;
                case 0x8001:
                    if (reg <= 0x05)
                    {
                        if ((data & 0x20) != 0)
                        {
                            Map.cartridge.Mirroring = Mirroring.One_Screen;
                            Map.cartridge.MirroringBase = 0x2400;
                        }
                        else
                        {
                            Map.cartridge.Mirroring = Mirroring.One_Screen;
                            Map.cartridge.MirroringBase = 0x2000;
                        }
                        data &= 0x1F;
                    }

                    switch (reg & 0x07)
                    {
                        case 0x00:
                            //if (VROM_1K_SIZE)
                            {
                                chr01 = (byte)(data & 0xFE);
                                SetBank_PPU();
                            }
                            break;
                        case 0x01:
                            //if (VROM_1K_SIZE)
                            {
                                chr23 = (byte)(data & 0xFE);
                                SetBank_PPU();
                            }
                            break;
                        case 0x02:
                            //if (VROM_1K_SIZE)
                            {
                                chr4 = data;
                                SetBank_PPU();
                            }
                            break;
                        case 0x03:
                            //if (VROM_1K_SIZE)
                            {
                                chr5 = data;
                                SetBank_PPU();
                            }
                            break;
                        case 0x04:
                            //if (VROM_1K_SIZE)
                            {
                                chr6 = data;
                                SetBank_PPU();
                            }
                            break;
                        case 0x05:
                            //if (VROM_1K_SIZE)
                            {
                                chr7 = data;
                                SetBank_PPU();
                            }
                            break;
                        case 0x06:
                            prg0 = data;
                            SetBank_CPU();
                            break;
                        case 0x07:
                            prg1 = data;
                            SetBank_CPU();
                            break;
                    }
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            SetBank_CPU();
            if (Map.cartridge.IsVRAM)
                Map.FillCHR(16);
            SetBank_PPU();
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
            get { return true; }
        }
        public bool WriteUnder6000
        {
            get { return true; }
        }
        void SetBank_CPU()
        {
            if ((reg & 0x40) != 0)
            {
                //SetPROM_32K_Bank( PROM_8K_SIZE-2, prg1, prg0, PROM_8K_SIZE-1 );
                Map.Switch8kPrgRom(((Map.cartridge.PRG_PAGES * 2) - 2) * 2, 0);
                Map.Switch8kPrgRom(prg1 * 2, 1);
                Map.Switch8kPrgRom(prg0 * 2, 2);
                Map.Switch8kPrgRom(((Map.cartridge.PRG_PAGES * 2) - 1) * 2, 3);

            }
            else
            {
                //SetPROM_32K_Bank(prg0, prg1, PROM_8K_SIZE - 2, PROM_8K_SIZE - 1);
                Map.Switch8kPrgRom(prg0 * 2, 0);
                Map.Switch8kPrgRom(prg1 * 2, 1);
                Map.Switch8kPrgRom(((Map.cartridge.PRG_PAGES * 2) - 2) * 2, 2);
                Map.Switch8kPrgRom(((Map.cartridge.PRG_PAGES * 2) - 1) * 2, 3);

            }
        }
        void SetBank_PPU()
        {
            if ((reg & 0x80) != 0)
            {
                //SetVROM_8K_Bank( chr4, chr5, chr6, chr7,
                //		 chr01, chr01+1, chr23, chr23+1 );
                Map.Switch1kChrRom(chr4, 0);
                Map.Switch1kChrRom(chr5, 1);
                Map.Switch1kChrRom(chr6, 2);
                Map.Switch1kChrRom(chr7, 3);
                Map.Switch1kChrRom(chr01, 4);
                Map.Switch1kChrRom(chr01 + 1, 5);
                Map.Switch1kChrRom(chr23, 6);
                Map.Switch1kChrRom(chr23 + 1, 7);
            }
            else
            {
                //SetVROM_8K_Bank( chr01, chr01+1, chr23, chr23+1,
                //		 chr4, chr5, chr6, chr7 );
                Map.Switch1kChrRom(chr01, 0);
                Map.Switch1kChrRom(chr01 + 1, 1);
                Map.Switch1kChrRom(chr23, 2);
                Map.Switch1kChrRom(chr23 + 1, 3);
                Map.Switch1kChrRom(chr4, 4);
                Map.Switch1kChrRom(chr5, 5);
                Map.Switch1kChrRom(chr6, 6);
                Map.Switch1kChrRom(chr7, 7);
            }
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
            stream.WriteByte(reg);
            stream.WriteByte(prg0);
            stream.WriteByte(prg1);
            stream.WriteByte(chr01);
            stream.WriteByte(chr23);
            stream.WriteByte(chr4);
            stream.WriteByte(chr5);
            stream.WriteByte(chr6);
            stream.WriteByte(chr7);
        }
        public void LoadState(System.IO.Stream stream)
        {
            reg = (byte)stream.ReadByte();
            prg0 = (byte)stream.ReadByte();
            prg1 = (byte)stream.ReadByte();
            chr01 = (byte)stream.ReadByte();
            chr23 = (byte)stream.ReadByte();
            chr4 = (byte)stream.ReadByte();
            chr5 = (byte)stream.ReadByte();
            chr6 = (byte)stream.ReadByte();
            chr7 = (byte)stream.ReadByte();
        }
    }
}
