#region MIT License
/*
 * Copyright (c) 2005-2008 Jonathan Mark Porter. http://physics2d.googlepages.com/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
#endregion

#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;
using System.Runtime.InteropServices;
using AdvanceMath;
using Tao.OpenGl;
using SdlDotNet.Graphics;
namespace Graphics2DDotNet
{



    /// <summary>
    /// The texture minifying function is used whenever
    /// the	pixel being textured maps to an	area greater
    /// than one texture element. There are	six defined
    /// minifying functions.  Two of them use the nearest
    /// one	or nearest four	texture	elements to compute
    /// the	texture	value. The other four use mipmaps.
    /// 
    /// A mipmap is	an ordered set of arrays representing
    /// the	same image at progressively lower resolutions.
    /// If the texture has dimensions 2nx2m, there are
    /// max(n,m)+1 mipmaps.	The first mipmap is the
    /// original texture, with dimensions 2nx2m. Each
    /// subsequent mipmap has dimensions 2k-1x2l-1,	where
    /// 2kx2l are the dimensions of	the previous mipmap,
    /// until either k=0 or	l=0.  At that point,
    /// subsequent mipmaps have dimension 1x2l-1 or	2k-1x1
    /// until the final mipmap, which has dimension	1x1.
    /// To define the mipmaps, call	glTexImage1D,
    /// glTexImage2D, glCopyTexImage1D, or
    /// glCopyTexImage2D with the level argument
    /// indicating the order of the	mipmaps.  Level	0 is
    /// the	original texture; level	max(n,m) is the	final
    /// 1x1	mipmap.
    /// </summary>
    public enum MinifyingOption : int
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,
        /// <summary>
        /// Returns the value	of the texture element
        ///	that is nearest (in Manhattan distance)
        ///	to the center of the pixel being
        ///	textured.
        /// </summary>
        Nearest = Gl.GL_NEAREST,
        /// <summary>
        /// Returns the weighted average of the four
        /// texture elements that are closest	to the
        /// center of the pixel being textured.
        /// These can include border texture
        /// elements, depending on the values	of
        /// GL_TEXTURE_WRAP_S and GL_TEXTURE_WRAP_T,
        /// and on the exact mapping.
        /// </summary>
        Linear = Gl.GL_LINEAR,
        /// <summary>
        /// Chooses the mipmap that most closely
        /// matches the size of the pixel being
        /// textured and uses the GL_NEAREST
        /// criterion (the texture element nearest
        /// to the center of the pixel) to produce a
        /// texture value.
        /// </summary>
        NearestMipMapNearest = Gl.GL_NEAREST_MIPMAP_NEAREST,
        /// <summary>
        /// Chooses the mipmap that most closely
        /// matches the size of the pixel being
        /// textured and uses the GL_LINEAR
        /// criterion (a weighted average of the
        /// four texture elements that are closest
        /// to the center of the pixel) to produce a
        /// texture value.
        /// </summary>
        LinearMipMapNearest = Gl.GL_LINEAR_MIPMAP_NEAREST,
        /// <summary>
        /// Chooses the two mipmaps that most
        /// closely match the	size of	the pixel
        /// being textured and uses the GL_NEAREST
        /// criterion	(the texture element nearest
        /// to the center of the pixel) to produce a
        /// texture value from each mipmap. The
        /// final texture value is a weighted
        /// average of those two values.
        /// </summary>
        NearestMipMapLinear = Gl.GL_NEAREST_MIPMAP_LINEAR,
        /// <summary>
        /// Chooses the two mipmaps that most
        /// closely match the size of the pixel
        /// being textured and uses the GL_LINEAR
        /// criterion (a weighted average of the
        /// four texture elements that are closest
        /// to the center of the pixel) to produce a
        /// texture value from each mipmap. The
        /// final texture value is a weighted
        /// average of those two values.
        /// </summary>
        LinearMipMapLinear = Gl.GL_LINEAR_MIPMAP_LINEAR
    }
    /// <summary>
    /// The texture magnification function is used when
    /// the	pixel being textured maps to an	area less than
    /// or equal to	one texture element.  It sets the
    /// texture magnification function to either
    /// GL_NEAREST or GL_LINEAR. GL_NEAREST is
    /// generally faster than GL_LINEAR, but it can
    /// produce textured images with sharper edges because
    /// the	transition between texture elements is not as
    /// smooth. The	initial	value of GL_TEXTURE_MAG_FILTER
    /// is GL_LINEAR.
    /// </summary>
    public enum MagnificationOption : int
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,
        /// <summary>
        /// Returns the value	of the texture element
        ///	that is nearest (in Manhattan distance)
        ///	to the center of the pixel being
        ///	textured.
        /// </summary>
        Nearest = Gl.GL_NEAREST,
        /// <summary>
        /// Returns the weighted average of the four
        /// texture elements that are closest	to the
        /// center of the pixel being textured.
        /// These can include border texture
        /// elements, depending on the values	of
        /// GL_TEXTURE_WRAP_S and GL_TEXTURE_WRAP_T,
        /// and on the exact mapping.
        /// </summary>
        Linear = Gl.GL_LINEAR
    }
    /// <summary>
    /// The wrap parameter for a texture coordinate
    /// </summary>
    public enum WrapOption : int
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,
        /// <summary>
        /// Causes texture coordinates to be clamped to the range [0,1] and
        /// is useful for preventing wrapping artifacts	when
        /// mapping a single image onto	an object.
        /// </summary>
        Clamp = Gl.GL_CLAMP,
        /// <summary>
        /// Causes texture coordinates to loop around so to remain in the 
        /// range [0,1] where 1.5 would be .5. this is useful for repeating
        /// a texture for a tiled floor.
        /// </summary>
        Repeat = Gl.GL_REPEAT
    }

    public class TextureHelper
    {
        public static int GetPower(int value)
        {
            int index = 30;
            for (; (1 << index) >= value && index > -2; --index) { }
            return 1 << index + 1;
        }


        private static Surface TransformSurface(Surface surface, bool isFlipped)
        {
            byte alpha = surface.Alpha;
            Surface textureSurface2 = null;
            surface.Alpha = 0;
            try
            {
                Surface textureSurface = surface;
                if (isFlipped)
                {
                    textureSurface = textureSurface.CreateFlippedVerticalSurface();
                    textureSurface2 = textureSurface;
                    textureSurface2.Alpha = 0;
                }
                return textureSurface.CreateResizedSurface();
            }
            finally
            {
                surface.Alpha = alpha;
                if (isFlipped)
                {
                    textureSurface2.Dispose();
                }
            }
        }

        public static int LoadTexture2D(
            Surface surface,
            bool flip,
            TextureOptions options)
        {
            if (surface == null) { throw new ArgumentNullException("surface"); }
            if (options == null) { throw new ArgumentNullException("options"); }
            int textureID;
            using (Surface textureSurface = TransformSurface(surface, flip))
            {
                Gl.glGenTextures(1, out textureID);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureID);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, (int)options.MinifyingFilter);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, (int)options.MagnificationFilter);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, (int)options.WrapS);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, (int)options.WrapT);
                if (options.MinifyingFilter == MinifyingOption.Linear ||
                    options.MinifyingFilter == MinifyingOption.Nearest)
                {
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, textureSurface.BytesPerPixel, textureSurface.Width, textureSurface.Height, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, textureSurface.Pixels);
                }
                else
                {
                    Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, textureSurface.BytesPerPixel, textureSurface.Width, textureSurface.Height, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, textureSurface.Pixels);
                }
            }
            return textureID;
        }
        public static int[] LoadTexture2DRange(
             Surface surface,
             bool flip,
             TextureOptions[] options)
        {
            if (surface == null) { throw new ArgumentNullException("surface"); }
            if (options == null) { throw new ArgumentNullException("options"); }
            for (int index = 0; index < options.Length; ++index)
            {
                if (options[index] == null) { throw new ArgumentNullException("options"); }
            }
            int[] textureIDs = new int[options.Length];
            using (Surface textureSurface = TransformSurface(surface, flip))
            {
                Gl.glGenTextures(options.Length, textureIDs);
                for (int index = 0; index < options.Length; ++index)
                {
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureIDs[index]);
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, (int)options[index].MinifyingFilter);
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, (int)options[index].MagnificationFilter);
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, (int)options[index].WrapS);
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, (int)options[index].WrapT);
                    if (options[index].MinifyingFilter == MinifyingOption.Linear ||
                        options[index].MinifyingFilter == MinifyingOption.Nearest)
                    {
                        Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, textureSurface.BytesPerPixel, textureSurface.Width, textureSurface.Height, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, textureSurface.Pixels);
                    }
                    else
                    {
                        Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, textureSurface.BytesPerPixel, textureSurface.Width, textureSurface.Height, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, textureSurface.Pixels);
                    }
                }
            }
            return textureIDs;
        }

        private static void ScaleTo1(ref Vector3D vector)
        {
            vector = vector.Normalized * .5f + new Vector3D(.5f, .5f, .5f);
        }
        public static int GenNormalizationCubeMap(int size)
        {
            int texid;
            Gl.glGenTextures(1, out texid);
            Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, texid);

            byte[] data = new byte[size * size * 3];

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();

                Scalar offset = 0.5f;
                Scalar halfSize = size * 0.5f;
                int bytePtr = 0;
                Vector3D vector;
                for (int j = 0; j < size; j++)
                {
                    for (int i = 0; i < size; i++)
                    {
                        vector.X = halfSize;
                        vector.Y = (j + offset - halfSize);
                        vector.Z = -(i + offset - halfSize);
                        ScaleTo1(ref vector);
                        data[bytePtr] = (byte)(vector.X * 255.0f);
                        data[bytePtr + 1] = (byte)(vector.Y * 255.0f);
                        data[bytePtr + 2] = (byte)(vector.Z * 255.0f);

                        bytePtr += 3;
                    }
                }
                Gl.glTexImage2D(Gl.GL_TEXTURE_CUBE_MAP_POSITIVE_X,
                        0, Gl.GL_RGB8, size, size, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, ptr);

                bytePtr = 0;
                for (int j = 0; j < size; j++)
                {
                    for (int i = 0; i < size; i++)
                    {
                        vector.X = -halfSize;
                        vector.Y = (j + offset - halfSize);
                        vector.Z = (i + offset - halfSize);
                        ScaleTo1(ref vector);

                        data[bytePtr] = (byte)(vector.X * 255.0f);
                        data[bytePtr + 1] = (byte)(vector.Y * 255.0f);
                        data[bytePtr + 2] = (byte)(vector.Z * 255.0f);

                        bytePtr += 3;
                    }
                }
                Gl.glTexImage2D(Gl.GL_TEXTURE_CUBE_MAP_NEGATIVE_X,
                        0, Gl.GL_RGB8, size, size, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, ptr);

                bytePtr = 0;
                for (int j = 0; j < size; j++)
                {
                    for (int i = 0; i < size; i++)
                    {
                        vector.X = i + offset - halfSize;
                        vector.Y = -halfSize;
                        vector.Z = j + offset - halfSize;
                        ScaleTo1(ref vector);

                        data[bytePtr] = (byte)(vector.X * 255.0f);
                        data[bytePtr + 1] = (byte)(vector.Y * 255.0f);
                        data[bytePtr + 2] = (byte)(vector.Z * 255.0f);

                        bytePtr += 3;
                    }
                }
                Gl.glTexImage2D(Gl.GL_TEXTURE_CUBE_MAP_POSITIVE_Y,
                        0, Gl.GL_RGB8, size, size, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, ptr);

                bytePtr = 0;
                for (int j = 0; j < size; j++)
                {
                    for (int i = 0; i < size; i++)
                    {
                        vector.X = i + offset - halfSize;
                        vector.Y = halfSize;
                        vector.Z = -(j + offset - halfSize);
                        ScaleTo1(ref vector);

                        data[bytePtr] = (byte)(vector.X * 255.0f);
                        data[bytePtr + 1] = (byte)(vector.Y * 255.0f);
                        data[bytePtr + 2] = (byte)(vector.Z * 255.0f);

                        bytePtr += 3;
                    }
                }
                Gl.glTexImage2D(Gl.GL_TEXTURE_CUBE_MAP_NEGATIVE_Y,
                        0, Gl.GL_RGB8, size, size, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, ptr);

                bytePtr = 0;
                for (int j = 0; j < size; j++)
                {
                    for (int i = 0; i < size; i++)
                    {
                        vector.X = i + offset - halfSize;
                        vector.Y = (j + offset - halfSize);
                        vector.Z = halfSize;
                        ScaleTo1(ref vector);

                        data[bytePtr] = (byte)(vector.X * 255.0f);
                        data[bytePtr + 1] = (byte)(vector.Y * 255.0f);
                        data[bytePtr + 2] = (byte)(vector.Z * 255.0f);

                        bytePtr += 3;
                    }
                }
                Gl.glTexImage2D(Gl.GL_TEXTURE_CUBE_MAP_POSITIVE_Z,
                        0, Gl.GL_RGB8, size, size, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, ptr);

                bytePtr = 0;
                for (int j = 0; j < size; j++)
                {
                    for (int i = 0; i < size; i++)
                    {
                        vector.X = -(i + offset - halfSize);
                        vector.Y = (j + offset - halfSize);
                        vector.Z = -halfSize;
                        ScaleTo1(ref vector);

                        data[bytePtr] = (byte)(vector.X * 255.0f);
                        data[bytePtr + 1] = (byte)(vector.Y * 255.0f);
                        data[bytePtr + 2] = (byte)(vector.Z * 255.0f);

                        bytePtr += 3;
                    }
                }
                Gl.glTexImage2D(Gl.GL_TEXTURE_CUBE_MAP_NEGATIVE_Z,
                        0, Gl.GL_RGB8, size, size, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, ptr);



                Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE);
                Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE);
                Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_WRAP_R, Gl.GL_CLAMP_TO_EDGE);
                return texid;
            }
            finally
            {
                handle.Free();
            }
        }
    }
    public class TextureOptions
    {
        MinifyingOption minifyingFilter = MinifyingOption.Linear;
        MagnificationOption magnificationFilter = MagnificationOption.Linear;
        WrapOption wrapS = WrapOption.Clamp;
        WrapOption wrapT = WrapOption.Clamp;
        public MinifyingOption MinifyingFilter
        {
            get { return minifyingFilter; }
            set { minifyingFilter = value; }
        }
        public MagnificationOption MagnificationFilter
        {
            get { return magnificationFilter; }
            set { magnificationFilter = value; }
        }
        public WrapOption WrapS
        {
            get { return wrapS; }
            set { wrapS = value; }
        }
        public WrapOption WrapT
        {
            get { return wrapT; }
            set { wrapT = value; }
        }
    }
    public sealed class SpriteDrawable : BufferedDrawable
    {
        ARBArrayBuffer<Vector2D> vertexes;
        ARBArrayBuffer<Vector2D> coordinates;
        Texture2D texture;
        ScalarColor4 color;

        public SpriteDrawable(Surface surface, Vector2D[] vertexes, Vector2D[] coordinates)
            :this(surface,vertexes,coordinates,true,new TextureOptions())
        {
        }
        public SpriteDrawable(Surface surface, Vector2D[] vertexes, Vector2D[] coordinates, bool flip, TextureOptions options)
        {
            this.vertexes = new ARBArrayBuffer<Vector2D>(vertexes, Vector2D.Size);
            this.coordinates = new ARBArrayBuffer<Vector2D>(coordinates, Vector2D.Size);
            this.texture = new Texture2D(surface, flip, options);
            this.color = new ScalarColor4(1, 1, 1, 1);
        }
        public ScalarColor4 Color
        {
            get { return color; }
            set { color = value; }
        }
        protected override void EnableState()
        {
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glEnableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
        }
        protected override void DisableState()
        {
            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glDisableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
            Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);
        }
        protected override void BufferData(int refresh)
        {
            vertexes.Buffer(refresh);
            coordinates.Buffer(refresh);
            texture.Buffer(refresh);
        }
        protected override void DrawData(DrawInfo drawInfo, IDrawableState state)
        {
            texture.Bind();
            vertexes.Bind();
            Gl.glVertexPointer(Vector2D.Count, GlHelper.GlScalar, 0, IntPtr.Zero);

            coordinates.Bind();
            Gl.glTexCoordPointer(Vector2D.Count, GlHelper.GlScalar, 0, IntPtr.Zero);

            GlHelper.GlColor4(color.Red, color.Green, color.Blue, color.Alpha);

            Gl.glDrawArrays(Gl.GL_QUADS, 0, vertexes.Length);
        }
        public override IDrawableState CreateState()
        {
            return null;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                texture.Dispose();
                vertexes.Dispose();
                coordinates.Dispose();
            }
        }
    }


}