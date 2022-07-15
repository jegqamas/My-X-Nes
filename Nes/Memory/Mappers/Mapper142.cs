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

    class Mapper142 : IMapper
    {
        byte prg_sel = 0;
        byte irq_enable = 0;
        int irq_counter = 0;
        CPUMemory map;
        public Mapper142(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            switch (Address & 0xF000)
            {
                case 0x8000:
                    irq_counter = (irq_counter & 0xFFF0) | ((data & 0x0F) << 0);
                    break;
                case 0x9000:
                    irq_counter = (irq_counter & 0xFF0F) | ((data & 0x0F) << 4);
                    break;
                case 0xA000:
                    irq_counter = (irq_counter & 0xF0FF) | ((data & 0x0F) << 8);
                    break;
                case 0xB000:
                    irq_counter = (irq_counter & 0x0FFF) | ((data & 0x0F) << 12);
                    break;
                case 0xC000:
                    irq_enable = (byte)(data & 0x0F);
                    break;
                case 0xE000:
                    prg_sel = (byte)(data & 0x0F);
                    break;
                case 0xF000:
                    switch (prg_sel)
                    {
                        case 1: map.Switch8kPrgRom((data & 0x0F) * 2, 0); break;
                        case 2: map.Switch8kPrgRom((data & 0x0F) * 2, 1); break;
                        case 3: map.Switch8kPrgRom((data & 0x0F) * 2, 2); break;
                        case 4: map.Switch8kPrgRomToSRAM((data & 0x0F) * 2); break;
                    }
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            map.Switch8kPrgRomToSRAM(0);
            map.Switch8kPrgRom(0x0F * 2, 3);
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
            if (irq_enable != 0)
            {
                if (irq_counter > (0xFFFF - 113))
                {
                    irq_counter = 0;
                    map.cpu.SetIRQ(true);
                }
                else
                {
                    irq_counter += 113;
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
        { get { return false; } }
        public byte Read(ushort Address)
        {
            return 0;
        }
        public bool ScanlineTimerNotPauseAtVBLANK
        {
            get { return true; }
        }

        public void SaveState(System.IO.Stream stream)
        {
            stream.WriteByte(prg_sel);
            stream.WriteByte(irq_enable);
            stream.WriteByte((byte)((irq_counter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_counter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_counter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_counter & 0x000000FF)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            prg_sel = (byte)stream.ReadByte();
            irq_enable = (byte)stream.ReadByte();

            irq_counter = (int)(stream.ReadByte() << 24);
            irq_counter |= (int)(stream.ReadByte() << 16);
            irq_counter |= (int)(stream.ReadByte() << 8);
            irq_counter |= stream.ReadByte();
        }
    }
}
