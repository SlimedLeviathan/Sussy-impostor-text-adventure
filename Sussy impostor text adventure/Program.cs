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
            Question adminHall = new Question("The admin hall connects the cafeteria, admin room and the storage room, where would you like to go?", new string[] { "c", "a", "s" });
            Question medBayHall = new Question("The medbay hall connects the medbay, cafeteria and upper engine, where would you like to go?", new string[] { "c", "m", "e" });
            Question reactorHall = new Question("The reactor hallway connects the upper and lower engines, the reactor and the security room. Where would you like to go?", new string[] { "u", "l", "r", "c" });
            Question navHall = new Question("The navigation hallway connects the oxygen, weapons, navigation and shields. Where would you like to go?", new string[] { "w", "o", "n", "s" });
            Question commsHall = new Question("The comunications hallway connects the shields, comunications and storage room. Where would you like to go?", new string[] { "c", "st", "sh" });
            Question elecHall = new Question("The elecrtical hall connects the lower engine, electrical and storage rooms. Where would you like to go?", new string[] { "e", "s", "e" });
            Question weapons = new Question("A seat sits in the middle of the room, on its armrests are control sticks to move the laser outside the ship. You can go to either the cafeteria or the navigation hall from here.", new string[] { "c", "n" });
            Question oxygen = new Question("The oxygen room circulates and filters the oxygen for the entire ship. From here you can go back into the navigation hall.", new string[] { "h" });
            Question nav = new Question("The navigation room controls where the ship is going, you can go back into the hallway from here.", new string[] { "h" });
            Question shields = new Question("The shields room protects the ship from outside dangers, you can either go into the communications hallway or the navigation hallway.", new string[] { "c", "n" });
            Question comms = new Question("You forgot why you came in here... You can go back into the hall.", new string[] { "h" });
            Question storage = new Question("This room stores important cargo from the ship and connects multiple hallways. You can either go into the electrical hallway, the communications hallway or the admin hallway.", new string[] { "c", "a", "e" });
            Question admin = new Question("The admin room is where the crewmates unsuccessfully swipe their cards and also shows the number of crewmates in each room. You can look at the table or go back into the hallway.", new string[] { "h", "t" });
            Question elec = new Question("Electrical... spooky. The only way you can go is back into the hallway.", new string[] { "h" });
            Question lowerE = new Question("The lower of the two engines, it connects the electrical hallway to the reactor hallway.", new string[] { "e", "r" });
            Question upperE = new Question("The upper engine, it connects the reactor hallway to the medbay hallway.", new string[] { "m", "r" });
            Question sec = new Question("Security, where you are able to spy on everybody.... wherever there is a camera. You can look at the cameras or go back into the hallway.", new string[] { "h", "s" });
            Question reac = new Question("Reactor - the powerhouse of the ce- ship, I meant ship. You can go back into the hallway from here.", new string[] { "h" });
            Question med = new Question("The medbay, where hurt crewmates are housed and nursed back to health. You can go into the hallway from here.", new string[] { "h" });

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
                        assignCrews(crewNum, new Question[] { cafe, elec, weapons, reac, sec, med, shields, admin, nav, oxygen, storage, upperE, lowerE});
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