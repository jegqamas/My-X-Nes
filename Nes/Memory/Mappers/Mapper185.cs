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
   
    class Mapper185 : IMapper
    {
        CPUMemory map;
        byte patch = 0;
        public Mapper185(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            if (((patch == 0) && ((data & 0x03) == 0x03)) || ((patch == 1) && data == 0x21))
            {
                map.Switch8kChrRom(0);
            }
            else
            {
                byte fill=2;
                map.Switch1kChrRom(fill, 0);
                map.Switch1kChrRom(fill, 1);
                map.Switch1kChrRom(fill, 2);
                map.Switch1kChrRom(fill, 3);
                map.Switch1kChrRom(fill, 4);
                map.Switch1kChrRom(fill, 5);
                map.Switch1kChrRom(fill, 6);
                map.Switch1kChrRom(fill, 7);
            }
        }
        public void SetUpMapperDefaults()
        {
            map.Switch32kPrgRom(0);
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);
            if (map.cartridge.PRG_PAGES == 1)
            {
                map.Switch16kPrgRom(0, 1);
            }
            for (int i = 0; i < 1024; i++)
            {
                //map.cartridge.CHR[2][i] = 0xFF;
            }

            if (map.cartridge.CRC32 == -1208487807)
            {	// Spy vs Spy(J)
                patch = 1;
            }
        }
        public void TickScanlineTimer()
        {
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
            get { return false; }
        }

        public void SaveState(System.IO.Stream stream)
        {

        }
        public void LoadState(System.IO.Stream stream)
        {

        }
    }
}
