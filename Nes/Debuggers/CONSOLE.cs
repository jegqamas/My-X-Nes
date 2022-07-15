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
    public class CONSOLE
    {
        public static void WriteLine(object Sender, string Line, DebugStatus Status)
        {
            EventHandler<DebugArg> handler = DebugRised;
            if (handler != null)
            {
                handler(Sender, new DebugArg(Line, Status));
            }
        }
        public static void WriteSeparateLine(object Sender, DebugStatus Status)
        {
            EventHandler<DebugArg> handler = DebugRised;
            if (handler != null)
            {
                handler(Sender, new DebugArg("==========================", Status));
            }
        }
        /// <summary>
        /// Rised when the system write a debug
        /// </summary>
        public static event EventHandler<DebugArg> DebugRised;
    }
    public class DebugArg : EventArgs
    {
        public DebugArg(string Line, DebugStatus Status)
        {
            _DebugLine = Line;
            _Status = Status;
        }
        DebugStatus _Status;
        string _DebugLine;
        public DebugStatus Status
        { get { return _Status; } }
        public string DebugLine
        { get { return _DebugLine; } }
    }
    public enum DebugStatus
    {
        /// <summary>
        /// Color white
        /// </summary>
        None,
        /// <summary>
        /// Color green
        /// </summary>
        Cool,
        /// <summary>
        /// Color Yellow
        /// </summary>
        Warning,
        /// <summary>
        /// Color Red
        /// </summary>
        Error,
        /// <summary>
        /// Not colored, not for console
        /// </summary>
        Notification
    }
}
