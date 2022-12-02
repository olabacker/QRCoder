using QRCoder;
using SixLabors.ImageSharp;

Console.WriteLine("Hello, World!");


QRCodeGenerator qrGenerator = new QRCodeGenerator();
QRCodeData qrCodeData = qrGenerator.CreateQrCode("HEJ ERIK FRÅN NET7 QRCODER.", QRCodeGenerator.ECCLevel.Q);
QRCode qrCode = new QRCode(qrCodeData);
var qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, Image.Load("sant.png"), iconBorderWidth: 3);

qrCodeImage.Save("test.png"); // change the file extension accordingly