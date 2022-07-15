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

    class Mapper65 : IMapper
    {
        CPUMemory Map;
        short timer_irq_counter_65 = 0;
        short timer_irq_Latch_65 = 0;
        bool timer_irq_enabled;

        public Mapper65(CPUMemory Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x8000: Map.Switch8kPrgRom(data * 2, 0); break;

                case 0x9000:

                    if ((data & 0x40) == 0x40)
                        Map.cartridge.Mirroring = Mirroring.Vertical;
                    else
                        Map.cartridge.Mirroring = Mirroring.Horizontal;
                    break;

                case 0x9003: timer_irq_enabled = ((data & 0x80) != 0); Map.cpu.SetIRQ(false); break;
                case 0x9004: timer_irq_counter_65 = timer_irq_Latch_65; break;
                case 0x9005: timer_irq_Latch_65 = (short)((timer_irq_Latch_65 & 0x00FF) | (data << 8)); Map.cpu.SetIRQ(false); break;
                case 0x9006: timer_irq_Latch_65 = (short)((timer_irq_Latch_65 & 0xFF00) | (data)); break;

                case 0xB000:
                case 0xB001:
                case 0xB002:
                case 0xB003:
                case 0xB004:
                case 0xB005:
                case 0xB006:
                case 0xB007: Map.Switch1kChrRom(data, address & 0x7); break;

                case 0xA000: Map.Switch8kPrgRom(data * 2, 1); break;
                case 0xC000: Map.Switch8kPrgRom(data * 2, 2); break;
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom(0, 0);
            Map.Switch16kPrgRom((Map.cartridge.PRG_PAGES - 1) * 4, 1);
            if (Map.cartridge.IsVRAM)
                Map.FillCHR(16);
            Map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer(int cycles)
        {
            if (timer_irq_enabled)
            {

                if (timer_irq_counter_65 <= 0)
                {
                    Map.cpu.SetIRQ(true);
                }
                else
                    timer_irq_counter_65 -= (short)cycles;
            }
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
            get { return false; }
        }
        public void SaveState(System.IO.Stream stream)
        {
            stream.WriteByte((byte)((timer_irq_counter_65 & 0xFF00) >> 8));
            stream.WriteByte((byte)((timer_irq_counter_65 & 0x00FF)));
            stream.WriteByte((byte)((timer_irq_Latch_65 & 0xFF00) >> 8));
            stream.WriteByte((byte)((timer_irq_Latch_65 & 0x00FF)));
            stream.WriteByte((byte)((timer_irq_enabled ? 1 : 0)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            timer_irq_counter_65 = (short)(stream.ReadByte() << 8);
            timer_irq_counter_65 |= (short)stream.ReadByte();
            timer_irq_Latch_65 = (short)(stream.ReadByte() << 8);
            timer_irq_Latch_65 |= (short)stream.ReadByte();
            timer_irq_enabled = stream.ReadByte() == 1;
        }
    }
}
