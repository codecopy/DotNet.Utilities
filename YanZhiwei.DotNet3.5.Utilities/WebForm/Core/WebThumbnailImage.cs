﻿namespace YanZhiwei.DotNet3._5.Utilities.WebForm.Core
{
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using YanZhiwei.DotNet2.Utilities.Operator;
    using YanZhiwei.DotNet3._5.Utilities.Enum;

    /// <summary>
    /// 生成缩略图
    /// </summary>
    public class WebThumbnailImage
    {
        #region Methods

        /// <summary>
        ///  生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>>
        public static void CreateThumbnail(string originalImagePath, string thumbnailPath, int width, int height)
        {
            CreateThumbnail(originalImagePath, thumbnailPath, width, height, "Cut", false, ImagePosition.Default, null, 75);
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="specifiedwidth">缩略图宽度</param>
        /// <param name="specifiedheight">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>
        /// <param name="isaddwatermark">是否添加水印</param>
        /// <param name="quality">图片品质</param>
        /// <param name="imagePosition">水印位置</param>
        /// <param name="waterImagePath">水印图片名称</param>
        public static void CreateThumbnail(string originalImagePath, string thumbnailPath, int specifiedwidth, int specifiedheight, ThumbnailImageCutMode mode, bool isaddwatermark, ImagePosition imagePosition, string waterImagePath, int quality)
        {
            ValidateOperator.Begin().CheckFileExists(originalImagePath).IsFilePath(thumbnailPath);
            if (isaddwatermark)
                ValidateOperator.Begin().CheckFileExists(waterImagePath);

            using (Image originalImage = Image.FromFile(originalImagePath))
            {
                int _cutedWidth = specifiedwidth;
                int _cutedHeight = specifiedheight;

                int x = 0;
                int y = 0;
                int _originalWidth = originalImage.Width;
                int _originalHeigh = originalImage.Height;

                switch (mode)
                {
                    case ThumbnailImageCutMode.W://指定宽，高按比例
                        _cutedHeight = originalImage.Height * specifiedwidth / originalImage.Width;
                        break;

                    case ThumbnailImageCutMode.H://指定高，宽按比例
                        _cutedWidth = originalImage.Width * specifiedheight / originalImage.Height;
                        break;

                    case ThumbnailImageCutMode.Cut://指定高宽裁减（不变形）
                        if (originalImage.Width >= _cutedWidth && originalImage.Height >= _cutedHeight)
                        {
                            if ((double)originalImage.Width / (double)originalImage.Height > (double)_cutedWidth / (double)_cutedHeight)
                            {
                                _originalHeigh = originalImage.Height;
                                _originalWidth = originalImage.Height * _cutedWidth / _cutedHeight;
                                y = 0;
                                x = (originalImage.Width - _originalWidth) / 2;
                            }
                            else
                            {
                                _originalWidth = originalImage.Width;
                                _originalHeigh = originalImage.Width * specifiedheight / _cutedWidth;
                                x = 0;
                                y = (originalImage.Height - _originalHeigh) / 2;
                            }
                        }
                        else
                        {
                            x = (originalImage.Width - _cutedWidth) / 2;
                            y = (originalImage.Height - _cutedHeight) / 2;
                            _originalWidth = _cutedWidth;
                            _originalHeigh = _cutedHeight;
                        }
                        break;

                    case ThumbnailImageCutMode.Fit://不超出尺寸，比它小就不截了，不留白，大就缩小到最佳尺寸，主要为手机用
                        if (originalImage.Width > _cutedWidth && originalImage.Height > _cutedHeight)
                        {
                            if ((double)originalImage.Width / (double)originalImage.Height > (double)_cutedWidth / (double)_cutedHeight)
                                _cutedHeight = originalImage.Height * specifiedwidth / originalImage.Width;
                            else
                                _cutedWidth = originalImage.Width * specifiedheight / originalImage.Height;
                        }
                        else if (originalImage.Width > _cutedWidth)
                        {
                            _cutedHeight = originalImage.Height * specifiedwidth / originalImage.Width;
                        }
                        else if (originalImage.Height > _cutedHeight)
                        {
                            _cutedWidth = originalImage.Width * specifiedheight / originalImage.Height;
                        }
                        else
                        {
                            _cutedWidth = originalImage.Width;
                            _cutedHeight = originalImage.Height;
                            _originalWidth = _cutedWidth;
                            _originalHeigh = _cutedHeight;
                        }
                        break;

                    default:
                        break;
                }

                Image _bitmap = new Bitmap(_cutedWidth, _cutedHeight);
                Graphics _g = SetThumbnailGraphics(originalImage, _bitmap, _cutedWidth, _cutedHeight, _originalWidth, _originalHeigh, x, y);
                SetThumbnailWaterImage(isaddwatermark, waterImagePath, imagePosition, _g, _cutedWidth, _cutedHeight);
                EncoderParameters _encoderParams = null;
                ImageCodecInfo _jpegICI = SetThumbnailImageQuality(quality, out _encoderParams);
                SaveThumbnailImage(thumbnailPath, _bitmap, _jpegICI, _encoderParams);
            }
        }

        private static void SaveThumbnailImage(string thumbnailPath, Image bitmap, ImageCodecInfo jpegICI, EncoderParameters encoderParams)
        {
            if (jpegICI != null)
            {
                bitmap.Save(thumbnailPath, jpegICI, encoderParams);
            }
            else
            {
                //以jpg格式保存缩略图
                bitmap.Save(thumbnailPath, ImageFormat.Jpeg);
            }
        }

        private static ImageCodecInfo SetThumbnailImageQuality(int quality, out EncoderParameters encoderParams)
        {
            // 以下代码为保存图片时,设置压缩质量
            encoderParams = new EncoderParameters();
            long[] _qualityArray = new long[1];
            _qualityArray[0] = quality;
            EncoderParameter _encoderParam = new EncoderParameter(Encoder.Quality, _qualityArray);
            encoderParams.Param[0] = _encoderParam;
            //获得包含有关内置图像编码解码器的信息的ImageCodecInfo 对象.
            ImageCodecInfo[] _arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo _jpegICI = null;
            for (int i = 0; i < _arrayICI.Length; i++)
            {
                if (_arrayICI[i].FormatDescription.Equals("JPEG"))
                {
                    _jpegICI = _arrayICI[i];
                    //设置JPEG编码
                    break;
                }
            }
            return _jpegICI;
        }

        private static void SetThumbnailWaterImage(bool isaddwatermark, string waterImagePath, ImagePosition imagePosition, Graphics g, int cutedWidth, int cutedHeight)
        {
            if (isaddwatermark)
            {
                Image _waterImage = Image.FromFile(waterImagePath);
                int _xPosOfWm;
                int _yPosOfWm;
                int _wmHeight = _waterImage.Height;
                int _wmWidth = _waterImage.Width;
                int _phHeight = cutedHeight;
                int _phWidth = cutedWidth;
                switch (imagePosition)
                {
                    case ImagePosition.LeftBottom:
                        _xPosOfWm = 70;
                        _yPosOfWm = _phHeight - _wmHeight - 70;
                        break;

                    case ImagePosition.LeftTop:
                        _xPosOfWm = 70;
                        _yPosOfWm = 0 - 70;
                        break;

                    case ImagePosition.RightTop:
                        _xPosOfWm = _phWidth - _wmWidth - 70;
                        _yPosOfWm = 0 - 70;
                        break;

                    case ImagePosition.RigthBottom:
                        _xPosOfWm = _phWidth - _wmWidth - 70;
                        _yPosOfWm = _phHeight - _wmHeight - 70;
                        break;

                    default:
                        _xPosOfWm = 10;
                        _yPosOfWm = 0;
                        break;
                }
                g.DrawImage(_waterImage, new Rectangle(_xPosOfWm, _yPosOfWm, _waterImage.Width, _waterImage.Height), 0, 0, _waterImage.Width, _waterImage.Height, GraphicsUnit.Pixel);
            }
        }

        private static Graphics SetThumbnailGraphics(Image originalImage, Image bitmap, int cutedWidth, int cutedHeight, int originalWidth, int originalHeigh, int x, int y)
        {
            Graphics _g = Graphics.FromImage(bitmap);
            //设置高质量插值法
            _g.InterpolationMode = InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            _g.SmoothingMode = SmoothingMode.HighQuality;

            _g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            _g.CompositingQuality = CompositingQuality.HighQuality;

            //清空画布并以透明背景色填充
            _g.Clear(Color.White);

            //在指定位置并且按指定大小绘制原图片的指定部分
            _g.DrawImage(originalImage, new Rectangle(0, 0, cutedWidth, cutedHeight),
                new Rectangle(x, y, originalWidth, originalHeigh),
                GraphicsUnit.Pixel);
            return _g;
        }

        #endregion Methods
    }
}