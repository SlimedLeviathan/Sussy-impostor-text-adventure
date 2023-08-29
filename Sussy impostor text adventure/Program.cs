using System;
using System.Linq;
using System.Threading;

namespace Sussy_impostor_text_adventure
{
    internal class Program
    {
        // This class handels all of the questions in the game, it makes sure that the user won't input any wrong inputs and sends them to and from rooms
        // Will handle any interactions with impostors and crewmates
        public static int questionLoop(string question, string[] answers)
        {
            bool done = false;
            int endNum = -1;

            string options = " (";

            for (int i = 0; i < answers.Length; i++)
            {
                options += answers[i];

                if (i != answers.Length - 1)
                {
                    options += '/';
                }
            }

            options += ")";

            while (!done)
            {
                Console.Write(question + options + " : ");

                string input = Console.ReadLine().ToLower();

                Console.Clear();

                for (int num = 0; num < answers.Length; num++)
                {
                    if (answers[num] == input)
                    {
                        done = true;
                        endNum = num;
                    }
                }

                if (!done)
                {
                    Console.WriteLine("Your input was not able to be detected, please try again.");
                }
            }
            return endNum;
        }
        public class Question
        {
            private string question;
            public bool task = false;
            public string[] answers;
            public Question[] results;
            public int npcs;
            public int newNpcs;

            public Question(string question, string[] answers) 
            {
                this.question = question;
                this.answers = answers;
            }

            public void setNewResults(Question[] results)
            {
                this.results = results;
            }

            public int askQuestion()
            {
                return questionLoop(this.question, this.answers);
            }


            public void answerToQuestion()
            {
                this.results[askQuestion()].answerToQuestion();
            }
        }

        static void Main(string[] args)
        {
            // Making all of the rooms a player can be

            Question cafe = new Question("The cafeteria, where the crewmates go to eat and meetings are held. Would you like to go to Weapons, the admin hallway or medbay hallway?", new string[] { "w", "a", "m" });
            Question adminHall = new Question("Would you like to be an impostor or a crewmate?", new string[] { "c", "a", "s" });
            Question medBayHall = new Question("Would you like to be an impostor or a crewmate?", new string[] { "c", "m", "e" });
            Question reactorHall = new Question("Would you like to be an impostor or a crewmate?", new string[] { "u", "l", "r", "c" });
            Question navHall = new Question("Would you like to be an impostor or a crewmate?", new string[] { "w", "o", "n", "s" });
            Question commsHall = new Question("Would you like to be an impostor or a crewmate?", new string[] { "c", "st", "sh" });
            Question elecHall = new Question("Would you like to be an impostor or a crewmate?", new string[] { "e", "s", "e" });
            Question weapons = new Question("Would you like to be an impostor or a crewmate?", new string[] { "c", "n" });
            Question oxygen = new Question("Would you like to be an impostor or a crewmate?", new string[] { "i", "c" });
            Question nav = new Question("Would you like to be an impostor or a crewmate?", new string[] { "i", "c" });
            Question shields = new Question("Would you like to be an impostor or a crewmate?", new string[] { "i", "c" });
            Question comms = new Question("Would you like to be an impostor or a crewmate?", new string[] { "i", "c" });
            Question storage = new Question("Would you like to be an impostor or a crewmate?", new string[] { "i", "c" });
            Question admin = new Question("Would you like to be an impostor or a crewmate?", new string[] { "i", "c" });
            Question elec = new Question("Would you like to be an impostor or a crewmate?", new string[] { "i", "c" });
            Question lowerE = new Question("Would you like to be an impostor or a crewmate?", new string[] { "i", "c" });
            Question upperE = new Question("Would you like to be an impostor or a crewmate?", new string[] { "i", "c" });
            Question sec = new Question("Would you like to be an impostor or a crewmate?", new string[] { "i", "c" });
            Question reac = new Question("Would you like to be an impostor or a crewmate?", new string[] { "i", "c" });
            Question med = new Question("Would you like to be an impostor or a crewmate?", new string[] { "i", "c" });

            // Text adventure where you are either a crewmate or an impostor
            // youll get a summary at the end to show how many people you killed or tasks you completed
            Console.WriteLine("Welcome to the Among Us Text Game!");

            int playerRole = questionLoop("Would you like to be an impostor or a crewmate?", new string[] { "i", "c"}); // 0 is impostor 1 is crewmate

            if (playerRole == 0)
            {
                bool done = false;

                while (!done)
                {
                    Console.Write("How many crewmates would you like to be on the ship with you? : ");

                    string crews = Console.ReadLine();

                    Console.Clear();

                    try
                    {
                        int crewNum = int.Parse(crews);
                        assignCrews(crewNum, new Question[] { cafe, elec, weapons, reac, sec, med, comms, shields, admin, nav, oxygen, storage, upperE, lowerE});
                        done = true;
                    }
                    
                    catch (Exception)
                    {
                        Console.WriteLine("You did not input a number, please try again.");
                    }
                }
            }

            else if (playerRole == 1)
            {
                bool done = false;

                while (!done)
                {
                    Console.Write("How many tasks would you like there to be? (Max of ): ");

                    string tasks = Console.ReadLine();

                    Console.Clear();

                    try
                    {
                        int taskNum = int.Parse(tasks);
                        assignTasks(taskNum, new Question[] {cafe, reactorHall, adminHall, elec, navHall, storage, reac, admin, nav, reac, med, weapons, upperE, lowerE}, upperE, lowerE);
                        done = true;
                    }

                    catch (Exception)
                    {
                        Console.WriteLine("You did not input a number, please try again.");
                    }
                }
            }



        }
        public static void assignTasks(int taskNum, Question[] questionArray, Question upperE, Question lowerE) //Assigns tasks to rooms based on a number
        {
            Random random = new Random();

            Question[] newArray = questionArray;

            for (int num = 0; num <= taskNum; num++)
            {
                int randNum = random.Next(0, newArray.Length);


                if ((newArray[randNum] == upperE | newArray[randNum] == lowerE) & num <= taskNum - 2)
                {
                    upperE.task = true;
                    lowerE.task = true;
                    num++;

                    int upperENum = Array.IndexOf(questionArray, upperE);
                    int lowerENum = Array.IndexOf(questionArray, lowerE);

                    newArray = new Question[] { };

                    for (int arrayNum = 0; num <= questionArray.Length; arrayNum++)
                    {
                        if (arrayNum != upperENum & arrayNum != lowerENum)
                        {
                            newArray.Append(questionArray[arrayNum]);
                        }
                    }
                }
                else
                {
                    newArray[randNum].task = true;

                    newArray = new Question[] { };

                    for (int arrayNum = 0; num <= questionArray.Length; arrayNum++)
                    {
                        if (arrayNum != randNum)
                        {
                            newArray.Append(questionArray[arrayNum]);
                        }
                    }
                }

            }

        }

        public static void assignCrews(int crewNum, Question[] questionArray) //Assigns tasks to rooms based on a number
        {
            Random random = new Random();

            for (int num = 0; num <= crewNum; num++)
            {
                int randNum = random.Next(0, questionArray.Length);

                questionArray[randNum].npcs += 1;

                questionArray = new Question[] { };

            }
        }
        
    }
}