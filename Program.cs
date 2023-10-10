using System;
using System.Collections;
using System.Dynamic;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.FileIO;

namespace HelloWorld
{
    class Program
    {
        static ArrayList header = new ArrayList();
        static ArrayList dataLines = new ArrayList();

        static void Main(string[] args)
        {
            readCsv("data.csv");

            string classLabel = "Got covid";
            string classValue = "1";

            int numberOfClass = getNumberOf(classLabel, classValue);
            int numberOfNotClass = getNumberOf(classLabel, "0");

            Dictionary<string, int> jointCounts1 = getJointCounts(classLabel , classValue);
            Dictionary<string, int> jointCounts2 = getJointCounts(classLabel , "0");

            applyLaplacianSmoothing(jointCounts1);
            applyLaplacianSmoothing(jointCounts2);

            float z1 = z(jointCounts1, numberOfClass);
            float z2 = z(jointCounts2, numberOfNotClass);

            float normalizedConstant = z1 + z2;

            float p1 = z1 / normalizedConstant;
            float p2 = z2 / normalizedConstant;

            Console.WriteLine("P(" + classLabel + " = " + classValue + ") = " + p1);
            Console.WriteLine("P(" + classLabel + " = " + "0" + ") = " + p2);
        }

        //seriously what the fuck is z
        static float z(Dictionary<string, int> jointCounts, int numberOfClass)
        {
            float z = 0;
            foreach (KeyValuePair<string, int> kvp in jointCounts)
            {
                if(z == 0)
                {
                    z =(float)kvp.Value / ((float)numberOfClass + (float)jointCounts.Count);
                }
                else
                {
                    z *= (float)kvp.Value / ((float)numberOfClass + (float)jointCounts.Count);
                }
            }

            return z;
        }

        static void applyLaplacianSmoothing(Dictionary<string, int> jointCounts)
        {
            //add 1 to each value
            foreach (KeyValuePair<string, int> kvp in jointCounts)
            {
                jointCounts[kvp.Key] += 1;
            }
        }

        static Dictionary<string, int> getJointCounts(string classLabel, string classValue)
        {
            Dictionary<string, int> jointCounts = new Dictionary<string, int>();

            int classIndex = header.IndexOf(classLabel);

            foreach (string[] line in dataLines)
            {
                foreach (string value in line)
                {
                    if (!jointCounts.ContainsKey(value) && value != "0" && value != "1")
                    {
                        jointCounts.Add(value, 0);
                    }
                }
            }

            foreach (string[] line in dataLines)
            {
                if (line[classIndex] == classValue)
                {
                    foreach(string value in line)
                    {
                        if (value != "0" && value != "1")
                        {
                            jointCounts[value] += 1;
                        }
                    }

                }
            }

            return jointCounts;
        }

        static int getNumberOf(string columnName, string value)
        {
            int index = header.IndexOf(columnName);
            int count = 0;
            foreach (string[] line in dataLines)
            {
                if (line[index] == value)
                {
                    count++;
                }
            }
            
            return count;
        }


        static void readCsv(string filename)
        {
            using (TextFieldParser parser = new TextFieldParser(filename))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                bool firstLine = true;
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    if (firstLine)
                    {
                        foreach (string field in fields)
                        {
                            header.Add(field);
                        }
                        firstLine = false;
                    }
                    else
                    {
                        dataLines.Add(fields);
                    }
                }
            }
        }
        
    }
}