﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DirectOutput.General.BitmapHandling
{
    public class FastBitmap
    {
        private PixelData[,] _Pixels = new PixelData[0, 0];
        /// <summary>
        /// The pixel data array of the frame. <br />
        /// Dimension 0 if the array is the x/horizontal direction.
        /// Dimension 1 of the array is the y/vertical direction.
        /// </summary>
        /// <value>
        /// The pixels array of the frame.
        /// </value>
        public PixelData[,] Pixels
        {
            get
            {
                return _Pixels;
            }
            private set
            {
                _Pixels = value;
            }
        }


        /// <summary>
        /// Gets the PixelData for the specified pixel of the frame.<br/>
        /// For positions outside the frame, the method will return PixelData for a fully transparent black pixel.
        /// </summary>
        /// <param name="X">The X position of the pixel.</param>
        /// <param name="Y">The Y position of the pixel.</param>
        /// <returns>PixelData for the specified pixel.</returns>
        public PixelData GetPixel(int X, int Y)
        {
            if (X < Width && Y < Height)
            {
                return Pixels[X, Y];
            }
            else
            {
                return new PixelData();
            }


            //try
            //{
            //    return Pixels[X, Y];
            //}
            //catch { }
            //return new PixelData();
        }



        /// <summary>
        /// Gets a FastBitmap with a specified size representing a defineable section of the current object.
        /// </summary>
        /// <param name="ResultWidth">Width of the resulting FastBitmap object.</param>
        /// <param name="ResultHeight">Height of the resulting FastBitmap object.</param>
        /// <param name="SourceLeft">The left boundary for the source area.</param>
        /// <param name="SourceTop">The top boundary of the source arrea.</param>
        /// <param name="SourceWidth">Width of the source area.</param>
        /// <param name="SourceHeight">Height of the source area.</param>
        /// <param name="DataExtractMode">The data extract mode enum.</param>
        /// <returns>FastBitmap object with a specified size representing a defineable section of the current object</returns>
        public FastBitmap GetClip(int ResultWidth, int ResultHeight, int SourceLeft = 0, int SourceTop = 0, int SourceWidth = -1, int SourceHeight = -1, FastBitmapDataExtractModeEnum DataExtractMode = FastBitmapDataExtractModeEnum.SinglePixelCenter)
        {

            SourceLeft = SourceLeft.Limit(0, Width - 1);
            SourceTop = SourceTop.Limit(0, Height - 1);
            SourceWidth = (SourceWidth < 0 ? Width - SourceLeft : SourceWidth);
            SourceHeight = (SourceHeight < 0 ? Height - SourceTop : SourceHeight);
            ResultWidth = ResultWidth.Limit(0, int.MaxValue);
            ResultHeight = ResultHeight.Limit(0, int.MaxValue);

            FastBitmap F = new FastBitmap();
            F.SetFrameSize(ResultWidth, ResultHeight);
            PixelData[,] D = F.Pixels;

            switch (DataExtractMode)
            {
                case FastBitmapDataExtractModeEnum.BlendPixels:

                    float Red = 0;
                    float Green = 0;
                    float Blue = 0;
                    float Alpha = 0;
                    float Weight = 0;

                    float PixelSourceWidth = (float)SourceWidth / ResultWidth;
                    float PixelSourceHeight = (float)SourceHeight / ResultHeight;
                    float PixelSourceCount = PixelSourceWidth * PixelSourceHeight;


                    for (int y = 0; y < ResultHeight; y++)
                    {
                        float PixelSourceTop = SourceTop+ y * PixelSourceHeight;
                        float PixelSourceBottom = PixelSourceTop + PixelSourceHeight;
    

                        for (int x = 0; x < ResultWidth; x++)
                        {
                            int PSR = 0;
                            int PSB = 0;
                            float PixelSourceLeft = SourceLeft+x * PixelSourceWidth;
                            float PixelSourceRight = PixelSourceLeft + PixelSourceWidth;
                            Red = 0;
                            Green = 0;
                            Blue = 0;
                            Alpha = 0;

                            if (!PixelSourceTop.IsIntegral())
                            {
                                //Upper left corner
                                if (!PixelSourceLeft.IsIntegral())
                                {
                                    PixelData PD = GetPixel((int)PixelSourceLeft.Floor(), (int)PixelSourceTop.Floor());
                                    Weight = (PixelSourceTop.Ceiling() - PixelSourceTop) * (PixelSourceLeft.Ceiling() - PixelSourceLeft);
                                    Red += PD.Red * Weight;
                                    Green += PD.Green * Weight;
                                    Blue += PD.Blue * Weight;
                                    Alpha += PD.Alpha * Weight;
                                }

                                //Upper edge
                                PSR = (int)PixelSourceRight.Floor();
                                Weight = (PixelSourceTop.Ceiling() - PixelSourceTop);
                                for (int xs = (int)PixelSourceLeft.Ceiling(); xs < PSR; xs++)
                                {
                                    PixelData PD = GetPixel(xs, (int)PixelSourceTop.Floor());
                                    Red += PD.Red * Weight;
                                    Green += PD.Green * Weight;
                                    Blue += PD.Blue * Weight;
                                    Alpha += PD.Alpha * Weight;
                                }

                                //Upper right corner
                                if (!PixelSourceRight.IsIntegral())
                                {
                                    Weight = (PixelSourceTop.Ceiling() - PixelSourceTop) * (PixelSourceRight - PixelSourceRight.Floor());
                                    PixelData PD = GetPixel((int)PixelSourceRight.Floor(), (int)PixelSourceTop.Floor());
                                    Red += PD.Red * Weight;
                                    Green += PD.Green * Weight;
                                    Blue += PD.Blue * Weight;
                                    Alpha += PD.Alpha * Weight;
                                }
                            }

                            PSB = (int)PixelSourceBottom.Floor();
                            PSR = (int)PixelSourceRight.Floor();
                            for (int ys = (int)PixelSourceTop.Ceiling(); ys < PSB; ys++)
                            {
                                //Left edge
                                if (!PixelSourceLeft.IsIntegral())
                                {
                                    PixelData PD = GetPixel((int)PixelSourceLeft.Floor(), ys);
                                    Weight = (PixelSourceLeft.Ceiling() - PixelSourceLeft);
                                    Red += PD.Red * Weight;
                                    Green += PD.Green * Weight;
                                    Blue += PD.Blue * Weight;
                                    Alpha += PD.Alpha * Weight;
                                }

                                //full pixels
                                for (int xs = (int)PixelSourceLeft.Ceiling(); xs < PSR; xs++)
                                {
                                    PixelData PD = GetPixel(xs, ys);
                                    Red += PD.Red;
                                    Green += PD.Green;
                                    Blue += PD.Blue;
                                    Alpha += PD.Alpha;
                                }

                                //Right edge
                                if (!PixelSourceRight.IsIntegral())
                                {
                                    Weight = (PixelSourceRight - PSR);
                                    PixelData PD = GetPixel(PSR, ys);
                                    Red += PD.Red * Weight;
                                    Green += PD.Green * Weight;
                                    Blue += PD.Blue * Weight;
                                    Alpha += PD.Alpha * Weight;
                                }
                            }


                            if (!PixelSourceBottom.IsIntegral())
                            {
                                PSB = (int)PixelSourceBottom.Floor();
                                PSR = (int)PixelSourceRight.Floor();

                                //Lower left corner
                                if (!PixelSourceLeft.IsIntegral())
                                {
                                    PixelData PD = GetPixel((int)PixelSourceLeft.Floor(), PSB);
                                    Weight = (PixelSourceBottom - PSB) * (PixelSourceLeft.Ceiling() - PixelSourceLeft);
                                    Red += PD.Red * Weight;
                                    Green += PD.Green * Weight;
                                    Blue += PD.Blue * Weight;
                                    Alpha += PD.Alpha * Weight;
                                }

                                //Lower edge
                      

                                Weight = (PixelSourceBottom - PSB);
                                for (int xs = (int)PixelSourceLeft.Ceiling(); xs < PSR; xs++)
                                {
                                    PixelData PD = GetPixel(xs, PSB);
                                    Red += PD.Red * Weight;
                                    Green += PD.Green * Weight;
                                    Blue += PD.Blue * Weight;
                                    Alpha += PD.Alpha * Weight;
                                }

                                //Lower right corner
                                if (!PixelSourceRight.IsIntegral())
                                {
                                    Weight = (PixelSourceBottom - PSB) * (PixelSourceRight - PSR);
                                    PixelData PD = GetPixel(PSR, PSB);
                                    Red += PD.Red * Weight;
                                    Green += PD.Green * Weight;
                                    Blue += PD.Blue * Weight;
                                    Alpha += PD.Alpha * Weight;
                                }
                            }
                            D[x, y] = new PixelData((byte)(Red / PixelSourceCount).Limit(0, 255), (byte)(Green / PixelSourceCount).Limit(0, 255), (byte)(Blue / PixelSourceCount).Limit(0, 255), (byte)(Alpha / PixelSourceCount).Limit(0, 255));
                        }

                    }
                    break;
                case FastBitmapDataExtractModeEnum.SinglePixelTopLeft:
                case FastBitmapDataExtractModeEnum.SinglePixelCenter:
                default:
                    float XSource = 0;

                    float XSourceBase = 0;
                    float YSource = 0;
                    float XStep = (float)SourceWidth / ResultWidth;
                    float YStep = (float)SourceHeight / ResultHeight;

                    if (DataExtractMode == FastBitmapDataExtractModeEnum.SinglePixelCenter)
                    {
                        XSourceBase = XStep / 2;
                        YSource = YStep / 2;
                    }

                    for (int y = 0; y < ResultHeight; y++)
                    {
                        XSource = XSourceBase;
                        for (int x = 0; x < ResultWidth; x++)
                        {
                            D[x, y] = GetPixel(XSource.RoundToInt(), YSource.RoundToInt());
                            XSource += XStep;
                        }
                        YSource += YStep;
                    }

                    break;
            }


            return F;
        }


        /// <summary>
        /// Sets the size (width/height) of the frame.<br/>
        /// Setting the framesize will discard all existing pixel data of the frame.
        /// </summary>
        /// <param name="Width">The width of the frame.</param>
        /// <param name="Height">The height of the frame.</param>
        public void SetFrameSize(int Width, int Height)
        {
            Pixels = new PixelData[Width.Limit(0, int.MaxValue), Height.Limit(0, int.MaxValue)];
            _Width = Width;
            _Height = Height;
        }



        private int _Width = 0;
        
        /// <summary>
        /// Gets the width of the frame.<br/>
        /// </summary>
        /// <value>
        /// The width of the frame.
        /// </value>
        public int Width
        {
            get
            {
                return _Width;
            }
        }


        private int _Height = 0;
        /// <summary>
        /// Get the height of the frame.<br/>
        /// <value>
        /// The height of the frame.
        /// </value>
        /// </summary>
        public int Height
        {
            get
            {
                return Pixels.GetLength(1);
            }
        }


        /// <summary>
        /// Loads the currently active frame of the specified Image into the FastBitmap object.
        /// <param name="Image">The image.</param>
        /// </summary>
        public void Load(Image Image)
        {
            Load(new Bitmap(Image));
        }

        /// <summary>
        /// Loads the currently active frame of the specified bitmap into the FastBitmap object.
        /// <param name="Bitmap">The bitmap.</param>
        /// </summary>
        public void Load(Bitmap Bitmap)
        {

            UnsafeBitmap UImg = new UnsafeBitmap(Bitmap);
            UImg.LockBitmap();

            SetFrameSize(Bitmap.Width, Bitmap.Height);

            PixelData[,] P = Pixels;

            int h = Bitmap.Height;
            int w = Bitmap.Width;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    P[x, y] = UImg.GetPixel(x, y);
                }
            }
            UImg.UnlockBitmap();
            UImg = null;

            GC.Collect();
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="FastBitmap"/> class.
        /// </summary>
        public FastBitmap() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastBitmap"/> class and loads the currently active frame of the specified bitmap into the FastBitmap object.
        /// </summary>
        /// <param name="Bitmap">The bitmap.</param>
        public FastBitmap(Bitmap Bitmap)
        {
            Load(Bitmap);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastBitmap"/> class and loads the currently active frame of the specified image into the FastBitmap object..
        /// </summary>
        /// <param name="Image">The image.</param>
        public FastBitmap(Image Image)
        {
            Load(Image);
        }
    }
}
