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
   
    class Mapper83 : IMapper
    {
        CPUMemory map;
        byte[] reg = new byte[3];
        int chr_bank = 0;
        int irq_enable = 0;
        int irq_counter = 0;

        public Mapper83(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            switch (Address)
            {
                case 0x5101:
                case 0x5102:
                case 0x5103:
                    reg[2] = data;
                    break;
                case 0x8000:
                case 0xB000:
                case 0xB0FF:
                case 0xB1FF:
                    reg[0] = data;
                    chr_bank = (data & 0x30) << 4;
                    map.Switch16kPrgRom(data * 4, 0);
                    map.Switch16kPrgRom(((data & 0x30) | 0x0F) * 4, 1);
                    break;

                case 0x8100:
                    reg[1] = (byte)(data & 0x80);
                    data &= 0x03;
                    if (data == 0)
                        map.cartridge.Mirroring = Mirroring.Vertical;
                    else if (data == 1)
                        map.cartridge.Mirroring = Mirroring.Horizontal;
                    else if (data == 2)
                    {
                        map.cartridge.Mirroring = Mirroring.One_Screen;
                        map.cartridge.MirroringBase = 0x2000;
                    }
                    else
                    {
                        map.cartridge.Mirroring = Mirroring.One_Screen;
                        map.cartridge.MirroringBase = 0x2400;
                    }
                    break;

                case 0x8200:
                    irq_counter = (irq_counter & 0xFF00) | data;
                    break;
                case 0x8201:
                    irq_counter = (irq_counter & 0x00FF) | (data << 8);
                    irq_enable = reg[1];
                    break;

                case 0x8300:
                    map.Switch8kPrgRom(data * 2, 0);
                    break;
                case 0x8301:
                    map.Switch8kPrgRom(data * 2, 1);
                    break;
                case 0x8302:
                    map.Switch8kPrgRom(data * 2, 2);
                    break;

                case 0x8310:
                    if (map.cartridge.CHRSizeInKB == 512)
                    {
                        map.Switch2kChrRom((chr_bank | data) * 2, 0);
                    }
                    else
                    {
                        map.Switch1kChrRom(chr_bank | data, 0);
                    }
                    break;
                case 0x8311:
                    if (map.cartridge.CHRSizeInKB == 512)
                    {
                        map.Switch2kChrRom((chr_bank | data) * 2, 1);
                    }
                    else
                    {
                        map.Switch1kChrRom(chr_bank | data, 1);
                    }
                    break;
                case 0x8312:
                    map.Switch1kChrRom(chr_bank | data, 2);
                    break;
                case 0x8313:
                    map.Switch1kChrRom(chr_bank | data, 3);
                    break;
                case 0x8314:
                    map.Switch1kChrRom(chr_bank | data, 4);
                    break;
                case 0x8315:
                    map.Switch1kChrRom(chr_bank | data, 5);
                    break;
                case 0x8316:
                    if (map.cartridge.CHRSizeInKB == 512)
                    {
                        map.Switch2kChrRom((chr_bank | data) * 2, 2);
                    }
                    else
                    {
                        map.Switch1kChrRom(chr_bank | data, 6);
                    }
                    break;
                case 0x8317:
                    if (map.cartridge.CHRSizeInKB == 512)
                    {
                        map.Switch2kChrRom((chr_bank | data) * 2, 3);
                    }
                    else
                    {
                        map.Switch1kChrRom(chr_bank | data, 7);
                    }
                    break;

                case 0x8318:
                    map.Switch16kPrgRom(((reg[0] & 0x30) | data) * 4, 0);
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            if (map.cartridge.PRGSizeInKB >= 256)
            {
                reg[1] = 0x30;
                map.Switch16kPrgRom(0, 0);
                map.Switch8kPrgRom(30 * 2, 2);
                map.Switch8kPrgRom(31 * 2, 3);
            }
            else
            {
                map.Switch16kPrgRom(0, 0);
                map.Switch16kPrgRom((map.cartridge.PRG_PAGES - 1) * 4, 1);
            }
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
            if (irq_enable != 0)
            {
                if (irq_counter <= 113)
                {
                    irq_enable = 0;
                    map.cpu.SetIRQ(true);
                }
                else
                {
                    irq_counter -= 113;
                }
            }
        }
        public void TickCycleTimer(int cycles)
        {
        }
        public void SoftReset()
        { }
        public bool WriteUnder8000
        { get { return false; } }
        public bool WriteUnder6000
        { get { return true; } }

        public byte Read(ushort Address)
        {
            if ((Address & 0x5100) == 0x5100)
                return reg[2];
            return 0;
        }
        public bool ScanlineTimerNotPauseAtVBLANK
        {
            get { return true; }
        }
        public void SaveState(System.IO.Stream stream)
        {
            stream.Write(reg, 0, reg.Length);
            stream.WriteByte((byte)((irq_enable & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_enable & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_enable & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_enable & 0x000000FF)));

            stream.WriteByte((byte)((irq_counter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_counter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_counter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_counter & 0x000000FF)));

            stream.WriteByte((byte)((chr_bank & 0xFF000000) >> 24));
            stream.WriteByte((byte)((chr_bank & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((chr_bank & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((chr_bank & 0x000000FF)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            stream.Read(reg, 0, reg.Length);
            irq_enable = (int)(stream.ReadByte() << 24);
            irq_enable |= (int)(stream.ReadByte() << 16);
            irq_enable |= (int)(stream.ReadByte() << 8);
            irq_enable |= stream.ReadByte();

            irq_counter = (int)(stream.ReadByte() << 24);
            irq_counter |= (int)(stream.ReadByte() << 16);
            irq_counter |= (int)(stream.ReadByte() << 8);
            irq_counter |= stream.ReadByte();

            chr_bank = (int)(stream.ReadByte() << 24);
            chr_bank |= (int)(stream.ReadByte() << 16);
            chr_bank |= (int)(stream.ReadByte() << 8);
            chr_bank |= stream.ReadByte();
        }
    }
}
