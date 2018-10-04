﻿using System;
using static System.Console;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace TextImageConverter
{
    public static class Composer
    {
        public static void Compose()
        {
            ComposeConfiguraton currentConfig = new ComposeConfiguraton();
            GetFilePath(currentConfig);
            using (FileStream fileStream = new FileStream(currentConfig.WorkingPath, FileMode.Open))
            {
                currentConfig.FileLength = fileStream.Length;
                WriteLine($"The size of this file is {currentConfig.FileLength} bytes. ");
                ProcessFileTail(currentConfig, fileStream);
                GetImageSize(currentConfig);
                Bitmap bitmap = new Bitmap(currentConfig.ImgWidth.Value, currentConfig.ImgHeight.Value);
                Stopwatch stopWatch = Stopwatch.StartNew();
                stopWatch.Start();
                WriteLine("Started generating image... ");
                PlotPixels(currentConfig, fileStream, bitmap);
                stopWatch.Stop();
                WriteLine($"Process completed in {stopWatch.Elapsed}. ");
                SaveImage(bitmap, currentConfig);
            }
            File.Delete(currentConfig.WorkingPath); // However, if the user terminates the program before it finishes by itself, the temp file will not be deleted and thus disk space will be unnecessarily occupied in a relatively long term. 
            WriteLine("Done. ");
        }

        public static void SilentCompose(ComposeConfiguraton currentConfig)
        {
            currentConfig.WorkingPath = Path.GetTempFileName();
            File.Copy(currentConfig.SourcePath, currentConfig.WorkingPath, true);
            using (FileStream fileStream = new FileStream(currentConfig.WorkingPath, FileMode.Open))
            {
                currentConfig.FileLength = fileStream.Length;
                ProcessFileTail(currentConfig, fileStream);
                CalculateImageSize(currentConfig);
                Bitmap bitmap = new Bitmap(currentConfig.ImgWidth.Value, currentConfig.ImgHeight.Value);
                PlotPixels(currentConfig, fileStream, bitmap);
                bitmap.Save(currentConfig.SavePath);
            }
            File.Delete(currentConfig.WorkingPath);
        }

        private static void SaveImage(Bitmap bitmap, ComposeConfiguraton currentConfig)
        {
            WriteLine("Please specify the path for the image to be saved: ");
            while (true)
            {
                try
                {
                    currentConfig.SavePath = Path.GetFullPath(ReadLine());
                    bitmap.Save(currentConfig.SavePath);
                    WriteLine("Image saved. ");
                    break;
                }
                catch (Exception e)
                {
                    WriteLine($"Error: {e.Message} Please check your input and try again: ");
                }
            }
        }

        private static Color GetCurrentColor(long currentPosition, FileStream fileStream, ComposeConfiguraton currentConfig)
        {
            if (currentPosition < currentConfig.FileLength)
            {
                fileStream.Position = currentPosition;
                int red = fileStream.ReadByte();
                int green = fileStream.ReadByte();
                int blue = fileStream.ReadByte();
                return Color.FromArgb(red, green, blue);
            }
            return Color.Black;
        }

        private static void PlotPixels(ComposeConfiguraton currentConfig, FileStream fileStream, Bitmap bitmap)
        {
            long currentPosition = 0;
            for (int y = 0; y < currentConfig.ImgHeight.Value; y++)
            {
                for (int x = 0; x < currentConfig.ImgWidth.Value; x++)
                {
                    Color currentColor = GetCurrentColor(currentPosition, fileStream, currentConfig);
                    bitmap.SetPixel(x, y, currentColor);
                    currentPosition += 3;
                }
            }
        }

        private static void GetImageSize(ComposeConfiguraton currentConfig)
        {
            WriteLine("By default, an image with width and height as consistent as possible will be generated. However, you may specify the width, height, or both dimensions of the image. ");
            while (true)
            {
                WriteLine("Please specify the width of the image if you would like to, otherwise, press Enter to continue. ");
                try
                {
                    string inputWidth = ReadLine();
                    if (!string.IsNullOrWhiteSpace(inputWidth))
                    {
                        currentConfig.ImgWidth = int.Parse(inputWidth);
                    }
                    break;
                }
                catch (Exception e)
                {
                    WriteLine($"Error: {e.Message} Please check your input and try again: ");
                }
            }

            WriteLine("Please specify the height of the image if you would like to, otherwise, press Enter to continue. ");
            while (true)
            {
                try
                {
                    string inputHeight = ReadLine();
                    if (!string.IsNullOrWhiteSpace(inputHeight))
                    {
                        currentConfig.ImgHeight = int.Parse(inputHeight);
                    }
                    break;
                }
                catch (Exception e)
                {
                    WriteLine($"Error: {e.Message} Please check your input and try again: ");
                }
            }

            long pixelCount = currentConfig.FileLength / 3;
            if (currentConfig.ImgHeight.HasValue && currentConfig.ImgWidth.HasValue)
            {
                if (currentConfig.ImgHeight * currentConfig.ImgWidth > pixelCount)
                {
                    WriteLine("Warning: Your specified size of image has a larger capability than the size of the file. Please note that the remaining pixels will be filled with black color. ");
                }
                else if (currentConfig.ImgHeight * currentConfig.ImgWidth < pixelCount)
                {
                    WriteLine("Warning: Your specified size of image cannot fully contain the information stored in the file. Please note that the part of the file beyond the capability of the image will be truncated. ");
                }
            }
            else
            {
                CalculateImageSize(currentConfig);
                WriteLine($"An image of {currentConfig.ImgWidth} pixels in width and {currentConfig.ImgHeight} pixels in height will be generated. ");
            }
        }

        private static void CalculateImageSize(ComposeConfiguraton currentConfig)
        {
            double pixelCount = currentConfig.FileLength / 3; // currentConfig.FileLength can be surely divided by 3 with no remainder, however pixelCount need to be in a non-integral type to prevent force flooring during division operation with an int. 
            if (currentConfig.ImgHeight.HasValue && !currentConfig.ImgWidth.HasValue)
            {
                double imgWidth = pixelCount / currentConfig.ImgHeight.Value;
                currentConfig.ImgWidth = (int)Math.Ceiling(imgWidth);
            }
            else if (!currentConfig.ImgHeight.HasValue && currentConfig.ImgWidth.HasValue)
            {
                double imgHeight = pixelCount / currentConfig.ImgWidth.Value;
                currentConfig.ImgHeight = (int)Math.Ceiling(imgHeight);
            }
            else if (!currentConfig.ImgHeight.HasValue && !currentConfig.ImgWidth.HasValue)
            {
                currentConfig.ImgHeight = (int)Math.Ceiling(Math.Sqrt(pixelCount));
                double imgWidth = pixelCount / currentConfig.ImgHeight.Value;
                currentConfig.ImgWidth = (int)Math.Ceiling(imgWidth);
            }
        }

        private static void ProcessFileTail(ComposeConfiguraton currentConfig, FileStream fileStream)
        {
            fileStream.Seek(0, SeekOrigin.End);
            fileStream.WriteByte(23); // Append 'End of Transmission Block' byte at the end of the file. 
            switch ((currentConfig.FileLength + 1) % 3) // Ensure the length (in bytes) is divisible by 3, which is the number of bytes that a RGB24 pixel may contain. 
            {
                case 1:
                    fileStream.WriteByte(0);
                    fileStream.WriteByte(0);
                    break;
                case 2:
                    fileStream.WriteByte(0);
                    break;
            }
            currentConfig.FileLength = fileStream.Length;
        }

        private static void GetFilePath(ComposeConfiguraton currentConfig)
        {
            WriteLine("Please specify the path for the file to be read: ");
            while (true)
            {
                try
                {
                    currentConfig.SourcePath = Path.GetFullPath(ReadLine());
                    currentConfig.WorkingPath = Path.GetTempFileName();
                    File.Copy(currentConfig.SourcePath, currentConfig.WorkingPath, true);
                    break;
                }
                catch (Exception e)
                {
                    WriteLine($"Error: {e.Message} Please check your input and try again: ");
                }
            }
        }
    }
}
