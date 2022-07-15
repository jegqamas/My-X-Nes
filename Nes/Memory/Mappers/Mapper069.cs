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
   
    class Mapper69 : IMapper
    {
        CPUMemory Map;
        public ushort reg = 0;
        public short timer_irq_counter_69 = 0;
        public bool timer_irq_enabled;
        byte ss5bAddr = 0;
        SS5BExternalUnit externalUnit;
        public Mapper69(CPUMemory MAP)
        { Map = MAP; }
        public void Write(ushort address, byte data)
        {
            switch (address & 0xE000)
            {
                case 0x8000:
                    reg = data;
                    break;
                case 0xA000:
                    switch (reg & 0x0F)
                    {
                        case 0x00: Map.Switch1kChrRom(data, 0); break;
                        case 0x01: Map.Switch1kChrRom(data, 1); break;
                        case 0x02: Map.Switch1kChrRom(data, 2); break;
                        case 0x03: Map.Switch1kChrRom(data, 3); break;
                        case 0x04: Map.Switch1kChrRom(data, 4); break;
                        case 0x05: Map.Switch1kChrRom(data, 5); break;
                        case 0x06: Map.Switch1kChrRom(data, 6); break;
                        case 0x07: Map.Switch1kChrRom(data, 7); break;
                        case 0x08:
                            if ((data & 0x40) == 0)
                            {
                                Map.Switch8kPrgRomToSRAM((data & 0x3F) * 2);
                            }
                            break;
                        case 0x09:
                            Map.Switch8kPrgRom(data * 2, 0);
                            break;
                        case 0x0A:
                            Map.Switch8kPrgRom(data * 2, 1);
                            break;
                        case 0x0B:
                            Map.Switch8kPrgRom(data * 2, 2);
                            break;

                        case 0x0C:
                            data &= 0x03;
                            if (data == 0) Map.cartridge.Mirroring = Mirroring.Vertical;
                            if (data == 1) Map.cartridge.Mirroring = Mirroring.Horizontal;
                            if (data == 2)
                            {
                                Map.cartridge.Mirroring = Mirroring.One_Screen;
                                Map.cartridge.MirroringBase = 0x2000;
                            }
                            if (data == 3)
                            {
                                Map.cartridge.Mirroring = Mirroring.One_Screen;
                                Map.cartridge.MirroringBase = 0x2400;
                            }
                            break;

                        case 0x0D:
                            if (data == 0)
                                timer_irq_enabled = false;
                            if (data == 0x81)
                                timer_irq_enabled = true;
                            break;

                        case 0x0E:
                            timer_irq_counter_69 = (short)((timer_irq_counter_69 & 0xFF00) | data);

                            break;

                        case 0x0F:
                            timer_irq_counter_69 = (short)((timer_irq_counter_69 & 0x00FF) | (data << 8));
                            break;
                    }
                    break;
                //External sound...
                case 0xC000: ss5bAddr = data; break;
                case 0xE000:
                    switch (ss5bAddr)
                    {
                        case 00: externalUnit.Wave1.Poke1(address, data); break;
                        case 01: externalUnit.Wave1.Poke2(address, data); break;
                        case 02: externalUnit.Wave2.Poke1(address, data); break;
                        case 03: externalUnit.Wave2.Poke2(address, data); break;
                        case 04: externalUnit.Wave3.Poke1(address, data); break;
                        case 05: externalUnit.Wave3.Poke2(address, data); break;
                        case 06: break; // Skipped
                        case 07:
                            externalUnit.Wave1.Enabled = (data & 0x01) == 0;
                            externalUnit.Wave2.Enabled = (data & 0x02) == 0;
                            externalUnit.Wave3.Enabled = (data & 0x04) == 0;
                            break;

                        case 08: externalUnit.Wave1.Poke3(address, data); break;
                        case 09: externalUnit.Wave2.Poke3(address, data); break;
                        case 10: externalUnit.Wave3.Poke3(address, data); break;
                    }
                    break;
            }

        }
        public void SetUpMapperDefaults()
        {
            Map.NES.APU.External = externalUnit = new SS5BExternalUnit(Map.NES);
            Map.Switch8kPrgRom(((Map.cartridge.PRG_PAGES * 2) - 1) * 2, 3);
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
                if (timer_irq_counter_69 > 0)
                    timer_irq_counter_69 -= (short)cycles;
                else
                {
                    Map.cpu.SetIRQ(true);
                    timer_irq_enabled = false;
                }
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
            stream.WriteByte((byte)((reg & 0xFF00) >> 8));
            stream.WriteByte((byte)((reg & 0x00FF)));
            stream.WriteByte((byte)((timer_irq_counter_69 & 0xFF00) >> 8));
            stream.WriteByte((byte)((timer_irq_counter_69 & 0x00FF)));
            stream.WriteByte((byte)(timer_irq_enabled?1:0));
            stream.WriteByte(ss5bAddr);
        }
        public void LoadState(System.IO.Stream stream)
        {
            reg = (ushort)(stream.ReadByte() << 8);
            reg |= (ushort)stream.ReadByte();
            timer_irq_counter_69 = (short)(stream.ReadByte() << 8);
            timer_irq_counter_69 |= (short)stream.ReadByte();
            timer_irq_enabled |= stream.ReadByte()==1;
            ss5bAddr = (byte)stream.ReadByte();
        }
    }
}
