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
using MyNes.Nes.Output.Video;
using MyNes.Nes;

namespace MyXNes
{
    class Vid_XNA : IGraphicDevice
    {
        public int[] BUFFER = new int[240 * 256];
        int firstToCut = 0;

        public Vid_XNA(TVFORMAT tv)
        {
            if (tv == TVFORMAT.NTSC)
            {
                BUFFER = new int[224 * 256];
                firstToCut = 8;
            }
        }
        public void Begin()
        {

        }

        public void BlankPixel(int X, int Y)
        {

        }

        public bool CanRender
        {
            get
            {
                return true;
            }
            set
            {

            }
        }

        public void Clear()
        {

        }

        public string Description
        {
            get { return ""; }
        }

        public void DrawText(string Text, int Frames)
        {

        }

        public bool FullScreen
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public bool IsRendering
        {
            get { return false; }
        }

        public bool IsSizable
        {
            get { return false; }
        }

        public string Name
        {
            get { return ""; }
        }

        public void RenderFrame(int[] ScreenBuffer)
        {
          	for (int i = (256 * firstToCut); i < ScreenBuffer.Length -
				(256 * firstToCut); i++)
            {
                byte R = (byte)((ScreenBuffer[i] & 0x00FF0000) >> 16);
                byte G = (byte)((ScreenBuffer[i] & 0x0000FF00) >> 8);
                byte B = (byte)((ScreenBuffer[i] & 0x000000FF));
                BUFFER[i - (256 * firstToCut)] = (0xFF << 24) | (B << 16) | (G << 8) | R;
            }
        }

        public void Shutdown()
        {

        }

        public bool SupportFullScreen
        {
            get { return true; }
        }

        public void TakeSnapshot(string SnapPath, string Format)
        {

        }

        public void UpdateSize(int X, int Y, int W, int H)
        {

        }


        public void DrawText(string Text)
        {

        }

        public void ChangeSettings()
        {

        }
    }
}
