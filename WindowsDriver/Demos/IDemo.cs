#region LGPL License
/*
 * Physics 2D is a 2 Dimensional Rigid Body Physics Engine written in C#. 
 * For the latest info, see http://physics2d.sourceforge.net/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1fof the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 * 
 */
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Physics2D;
using AdvanceMath;
using System.Drawing;
using global::SdlDotNet;

namespace WindowsDriver.Demos
{
    public interface IDemo
    {
        string Name {get;}
        string Description { get;}
        string Instructions { get;}
        Vector2D CameraPosition { get;}
        float Scale {get;}
        World2D World2D {get;}
        Color BackgroundColor { get;set;}

        void DrawGraphics(Size ViewableAreaSize);


        bool Update(float dt);
        void UpdateAI(float dt);
        void UpdateAI(object dt);
        void UpdateKeyBoard(KeyboardState keys,float dt);
        void InitObjects();
        void AddObjects();
        IDemo CreateNew();

    }
}
