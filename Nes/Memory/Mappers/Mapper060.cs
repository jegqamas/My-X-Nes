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
using System.IO;
namespace MyNes.Nes
{
   
    class Mapper60 : IMapper
    {
        CPUMemory map;
        public Mapper60(CPUMemory MAP)
        { map = MAP; }
        byte patch = 0;
        byte game_sel = 0;
        public void Write(ushort address, byte data)
        {
            if (patch == 1)
            {
                if ((address & 0x80) == 0x80)
                {
                    map.Switch16kPrgRom(((address & 0x70) >> 4) * 4, 0);
                    map.Switch16kPrgRom(((address & 0x70) >> 4) * 4, 1);
                }
                else
                {
                    map.Switch32kPrgRom(((address & 0x70) >> 5) * 8);
                }

                map.Switch8kChrRom((address & 0x07) * 8);

                if ((data & 0x08) == 0x08)
                    map.cartridge.Mirroring = Mirroring.Vertical;
                else
                    map.cartridge.Mirroring = Mirroring.Horizontal;
            }
        }

        public byte Read(ushort Address)
        {
            return 0;
        }

        public void SetUpMapperDefaults()
        {
            map.IsSoftResetTrigger = true;
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            string name = map.cartridge.RomPath;

            if (name.Contains("Reset Based 4-in-1"))
            {
                map.Switch16kPrgRom(game_sel * 4, 0);
                map.Switch16kPrgRom(game_sel * 4, 1);
                map.Switch8kChrRom(game_sel * 8);

            }
            else
            {
                patch = 1;
                map.Switch32kPrgRom(0);
                map.Switch8kChrRom(0);
            }

        }

        public void TickScanlineTimer()
        {

        }

        public void TickCycleTimer(int cycles)
        {

        }

        public void SoftReset()
        {
            game_sel++;
            game_sel &= 3;
        }

        public bool WriteUnder8000
        {
            get { return false; }
        }

        public bool WriteUnder6000
        {
            get { return false; }
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
