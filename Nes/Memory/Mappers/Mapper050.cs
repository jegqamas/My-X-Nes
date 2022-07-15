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
   
    class Mapper50 : IMapper
    {
        byte irq_enable = 0;
        CPUMemory map;
        public Mapper50(CPUMemory MAP)
        { map = MAP; }

        public void Write(ushort address, byte data)
        {
            if ((address & 0xE060) == 0x4020)
            {
                if ((address & 0x0100) == 0x0100)
                {
                    irq_enable = (byte)(data & 0x01);
                }
                else
                {
                    map.Switch8kPrgRom(((data & 0x08) | ((data & 0x01) << 2) | ((data & 0x06) >> 1)) * 2, 2);
                }
            }
        }

        public byte Read(ushort Address)
        {
            return 0;
        }

        public void SetUpMapperDefaults()
        {
            map.Switch8kPrgRomToSRAM(15 * 2);
            map.Switch8kPrgRom(8 * 2, 0);
            map.Switch8kPrgRom(9 * 2, 1);
            // map.Switch8kPrgRom(0, 2);
            map.Switch8kPrgRom(11 * 2, 3);
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);
        }

        public void TickScanlineTimer()
        {
            if (irq_enable != 0)
            {
                if (map.NES.PPU.ScanLine == map.NES.PPU.ScanlineOfEndOfVblank + 21)
                {
                    map.NES.CPU.SetIRQ(true);
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
            get { return true; }
        }

        public bool WriteUnder6000
        {
            get { return true; }
        }

        public bool ScanlineTimerNotPauseAtVBLANK
        {
            get { return true; }
        }

        public void SaveState(System.IO.Stream stream)
        {

        }
        public void LoadState(System.IO.Stream stream)
        {

        }
    }
}
