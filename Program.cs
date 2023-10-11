using System;
using System.Collections;
using System.Dynamic;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.FileIO;

namespace HelloWorld
{
    public class Program
    {
        static ArrayList header = new ArrayList();
        static ArrayList dataLines = new ArrayList();

        static void Main(string[] args)
        {
            string fileName = "data.csv";
            Predict(fileName);
        }

        /// <summary>
        /// Predicts the probability of a patient having covid based on the data in the given file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static float[] Predict(string fileName)
        {
            readCsv(fileName);

            string classLabel = "Got covid";
            string classValue = "1";

            int numberOfClass = getNumberOf(classLabel, classValue);
            int numberOfNotClass = getNumberOf(classLabel, "0");

            Dictionary<string, int> jointCounts1 = getJointCounts(classLabel, classValue);
            Dictionary<string, int> jointCounts2 = getJointCounts(classLabel, "0");

            applyLaplacianSmoothing(jointCounts1);
            applyLaplacianSmoothing(jointCounts2);

            float z1 = CalculateJointCountProbability(jointCounts1, numberOfClass);
            float z2 = CalculateJointCountProbability(jointCounts2, numberOfNotClass);

            float normalizedConstant = z1 + z2;

            float p1 = z1 / normalizedConstant;
            float p2 = z2 / normalizedConstant;

            Console.WriteLine("P(" + classLabel + " = " + classValue + ") = " + p1);
            Console.WriteLine("P(" + classLabel + " = " + "0" + ") = " + p2);

            float[] result = { p1, p2 };
            return result;
        }

        /// <summary>
        /// Calculates the probability of a given set of joint counts occurring, given the number of classes and the total number of possible joint counts.
        /// </summary>
        /// <param name="jointCounts">A dictionary containing the joint counts for each attribute-value pair and class label.</param>
        /// <param name="numClasses">The total number of classes in the dataset.</param>
        /// <returns>The probability of the given set of joint counts occurring.</returns>
        public static float CalculateJointCountProbability(Dictionary<string, int> jointCounts, int numClasses)
        {
            float probability = 0;
            foreach (KeyValuePair<string, int> kvp in jointCounts)
            {
                // Calculate the probability of this joint count occurring
                float countProbability = (float)kvp.Value / ((float)numClasses + (float)jointCounts.Count);

                // Multiply the probability by the current value of probability
                if (probability == 0)
                {
                    probability = countProbability;
                }
                else
                {
                    probability *= countProbability;
                }
            }

            return probability;
        }

        /// <summary>
        /// Applies Laplacian smoothing to the given joint counts.
        /// </summary>
        /// <param name="jointCounts"></param>
        public static void applyLaplacianSmoothing(Dictionary<string, int> jointCounts)
        {
            //add 1 to each value
            foreach (KeyValuePair<string, int> kvp in jointCounts)
            {
                jointCounts[kvp.Key] += 1;
            }
        }

        /// <summary>
        /// Returns a dictionary containing the joint counts for each attribute-value pair and class label.
        /// </summary>
        /// <param name="classLabel"></param>
        /// <param name="classValue"></param>
        /// <returns></returns>
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
                    foreach (string value in line)
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

        /// <summary>
        /// Returns the number of times the given value appears in the given column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Reads the given csv file into the header and dataLines ArrayLists.
        /// </summary>
        /// <param name="filename"></param>
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