using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using static QRCoder.QRCodeGenerator;

namespace QRCoder
{
    public class QRCode : AbstractQRCode, IDisposable
    {
        /// <summary>
        /// Constructor without params to be used in COM Objects connections
        /// </summary>
        public QRCode() { }

        public QRCode(QRCodeData data) : base(data) { }

        public Image GetGraphic(int pixelsPerModule) => GetGraphic(pixelsPerModule, Color.Black, Color.White, true);

        public Image GetGraphic(int pixelsPerModule, string darkColorHtmlHex, string lightColorHtmlHex, bool drawQuietZones = true) => GetGraphic(pixelsPerModule, Color.HotPink /* ColorTranslator.FromHtml(darkColorHtmlHex)*/, Color.HotPink /*ColorTranslator.FromHtml(lightColorHtmlHex)*/, drawQuietZones);

        public Image GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true)
        {
            var size = (QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
            var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

            //using var img = Image.<SixLabors.ImageSharp.PixelFormats.Rgba32>()

            //Image.n(size, size);

            var bmp = new Image<Rgba32>(size, size);
            //using (var gfx = Graphics.FromImage(bmp))
            var lightBrush = new SolidBrush(lightColor);
            var darkBrush = new SolidBrush(darkColor);
            {
                for (var x = 0; x < size + offset; x = x + pixelsPerModule)
                {
                    for (var y = 0; y < size + offset; y = y + pixelsPerModule)
                    {
                        var module = QrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1];

                        if (module)
                        {
                            var rect = new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule);
                            bmp.Mutate(xy => xy.Fill(darkBrush, rect));
                            //gfx.FillRectangle(darkBrush, );
                        }
                        else
                        {
                            var rect = new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule);
                            bmp.Mutate(xy => xy.Fill(lightBrush, rect));

                            //gfx.FillRectangle(lightBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                        }
                    }
                }

                //gfx.Save();
            }

            //bmp.Save("test.png");

            return bmp;
        }

        public Image GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, Image icon = null, int iconSizePercent = 15, int iconBorderWidth = 0, bool drawQuietZones = true, Color? iconBackgroundColor = null)
        {
            var size = (QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
            var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

            var bmp = new Image<Rgba32>(size, size /*PixelFormat.Format32bppArgb*/);

            //using (var gfx = Graphics.FromImage(bmp))
            var lightBrush = new SolidBrush(lightColor);
            var darkBrush = new SolidBrush(darkColor);
            //{
            //    gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //    gfx.CompositingQuality = CompositingQuality.HighQuality;
            //    gfx.Clear(lightColor);
            bmp.Mutate(x => x.Clear(lightBrush));


            var drawIconFlag = icon != null && iconSizePercent > 0 && iconSizePercent <= 100;

            for (var x = 0; x < size + offset; x = x + pixelsPerModule)
            {
                for (var y = 0; y < size + offset; y = y + pixelsPerModule)
                {
                    var moduleBrush = QrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1] ? darkBrush : lightBrush;

                    bmp.Mutate(xy => xy.Fill(moduleBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule)));
                    //gfx.FillRectangle(moduleBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                }
            }

            if (drawIconFlag)
            {
                float iconDestWidth = iconSizePercent * bmp.Width / 100f;
                float iconDestHeight = drawIconFlag ? iconDestWidth * icon.Height / icon.Width : 0;
                float iconX = (bmp.Width - iconDestWidth) / 2;
                float iconY = (bmp.Height - iconDestHeight) / 2;
                var centerDest = new RectangleF(iconX - iconBorderWidth, iconY - iconBorderWidth, iconDestWidth + iconBorderWidth * 2, iconDestHeight + iconBorderWidth * 2);
                var iconDestRect = new RectangleF(iconX, iconY, iconDestWidth, iconDestHeight);
                var iconBgBrush = iconBackgroundColor != null ? new SolidBrush((Color)iconBackgroundColor) : lightBrush;
                //Only render icon/logo background, if iconBorderWith is set > 0
                // TODO
                if (iconBorderWidth > 0)
                {
                    var iconPath = CreateRoundedRectanglePath(centerDest, iconBorderWidth * 2);

                    bmp.Mutate(x => x.Fill(iconBgBrush, iconPath));

                    //using (GraphicsPath iconPath = )
                    //{
                    //    gfx.FillPath(iconBgBrush, iconPath);
                    //}

                    
                }

                icon.Mutate(i => i.Resize(new ResizeOptions() { Size = new Size((int)iconDestWidth, (int)iconDestHeight), Mode = ResizeMode.Crop }));

                bmp.Mutate(x => x.DrawImage(icon, new Point((int)iconDestRect.Left, (int)iconDestRect.Top), new GraphicsOptions()
                {
                    
                }));
                //gfx.DrawImage(icon, iconDestRect, new RectangleF(0, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);
            }

            //bmp.Save();

            //    gfx.Save();
            //}

            return bmp;
        }

        internal Path CreateRoundedRectanglePath(RectangleF rect, int cornerRadius)
        {

            var lineSegments = new ILineSegment[]
              {
            new ArcLineSegment(new PointF(rect.X, rect.Y), new SizeF(cornerRadius, cornerRadius), 0, 180, 90),
            new LinearLineSegment(new PointF(rect.X + cornerRadius, rect.Y), new PointF(rect.Right - cornerRadius * 2, rect.Y)),
            new ArcLineSegment(new PointF(rect.X + rect.Width - cornerRadius * 2, rect.Y), new SizeF(cornerRadius, cornerRadius), 0, 270, 90),
            new LinearLineSegment(new PointF(rect.Right, rect.Y + cornerRadius * 2), new PointF(rect.Right, rect.Y + rect.Height - cornerRadius * 2)),
            new ArcLineSegment(new PointF(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2), new SizeF(cornerRadius * 2, cornerRadius * 2), 0, 0, 90),
            new LinearLineSegment(new PointF(rect.Right - cornerRadius * 2, rect.Bottom), new PointF(rect.X + cornerRadius * 2, rect.Bottom)),
            new ArcLineSegment(new PointF(rect.X, rect.Bottom - cornerRadius * 2), new SizeF(cornerRadius * 2, cornerRadius * 2), 0, 90, 90),
            new LinearLineSegment(new PointF(rect.X, rect.Bottom - cornerRadius * 2), new PointF(rect.X, rect.Y + cornerRadius * 2))
          };

            var roundedRect = new Path();

            //roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            //roundedRect.CloseFigure();
            return roundedRect;
        }
    }

    public static class QRCodeHelper
    {
        public static Image GetQRCode(string plainText, int pixelsPerModule, Color darkColor, Color lightColor, ECCLevel eccLevel, bool forceUtf8 = false, bool utf8BOM = false, EciMode eciMode = EciMode.Default, int requestedVersion = -1, Image icon = null, int iconSizePercent = 15, int iconBorderWidth = 0, bool drawQuietZones = true)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion))
            using (var qrCode = new QRCode(qrCodeData))
            {
                return qrCode.GetGraphic(pixelsPerModule, darkColor, lightColor, icon, iconSizePercent, iconBorderWidth, drawQuietZones);
            }
        }
    }
}

