using System;
using System.Collections.Generic;
using System.IO;

namespace GradingSystem
{
    public class Student
    {
        public int Id;
        public string FullName = "";
        public int Score;

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70 && Score <= 79) return "B";
            if (Score >= 60 && Score <= 69) return "C";
            if (Score >= 50 && Score <= 59) return "D";
            return "F";
        }
    }

    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath, out List<string> errors)
        {
            var students = new List<Student>();
            errors = new List<string>();

            using (var reader = new StreamReader(inputFilePath))
            {
                string? line;
                int lineNo = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNo++;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    try
                    {
                        var parts = line.Split(',');
                        if (parts.Length != 3)
                            throw new MissingFieldException($"Line {lineNo}: Expected 3 fields, found {parts.Length}.");

                        string idText = parts[0].Trim();
                        string nameText = parts[1].Trim();
                        string scoreText = parts[2].Trim();

                        if (string.IsNullOrWhiteSpace(idText) || string.IsNullOrWhiteSpace(nameText) || string.IsNullOrWhiteSpace(scoreText))
                            throw new MissingFieldException($"Line {lineNo}: One or more fields are empty.");

                        if (!int.TryParse(idText, out int id))
                            throw new Exception($"Line {lineNo}: Invalid ID format ('{idText}').");

                        if (!int.TryParse(scoreText, out int score))
                            throw new InvalidScoreFormatException($"Line {lineNo}: Score '{scoreText}' is not a valid integer.");

                        students.Add(new Student
                        {
                            Id = id,
                            FullName = nameText,
                            Score = score
                        });
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex.Message);
                    }
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath))
            {
                foreach (var s in students)
                {
                    string line = $"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}";
                    writer.WriteLine(line);
                }
            }
        }
    }

    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Grading System");
            string input = "students_input.txt";
            string output = "students_report.txt";

            try
            {
                var processor = new StudentResultProcessor();
                var students = processor.ReadStudentsFromFile(input, out List<string> errors);

                processor.WriteReportToFile(students, output);

                Console.WriteLine($"Report generated successfully. {students.Count} valid record(s) written.");
                if (errors.Count > 0)
                {
                    Console.WriteLine("\nErrors encountered:");
                    foreach (var err in errors)
                        Console.WriteLine(" - " + err);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error: Input file not found. Make sure 'students_input.txt' exists.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
