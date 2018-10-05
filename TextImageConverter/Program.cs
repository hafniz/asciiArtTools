using System;
using static System.Console;
using System.Collections.Generic;
using System.Linq;

namespace TextImageConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (string.IsNullOrWhiteSpace(string.Concat(args)))
            {
                ShowHelp();
            }
            else
            {
                ParseArgs(args);
            }
        }

        private static void ParseArgs(string[] args)
        {
            switch (args[0])
            {
                case "-c":
                case "/c":
                    ParseForCompose(args);
                    break;
                case "-d":
                case "/d":
                    ParseForDecompose(args);
                    break;
                case "-h":
                case "/h":
                case "-?":
                case "/?":
                case "-help":
                case "--help": // Taking Linux users' usage habit into consideration.
                case "/help":
                    ShowHelp();
                    break;
                default:
                    WriteLine("Error: Unrecognized or incomplete arguments. Please enter -h for help. ");
                    break;
            }
        }

        private static void ShowHelp() => WriteLine("\nThis program composes an image from specified file by plotting pixels bitwisely, or writes the byte data from the pixels of specified image into a file. \n" +
                "Simply attach argument -c or -d to start composing or decomposing an image with command line prompts, or use the following syntax for unattended process: \n\n" +
                "Usage: [-c [source-path save-path [-w width] [-h height] [-s seed]] | -d [source-path save-path [-s seed]]]\n\n" +
                " -c\t\t Compose an image from specified file. \n" +
                " -d\t\t Decompose specified image into a file. \n" +
                " -w width\t Specify the width of the image to be composed, which is an integer. \n" +
                " -h height\t Specify the height of the image to be composed, which is an integer. \n" +
                " -s seed\t Specify the seed for image encryption and decryption. \n" +
                " source-path\t Specify the path and filename where the file or image is to be read. \n" +
                " save-path\t Specify the path and filename where the image or file generated is to be saved. \n");


        private static void ParseForDecompose(string[] args)
        {
            switch (args.Count())
            {
                case 1:
                    Decomposer.Decompose();
                    break;
                case 3:
                    try
                    {
                        Decomposer.SilentDecompose(new DecomposeConfiguration
                        {
                            SourcePath = args[1],
                            SavePath = args[2]
                        });
                    }
                    catch (Exception e)
                    {
                        WriteLine($"Error: {e.Message} Please check your input and try again. ");
                    }
                    break;
                case 5:
                    Dictionary<int, string> inputArgs = new Dictionary<int, string>();
                    for (int i = 0; i < args.Count(); i++)
                    {
                        inputArgs.Add(i, args[i]);
                    }
                    DecomposeConfiguration parsedConfig = new DecomposeConfiguration
                    {
                        SourcePath = args[1],
                        SavePath = args[2]
                    };
                    ParseSeedArg(inputArgs, parsedConfig);
                    try
                    {
                        Decomposer.SilentDecompose(parsedConfig);
                    }
                    catch (Exception e)
                    {
                        WriteLine($"Error: {e.Message} Please check your input and try again. ");
                    }
                    break;
                default:
                    WriteLine("Error: Improper number of arguments. Please check your input and try again. ");
                    break;
            }
        }

        private static void ParseForCompose(string[] args)
        {
            switch (args.Count())
            {
                case 1:
                    Composer.Compose();
                    break;
                case 3:
                    try
                    {
                        Composer.SilentCompose(new ComposeConfiguration
                        {
                            SourcePath = args[1],
                            SavePath = args[2]
                        });
                    }
                    catch (Exception e)
                    {
                        WriteLine($"Error: {e.Message} Please check your input and try again. ");
                    }
                    break;
                case 5:
                case 7:
                case 9:
                    Dictionary<int, string> inputArgs = new Dictionary<int, string>();
                    for (int i = 0; i < args.Count(); i++)
                    {
                        inputArgs.Add(i, args[i]);
                    }
                    ComposeConfiguration parsedConfig = new ComposeConfiguration
                    {
                        SourcePath = args[1],
                        SavePath = args[2]
                    };
                    ParseWidthArg(inputArgs, parsedConfig);
                    ParseHeightArg(inputArgs, parsedConfig);
                    ParseSeedArg(inputArgs, parsedConfig);
                    try
                    {
                        Composer.SilentCompose(parsedConfig);
                    }
                    catch (Exception e)
                    {
                        WriteLine($"Error: {e.Message} Please check your input and try again. ");
                    }
                    break;
                default:
                    WriteLine("Error: Improper number of arguments. Please check your input and try again. ");
                    break;
            }
        }

        private static void ParseSeedArg(Dictionary<int, string> inputArgs, ConfigurationBase parsedConfig)
        {
            Func<KeyValuePair<int, string>, bool> isSeedArg = x => x.Value == "-s" || x.Value == "/s";
            switch (inputArgs.Count(isSeedArg))
            {
                case 0:
                    break;
                case 1:
                    inputArgs.TryGetValue(inputArgs.First(isSeedArg).Key + 1, out string inputSeed);
                    try
                    {
                        parsedConfig.OffsetSeed = int.Parse(inputSeed);
                    }
                    catch (Exception e)
                    {
                        WriteLine($"Error: {e.Message} Please check your input and try again. ");
                    }
                    break;
                default:
                    WriteLine("Error: -s argument can only be used once. Please check your input and try again. ");
                    break;
            }
        }

        private static void ParseHeightArg(Dictionary<int, string> inputArgs, ComposeConfiguration parsedConfig)
        {
            Func<KeyValuePair<int, string>, bool> isHeightArg = x => x.Value == "-h" || x.Value == "/h";
            switch (inputArgs.Count(isHeightArg))
            {
                case 0:
                    break;
                case 1:
                    inputArgs.TryGetValue(inputArgs.First(isHeightArg).Key + 1, out string inputHeight);
                    try
                    {
                        parsedConfig.ImgHeight = int.Parse(inputHeight);
                    }
                    catch (Exception e)
                    {
                        WriteLine($"Error: {e.Message} Please check your input and try again. ");
                    }
                    break;
                default:
                    WriteLine("Error: -h argument can only be used once. Please check your input and try again. ");
                    break;
            }
        }

        private static void ParseWidthArg(Dictionary<int, string> inputArgs, ComposeConfiguration parsedConfig)
        {
            Func<KeyValuePair<int, string>, bool> isWidthArg = x => x.Value == "-w" || x.Value == "/w";
            switch (inputArgs.Count(isWidthArg))
            {
                case 0:
                    break;
                case 1:
                    inputArgs.TryGetValue(inputArgs.First(isWidthArg).Key + 1, out string inputWidth);
                    try
                    {
                        parsedConfig.ImgWidth = int.Parse(inputWidth);
                    }
                    catch (Exception e)
                    {
                        WriteLine($"Error: {e.Message} Please check your input and try again. ");
                    }
                    break;
                default:
                    WriteLine("Error: -w argument can only be used once. Please check your input and try again. ");
                    break;
            }
        }
    }
}
