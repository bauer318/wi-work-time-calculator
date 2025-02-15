using System.Text.RegularExpressions;

namespace WorkTimeCalculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PrintMainMenu();
            try
            {
                bool keepRunning = true;
               
                while (keepRunning)
                {
                    string? chooseen = Console.ReadLine();
                    if (!string.IsNullOrEmpty(chooseen))
                    {
                        switch (chooseen)
                        {
                            case "0":
                                keepRunning = false;
                                break;
                            case "1":
                                Console.Clear();
                                PrintMainMenu();
                                break;
                            default:
                                Console.WriteLine("Wrong option was chosen!!!" +
                                    "");
                                PrintMainMenu();
                                break;
                        }
                    }
                    else
                    {
                        ComputeWorkTime();
                    }

                }

            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void ComputeWorkTime()
        {
            DateTime currentDate = DateTime.Now;
            int currentYear = currentDate.Year;
            int currentMonth = currentDate.Month;
            int currentDay = currentDate.Day;
            int totalHours = 0;
            int totalMinutes = 0;
            int computingIndex = 0;
            while (true)
            {
                PrintTimeComputingMenu(computingIndex);
                string? inputTime = Console.ReadLine();
                if (!string.IsNullOrEmpty(inputTime))
                {
                    string inputTimeWithoutWhiteSpace = Regex.Replace(inputTime, @"\s+", "");
                    bool isCorrectInputTimeFormat = Regex.IsMatch(inputTimeWithoutWhiteSpace, "[0-9]{2}:[0-9]{2}->[0-9]{2}:[0-9]{2}");
                    if (isCorrectInputTimeFormat)
                    {
                        string[] inputTimeSplit = inputTimeWithoutWhiteSpace.Split("->");
                        string[] startTime = inputTimeSplit[0].Split(":");
                        string[] endTime = inputTimeSplit[1].Split(":");
                        int startHour = Int32.Parse(startTime[0]);  
                        int startMinute = Int32.Parse(startTime[1]);
                     
                        int endHour = Int32.Parse(endTime[0]);
                        int endMinute = Int32.Parse(endTime[1]);
                        if (!IsCorrectHour(startHour) || 
                            !IsCorrectMinute(startMinute) ||
                            !IsCorrectHour(endHour) ||
                            !IsCorrectMinute(endMinute))
                        {
                            Console.WriteLine("Wrong value of hour or minute. Note 00 <= HH <= 23 and 00 <= MM <= 59");
                            continue;
                        }
                        if(startHour==endHour && endMinute < startMinute)
                        {
                            Console.WriteLine("Wrong end minutes");
                            continue;
                        }
                        DateTime startDateTime = new(currentYear, currentMonth, currentDay, startHour, startMinute, 00);
                        //TODO Create method to encapsulate this logic
                        if (endHour < startHour)
                        {
                            int incrementedIndex = TryToFindNextDate(currentDay, currentMonth, currentYear);
                            if (incrementedIndex == 0)
                            {
                                currentYear++;
                                currentDay = 1;
                                currentMonth = 1;

                            }
                            else if (incrementedIndex == 1)
                            {
                                currentDay = 1;
                                currentMonth++;

                            }
                            else
                            {
                                currentDay++;

                            }
                        }

                        DateTime endDateTime = new(currentYear, currentMonth, currentDay, endHour, endMinute, 00);

                        TimeSpan durration = endDateTime.Subtract(startDateTime);
                        totalHours += durration.Hours;
                        totalMinutes += durration.Minutes;

                        if (totalMinutes > 60)
                        {
                            var convertedMinutes = ConvertMinutesToHourMinutes(totalHours, totalMinutes);
                            totalHours = convertedMinutes.Item1;
                            totalMinutes = convertedMinutes.Item2;
                        }

                        Console.WriteLine($"\t\t\t\t{totalHours}h : {totalMinutes}min");
                        computingIndex++;
                    }
                    else
                    {
                        if (Int32.TryParse(inputTime, null, out var result) && result == 0)
                        {
                            PrintMainMenu();
                            break;

                        }
                        Console.WriteLine("Incorrect input time format");
                    }
                }
            }
        }
        private static (int, int) ConvertMinutesToHourMinutes(int currentHours, int currentMinutes)
        {
            int oneHourToMinutes = 60;
            int newMinutes = currentMinutes % oneHourToMinutes;
            int newHours = currentMinutes / oneHourToMinutes;

            return (currentHours + newHours, newMinutes);
        }
        private static int SetToDefaultValueIfIncorrect(int value, bool isMinute=false)
        {
            if (isMinute)
            {
                if (!IsCorrectMinute(value))
                {
                    Console.WriteLine($"{value} wrong minutes value");
                    return 0;
                }
                return value;
            }

            if (IsCorrectHour(value))
            {
                return value;
            }
            Console.WriteLine($"{value} wrong hours value");
            return 0;
        }
        private static bool IsCorrectHour(int hour) => hour >= 0 && hour <= 23;
        private static bool IsCorrectMinute(int minute) => minute >= 0 && minute <= 59;

        private static int TryToFindNextDate(int currentDay, int currentMonth, int currentYear)
        {
            try
            {
                _ = new DateTime(currentYear, currentMonth, ++currentDay);
                int dayIncrementedIndex = 2;
                return dayIncrementedIndex;

            }
            catch (ArgumentOutOfRangeException)
            {
                try
                {
                    currentDay = 1;
                    _ = new DateTime(currentYear, ++currentMonth, currentDay);
                    int monthIncrementedIndex = 1;
                    return monthIncrementedIndex;
                }
                catch (ArgumentOutOfRangeException)
                {
                    currentDay = 1;
                    currentMonth = 1;
                    _ = new DateTime(++currentYear, currentDay, currentMonth);
                    int yearIncrementedIndex = 0;
                    return yearIncrementedIndex;
                }
            }

        }

        private static void PrintMainMenu()
        {
            Console.WriteLine("*****Menu******\n0. Stop\n1.Clear display\nEnter. Calculate\n");
        }

        private static void PrintTimeComputingMenu(int index)
        {
            Console.Write($"Tape 0 to stop\n{index}) Time format [HH:MM->HH:MM] : ");
        }
    }
}
