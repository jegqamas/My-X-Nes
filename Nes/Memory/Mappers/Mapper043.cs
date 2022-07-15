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
   
    class Mapper43 : IMapper
    {
        //NOT COMPLETED
        CPUMemory map;
        public Mapper43(CPUMemory mem)
        { map = mem; }
        public void Write(ushort address, byte data)
        {
            bool mode = (address & 0x0800) == 0x0800;
            //switch
            if (((address & 0x0300) >> 8) == 0)//ship 0, 1024K
            {
                if ((address & 0x0800) == 0)//32k mode
                    map.Switch32kPrgRom((address & 0xFF) * 8);
                else//16k mode
                    map.Switch16kPrgRom((address & 0xFF) * 4, ((address & 0x1000) >> 12));
            }
            else if (((address & 0x0300) >> 8) == 1)//ship 1, 512k
            {
                if ((address & 0x0800) == 0)
                    map.Switch32kPrgRom(255 + ((address & 0xF) * 8));
                else
                    map.Switch16kPrgRom(255 + ((address & 0xF) * 4), ((address & 0x1000) >> 12));
            }
            //apply mirroring
            if ((address & 0x2000) == 0x2000)
                map.cartridge.Mirroring = Mirroring.Horizontal;
            else
                map.cartridge.Mirroring = Mirroring.Vertical;
        }
        public byte Read(ushort Address)
        {
            return 0;
        }
        public void SetUpMapperDefaults()
        {
            map.Switch32kPrgRom(0);
            if (map.cartridge.IsVRAM)
                map.FillCHR(32);
            map.Switch8kChrRom(0);
            CONSOLE.WriteLine(this, "MAPPER # 43 IS NOT COMPLETED !!", DebugStatus.Error);
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
