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

namespace MyNes.Nes.Output.Video
{
    public interface IGraphicDevice
    {
        /// <summary>
        /// The name of this device
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The Description of this device
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Get the buffer ready and start, call this at scanline 0 
        /// (at the begining of the frame)
        /// </summary>
        void Begin();
        /// <summary>
        /// Being notified of a transparent pixel
        /// </summary>
        /// <param name="X">The pixel x coordinate</param>
        /// <param name="Y">The pixel y coordinate</param>
        void BlankPixel(int X, int Y);
        /// <summary>
        /// After the completion of the buffer, call this to present the buffer into the screen
        /// </summary>
        /// <param name="ScreenBuffer"></param>
        void RenderFrame(int[] ScreenBuffer);
        /// <summary>
        /// Take and save snapshot
        /// </summary>
        /// <param name="SnapPath">The path where to save the image</param>
        /// <param name="Format">The image format (e.g : bmp)</param>
        void TakeSnapshot(string SnapPath, string Format);
        /// <summary>
        /// Draw a text into the screen
        /// </summary>
        /// <param name="Text">The text to draw</param>
        void DrawText(string Text);
        /// <summary>
        /// Clear the screen (Black it !!)
        /// </summary>
        void Clear();
        /// <summary>
        /// Update the draw size if this video device is sizabel
        /// </summary>
        /// <param name="X">The x coordinate</param>
        /// <param name="Y">The y coordinate</param>
        /// <param name="W">The screen width</param>
        /// <param name="H">The screen height</param>
        void UpdateSize(int X, int Y, int W, int H);
        /// <summary>
        /// Get if this video device is sizable
        /// </summary>
        bool IsSizable { get; }
        /// <summary>
        /// Get if this video is currently rendering or not
        /// </summary>
        bool IsRendering { get; }
        /// <summary>
        /// Get or set if the video device can render a frame (ON/OFF)
        /// </summary>
        bool CanRender { get; set; }
        /// <summary>
        /// Get or set if it should run in Full Screen
        /// </summary>
        bool FullScreen { get; set; }
        /// <summary>
        /// Get a value idecate if this device supports Full Screen
        /// </summary>
        bool SupportFullScreen { get; }
        /// <summary>
        /// Turn off this device
        /// </summary>
        void Shutdown();
        /// <summary>
        /// Change this video mode settings
        /// </summary>
        void ChangeSettings();
    }
}
