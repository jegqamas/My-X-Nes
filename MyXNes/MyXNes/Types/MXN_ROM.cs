/*********************************************************************\
*This file is part of My X Nes                                        *
*A Nintendo Entertainment System Emulator.                            *
*                                                                     *
*Copyright (C) 2010 - 2011 Ala Hadid                                  *
*E-mail: mailto:ahdsoftwares@hotmail.com                              *
*                                                                     *
*My X Nes is free software: you can redistribute it and/or modify     *
*it under the terms of the GNU General Public License as published by *
*the Free Software Foundation, either version 3 of the License, or    *
*(at your option) any later version.                                  *
*                                                                     *
*My X Nes is distributed in the hope that it will be useful,          *
*but WITHOUT ANY WARRANTY; without even the implied warranty of       *
*MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
*GNU General Public License for more details.                         *
*                                                                     *
*You should have received a copy of the GNU General Public License    *
*along with My X Nes.  If not, see <http://www.gnu.org/licenses/>.    *
\*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyXNes
{
    public class MXN_ROM
    {
        string _Name;
        string _Path;
        string _SnapShotPath;
        int _Mapper = 0;
        bool _IsSRAM = false;
        string _System = "NTSC";
        public string Name
        { get { return _Name; } set { _Name = value; } }
        public string Path
        { get { return _Path; } set { _Path = value; } }
        public string SnapShotPath
        { get { return _SnapShotPath; } set { _SnapShotPath = value; } }
        public int Mapper
        { get { return _Mapper; } set { _Mapper = value; } }
        public bool IsSRAM
        { get { return _IsSRAM; } set { _IsSRAM = value; } }
        public string System
        { get { return _System; } set { _System = value; } }
    }
}
