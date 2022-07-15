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
   
    class Mapper40 : IMapper
    {
        CPUMemory map;
        bool irq_enable = false;
        int irq_counter = 0;
        int irq_line = 0;
        public Mapper40(CPUMemory map)
        { this.map = map; }
        public void Write(ushort address, byte data)
        {
            switch (address & 0xE000)
            {
                case 0x8000: irq_enable = false; break;
                case 0xA000: irq_enable = true; irq_line = 0; break;
                case 0xE000: map.Switch8kPrgRom((data & 0x07) * 2, 2); break;
            }
        }
        public void SetUpMapperDefaults()
        {
            //map.Switch32kPrgRom(0);
            map.Switch8kPrgRomToSRAM(12);
            map.Switch8kPrgRom(8, 0);
            map.Switch8kPrgRom(10, 1);
            map.Switch8kPrgRom(14, 3);

            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
          
        }
        public void TickCycleTimer(int cycles)
        {
            irq_counter += cycles;
            if (irq_counter >= 113)
            {
                irq_counter = 0;
                if (irq_enable)
                {
                    irq_line++;
                    if (irq_line % 37 == 0)
                    {
                        map.cpu.SetIRQ(true);
                    }
                }
            }
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
            stream.WriteByte((byte)(irq_enable ? 1 : 0));

            stream.WriteByte((byte)((irq_counter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_counter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_counter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_counter & 0x000000FF)));

            stream.WriteByte((byte)((irq_line & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_line & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_line & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_line & 0x000000FF)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            irq_enable = stream.ReadByte() == 1;

            irq_counter = (int)(stream.ReadByte() << 24);
            irq_counter |= (int)(stream.ReadByte() << 16);
            irq_counter |= (int)(stream.ReadByte() << 8);
            irq_counter |= stream.ReadByte();

            irq_line = (int)(stream.ReadByte() << 24);
            irq_line |= (int)(stream.ReadByte() << 16);
            irq_line |= (int)(stream.ReadByte() << 8);
            irq_line |= stream.ReadByte();
        }
    }
}
