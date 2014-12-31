using System;
using System.Collections.Generic;
using System.Text;


namespace MultRangerWIconExpress
{
    class DataProcessor
    {
        /// <summary>
        /// Accessing generic data using correct data type for each subcomponent
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="resultString"></param>
        public static void accessData(Sick.Icon.IIconBuffer buffer, out String resultString)
        {
            resultString = "";
            int numberOfScans = buffer.Height;
            
            // Check if it is an image buffer
            if (buffer.Format.Name.Equals("IMAGE"))
            {
                resultString += "This is an image buffer.\n";
                return;
            }

            // Loop through all components in the buffer
            foreach (Sick.Icon.IIconBufferComponent component in buffer.Components)
            {
                // Loop through all subcomponents of the component.
                foreach (Sick.Icon.IIconBufferSubComponent subComponent in component.SubComponents)
                {
                    // Handle Mark subcomponents separately and only print mark value from the first line scan.
                    // See the reference manual for a detailed description of the mark data format.
                    if (subComponent.Format.Name.Equals("Mark"))
                    {
                        int[,] markData = buffer.Components[component.Format.Name].SubComponents[subComponent.Format.Name].GetRows<int>(0, 1);
                        int counter = markData[0, 0];
                        byte overtrig = (byte)(markData[0, 1] >> 16);
                        byte enable = (byte)((markData[0, 1] >> 30) & 0x01);
                        resultString += String.Format("{0,-15}{1,-15}{2,-12}{3,8}{4,18}{5,-8}{6,12}{7,-4}\n", component.Format.Name,
                                                      subComponent.Format.Name, "Counter: ", counter, "Overtrig: ", overtrig, "Enable: ", enable);
                    }

                    else    // Calculate and print mean value for non-mark subcomponents.
                    {
                        double sum = 0;
                        uint count = 0;

                        // Since the data type is unknown in the general case it must first be determined so that the 
                        // data pointer and value types can be declared correctly.
                        switch (subComponent.Format.ValueType)
                        {
                            // Subcomponent data are 8 bit unsigned integers.
                            case "BYTE":
                                byte byteValue;
                                byte[,] byteData = buffer.Components[component.Format.Name].SubComponents[subComponent.Format.Name].GetRows<byte>(0, numberOfScans);
                                for (int scan = 0; scan < numberOfScans; scan++)
                                {
                                    for (int col = 0; col < subComponent.Format.Width; col++)
                                    {
                                        byteValue = byteData[scan, col];
                                        if (byteValue != 0)  // Missing data is represented with a 0.
                                        {
                                            sum += byteValue;
                                            count++;
                                        }
                                    }
                                }
                                break;

                            // Subcomponent data are 16 bit unsigned integers.
                            case "WORD":
                                ushort wordValue;
                                ushort[,] wordData = buffer.Components[component.Format.Name].SubComponents[subComponent.Format.Name].GetRows<ushort>(0, numberOfScans);
                                for (int scan = 0; scan < numberOfScans; scan++)
                                {
                                    for (int col = 0; col < subComponent.Format.Width; col++)
                                    {
                                        wordValue = wordData[scan, col];
                                        if (wordValue != 0)  // Missing data is represented with a 0.
                                        {
                                            sum += wordValue;
                                            count++;
                                        }
                                    }
                                }
                                break;

                            // Subcomponent data are 32 bit unsigned integers.
                            case "DWORD":
                                uint dwordValue;
                                uint[,] dwordData = buffer.Components[component.Format.Name].SubComponents[subComponent.Format.Name].GetRows<uint>(0, numberOfScans);
                                for (int scan = 0; scan < numberOfScans; scan++)
                                {
                                    for (int col = 0; col < subComponent.Format.Width; col++)
                                    {
                                        dwordValue = dwordData[scan, col];
                                        if (dwordValue != 0)  // Missing data is represented with a 0.
                                        {
                                            sum += dwordValue;
                                            count++;
                                        }
                                    }
                                }
                                break;

                            // Subcomponent data are 32 bit signed integers.
                            case "INT":
                                int intValue;
                                int[,] intData = buffer.Components[component.Format.Name].SubComponents[subComponent.Format.Name].GetRows<int>(0, numberOfScans);
                                for (int scan = 0; scan < numberOfScans; scan++)
                                {
                                    for (int col = 0; col < subComponent.Format.Width; col++)
                                    {
                                        intValue = intData[scan, col];
                                        if (intValue != 0)  // Missing data is represented with a 0.
                                        {
                                            sum += intValue;
                                            count++;
                                        }
                                    }
                                }
                                break;

                            // Subcomponent data are 32 bit floating point numbers.
                            case "FLOAT":
                                float floatValue;
                                float[,] floatData = buffer.Components[component.Format.Name].SubComponents[subComponent.Format.Name].GetRows<float>(0, numberOfScans);
                                for (int scan = 0; scan < numberOfScans; scan++)
                                {
                                    for (int col = 0; col < subComponent.Format.Width; col++)
                                    {
                                        floatValue = floatData[scan, col];
                                        if (floatValue != 0) // Missing data is represented with a 0.
                                        {
                                            sum += floatValue;
                                            count++;
                                        }
                                    }
                                }
                                break;

                            // Subcomponent data are 32 bit clusters of 8 bit color data organized like [B G R 0].
                            case "RGB2":
                                dwordData = buffer.Components[component.Format.Name].SubComponents[subComponent.Format.Name].GetRows<uint>(0, numberOfScans);
                                for (int scan = 0; scan < numberOfScans; scan++)
                                {
                                    for (int col = 0; col < subComponent.Format.Width; col++)
                                    {
                                        dwordValue = dwordData[scan, col];
                                        if (dwordValue != 0) // Missing data is represented with a 0.
                                        {
                                            byte blue = (byte)(dwordValue >> 24);
                                            byte green = (byte)(dwordValue >> 16);
                                            byte red = (byte)(dwordValue >> 8);
                                            sum += (blue + green + red);
                                            count = count + 3;
                                        }
                                    }
                                }
                                break;
                        }

                        double mean = (double)sum / (double)count;
                        resultString += String.Format("{0,-15}{1,-15}{2,-12}{3,8:N2}\n", component.Format.Name,
                                                      subComponent.Format.Name, "Mean value: ", mean);
                    }
                }
            }
        }
    }
}
