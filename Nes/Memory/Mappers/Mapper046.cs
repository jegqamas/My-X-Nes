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
   
    class Mapper46 : IMapper
    {
        int[] reg = new int[4];
        CPUMemory map;
        public Mapper46(CPUMemory MAP)
        { map = MAP; }

        public void Write(ushort address, byte data)
        {
            if (address < 0x8000)
            {
                reg[0] = data & 0x0F;
                reg[1] = (data & 0xF0) >> 4;
                SetBank();
            }
            else
            {
                reg[2] = data & 0x01;
                reg[3] = (data & 0x70) >> 4;
                SetBank();
            }
        }

        public byte Read(ushort Address)
        {
            return 0;
        }

        public void SetUpMapperDefaults()
        {
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);
            SetBank();
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

        public bool ScanlineTimerNotPauseAtVBLANK
        {
            get { return false; }
        }
        void SetBank()
        {
            map.Switch8kPrgRom((reg[0] * 8 + reg[2] * 4 + 0) * 2, 0);
            map.Switch8kPrgRom((reg[0] * 8 + reg[2] * 4 + 1) * 2, 1);
            map.Switch8kPrgRom((reg[0] * 8 + reg[2] * 4 + 2) * 2, 2);
            map.Switch8kPrgRom((reg[0] * 8 + reg[2] * 4 + 3) * 2, 3);
            map.Switch1kChrRom(reg[1] * 64 + reg[3] * 8 + 0, 0);
            map.Switch1kChrRom(reg[1] * 64 + reg[3] * 8 + 1, 1);
            map.Switch1kChrRom(reg[1] * 64 + reg[3] * 8 + 2, 2);
            map.Switch1kChrRom(reg[1] * 64 + reg[3] * 8 + 3, 3);
            map.Switch1kChrRom(reg[1] * 64 + reg[3] * 8 + 4, 4);
            map.Switch1kChrRom(reg[1] * 64 + reg[3] * 8 + 5, 5);
            map.Switch1kChrRom(reg[1] * 64 + reg[3] * 8 + 6, 6);
            map.Switch1kChrRom(reg[1] * 64 + reg[3] * 8 + 7, 7);
        }
        public void SaveState(System.IO.Stream stream)
        {

        }
        public void LoadState(System.IO.Stream stream)
        {

        }
    }
}
