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
using AdvanceMath.Geometry2D;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using Tao.OpenGl;
using SdlDotNet.Graphics;
using Color = System.Drawing.Color;
namespace Graphics2DDotNet
{

    public class Light
    {
        public Vector3D Position;
    }

    public class BumpmapSpriteDrawable : BufferedDrawable
    {
        public static Surface Createbumpmap(Surface original,Vector2D offset, IShape shape, Scalar depthToCenter)
        {

            int width = original.Width;
            int height = original.Height;
            //Color[,] colors = new Color[width, height];

            Vector3D mid = new Vector3D(128,128,128);
            Vector3D min = Vector3D.Zero;
            Vector3D max = new Vector3D(255,255,255);
            Surface result = new Surface(width, height,32,true);
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    Vector2D point = new Vector2D(x, y) -  offset;
                    IntersectionInfo info;
                    Vector3D normal;
                    if (shape.TryGetIntersection(point, out info))
                    {
                        Scalar va = Math.Abs(info.Distance / depthToCenter);
                        if (va < 1)
                        {
                            Vector3D temp = new Vector3D(info.Normal.X, info.Normal.Y, .1f);
                            temp = Vector3D.Lerp(temp, Vector3D.ZAxis, va);
                            normal = temp.Normalized;
                        }
                        else
                        {
                            normal = Vector3D.ZAxis;
                        }
                    }
                    else
                    {
                        normal = Vector3D.ZAxis;
                    }
                    normal = Vector3D.Clamp(mid + (normal * 128), min, max);

                    result.Draw(
                        new System.Drawing.Point(x,  y), 
                        Color.FromArgb(255, (int)normal.X, (int)normal.Y, (int)normal.Z));
                    // colors[x, y] = Color.FromArgb(255,(int)normal.X, (int)normal.Y, (int)normal.Z); 
                }
            }



           // result.SetPixels(new System.Drawing.Point(), colors);
            return result;
        }


        bool yInverted;
        bool xInverted;
        static Scalar[] GlMatrix = new Scalar[16];
        Light light;
        ARBArrayBuffer<Vector2D> vertexes;
        ARBArrayBuffer<Vector2D> coordinates;
        Texture2D bumpmap, sprite;
        int size;
        int normalization_cube_map;
        public BumpmapSpriteDrawable(
            Surface surface, 
            Surface bumpmap, 
            Vector2D[] vertexes, Vector2D[] coordinates,
            bool flip, bool xInverted, bool yInverted,
            Light light)
        {
            this.light = light;
            this.size = Math.Max(surface.Width, surface.Height);
            this.xInverted = xInverted;
            if (flip)
            {
                this.yInverted = !yInverted;
            }
            else
            {
                this.yInverted = yInverted;
            }
            this.vertexes = new ARBArrayBuffer<Vector2D>(vertexes, Vector2D.Size);
            this.coordinates = new ARBArrayBuffer<Vector2D>(coordinates, Vector2D.Size);
            this.bumpmap = new Texture2D(bumpmap, flip, new TextureOptions());
            this.sprite = new Texture2D(surface, flip, new TextureOptions());
        }
        ~BumpmapSpriteDrawable()
        {
            Dispose(false);
        }

        protected override void EnableState()
        {
            Gl.glPushAttrib(unchecked((int)0xffffffff));

            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glEnableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
        }
        protected override void DisableState()
        {
            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
            Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glPopAttrib();
        }

        protected override void BufferData(int refresh)
        {
            normalization_cube_map = TextureHelper.GenNormalizationCubeMap(size);

            bumpmap.Buffer(refresh);
            sprite.Buffer(refresh);
            vertexes.Buffer(refresh);
            coordinates.Buffer(refresh);
        }
        protected override void DrawData(DrawInfo drawInfo, IDrawableState state)
        {

            // Set The First Texture Unit To Normalize Our Vector From The Surface To The Light.
            // Set The Texture Environment Of The First Texture Unit To Replace It With The
            // Sampled Value Of The Normalization Cube Map.

            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            vertexes.Bind();
            //Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, vertexName);
            Gl.glVertexPointer(Vector2D.Count, GlHelper.GlScalar, 0, IntPtr.Zero);

            Gl.glActiveTextureARB(Gl.GL_TEXTURE0_ARB);
            Gl.glClientActiveTexture(Gl.GL_TEXTURE0_ARB);
            Gl.glEnable(Gl.GL_TEXTURE_CUBE_MAP);
            Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, normalization_cube_map);
            Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_COMBINE);
            Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_COMBINE_RGB, Gl.GL_REPLACE);
            Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_SOURCE0_RGB, Gl.GL_TEXTURE);

            Gl.glEnableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
            vertexes.Bind();
            //Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, vertexName);
            Gl.glTexCoordPointer(Vector2D.Count, GlHelper.GlScalar, 0, IntPtr.Zero);

            //now we change the textures origin to that of the light's position
            GlHelper.GlGetModelViewMatrix(GlMatrix);
            Matrix4x4 matrix;
            Matrix4x4.CopyTranspose(GlMatrix, out matrix);
            Matrix4x4.Invert(ref matrix, out matrix);
            Vector3D lightPos;
            Vector3D.Transform(ref matrix, ref light.Position, out lightPos);
            Gl.glMatrixMode(Gl.GL_TEXTURE);
            Gl.glLoadIdentity();
            GlHelper.GlScale(-1, -1, -1);


            GlHelper.GlTranslate(
                (xInverted) ? (lightPos.X) : (-lightPos.X),
                (yInverted) ? (lightPos.Y) : (-lightPos.Y),
                -lightPos.Z);

            // Set The Second Unit To The Bump Map.
            // Set The Texture Environment Of The Second Texture Unit To Perform A Dot3
            // Operation With The Value Of The Previous Texture Unit (The Normalized
            // Vector Form The Surface To The Light) And The Sampled Texture Value (The
            // Normalized Normal Vector Of Our Bump Map).
            Gl.glActiveTextureARB(Gl.GL_TEXTURE1_ARB);
            Gl.glClientActiveTexture(Gl.GL_TEXTURE1_ARB);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            bumpmap.Bind();

            Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_COMBINE);
            Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_COMBINE_RGB, Gl.GL_DOT3_RGB);
            Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_SOURCE0_RGB, Gl.GL_PREVIOUS);
            Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_SOURCE1_RGB, Gl.GL_TEXTURE);

            Gl.glEnableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
            coordinates.Bind();
            Gl.glTexCoordPointer(Vector2D.Count, GlHelper.GlScalar, 0, IntPtr.Zero);



            // Set The Third Texture Unit To Our Texture.
            // Set The Texture Environment Of The Third Texture Unit To Modulate
            // (Multiply) The Result Of Our Dot3 Operation With The Texture Value.
            Gl.glActiveTextureARB(Gl.GL_TEXTURE2_ARB);
            Gl.glClientActiveTexture(Gl.GL_TEXTURE2_ARB);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            sprite.Bind();

            Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);

            Gl.glEnableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
            coordinates.Bind();
            Gl.glTexCoordPointer(Vector2D.Count, GlHelper.GlScalar, 0, IntPtr.Zero);

            //THEN YOU DRAW IT! MUAHHAAHA IT WORKS! it finally works!
            Gl.glDrawArrays(Gl.GL_QUADS, 0, 4);




            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glDisableClientState(Gl.GL_TEXTURE_COORD_ARRAY);


            Gl.glActiveTextureARB(Gl.GL_TEXTURE1_ARB);
            Gl.glClientActiveTexture(Gl.GL_TEXTURE1_ARB);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glDisableClientState(Gl.GL_TEXTURE_COORD_ARRAY);

            Gl.glActiveTextureARB(Gl.GL_TEXTURE0_ARB);
            Gl.glClientActiveTexture(Gl.GL_TEXTURE0_ARB);
            Gl.glDisable(Gl.GL_TEXTURE_CUBE_MAP);
            Gl.glDisableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
            Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);

            Gl.glLoadIdentity();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
        }
        public override IDrawableState CreateState()
        {
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                vertexes.Dispose();
                coordinates.Dispose();
                sprite.Dispose();
                bumpmap.Dispose();
            }
            GlHelper.GlDeleteTextures(LastRefresh, new int[] { normalization_cube_map });
        }
    }

}