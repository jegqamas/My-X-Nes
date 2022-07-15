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
    
    class Mapper42:IMapper
    {
        int irq_enable = 0;
	    int irq_counter = 0;
        CPUMemory map;
        public Mapper42(CPUMemory map)
        {
            this.map = map;
        }
        public void Write(ushort address, byte data)
        {
            if (address == 0x8000)
            { map.Switch8kChrRom(data * 8); }
            else if (address == 0xF000)
            { map.Switch8kPrgRomToSRAM((data & 0x0F) * 2); }
            else
            switch (address & 0xE003)
            {
                case 0xE000:
                    map.Switch8kPrgRomToSRAM((data & 0x0F) * 2);
                    break;

                case 0xE001:
                    if ((data & 0x08) == 0x08)
                        map.cartridge.Mirroring = Mirroring.Horizontal;
                    else
                        map.cartridge.Mirroring = Mirroring.Vertical;
                    break;

                case 0xE002:
                    if ((data & 0x02) == 0x02)
                    {
                        irq_enable = 0xFF;
                    }
                    else
                    {
                        irq_enable = 0;
                        irq_counter = 0;
                    }
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            map.Switch16kPrgRom((map.cartridge.PRG_PAGES - 2) * 4, 0);
            map.Switch16kPrgRom((map.cartridge.PRG_PAGES - 1) * 4, 1);
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
            if (irq_enable!=0)
            {
                if (irq_counter < 215)
                {
                    irq_counter++;
                }
                if (irq_counter == 215)
                {
                    irq_enable = 0;
                    map.cpu.SetIRQ(true);
                }
            }
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
            get { return true; }
        }
        public void SaveState(System.IO.Stream stream)
        {
            stream.WriteByte((byte)((irq_counter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_counter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_counter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_counter & 0x000000FF)));

            stream.WriteByte((byte)((irq_enable & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_enable & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_enable & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_enable & 0x000000FF)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            irq_counter = (int)(stream.ReadByte() << 24);
            irq_counter |= (int)(stream.ReadByte() << 16);
            irq_counter |= (int)(stream.ReadByte() << 8);
            irq_counter |= stream.ReadByte();

            irq_enable = (int)(stream.ReadByte() << 24);
            irq_enable |= (int)(stream.ReadByte() << 16);
            irq_enable |= (int)(stream.ReadByte() << 8);
            irq_enable |= stream.ReadByte();
        }
    }
}
