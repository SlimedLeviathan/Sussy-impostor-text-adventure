using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sussy_impostor_text_adventure
{
    internal class Program
    {
        // This class handels all of the questions in the game, it makes sure that the user won't input any wrong inputs and sends them to and from rooms
        // Will handle any interactions with impostors and crewmates
        public static int questionLoop(string question, string extras, string[] answers)
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
                print(question + extras + options + " : ", 10);

                string input = Console.ReadLine().ToLower();

                Console.Clear();

                for (int num = 0; num < answers.Length; num++)
                {
                    if (input == "help")
                    {
                        tutorial();
                    }

                    else if (input == "wait")
                    {
                        done = true;
                        endNum = -1;
                    }

                    else if (input == "interact")
                    {
                        done = true;
                        endNum = -2;
                    }

                    else if (input == answers[num])
                    {
                        done = true;
                        endNum = num;
                    }
                }

                if (!done & input != "help")
                {
                    printLine("Your input was not able to be detected, please try again.", 1);
                }
            }
            return endNum;
        }
        public class Question
        {
            private string question;
            public string[] answers;
            public Question[] results;
            public int imps;
            public int crews;
            public int tasks = 0;

            public Question(string question, string[] answers) 
            {
                this.question = question;
                this.answers = answers;
            }

            public void setNewResults(Question[] results)
            {
                this.results = results;
            }

            public int askQuestion(string extras, int playerRole)
            {
                string[] answers = this.answers;

                answers.Append("wait");

                if (this.tasks > 0 & playerRole == 1)
                {
                    extras += $"\nThere are {this.tasks} tasks left to be done in this room.";
                    answers.Append("interact");
                }

                if (this.crews > 0 & playerRole == 0)
                {
                    answers.Append("interact");
                }

                return questionLoop(this.question, extras, answers);
            }


            public void answerToQuestion(Room[] rooms, Question[] specials, Room[] adminRooms, Room[] halls, int playerRole, int points)
            {

                string extras = "";
                   
                if (this == specials[0]) 
                {
                    // show how many npcs are in each room
                    extras += adminTable(adminRooms);
                }

                else if (this == specials[1])
                {
                    //show how many npcs there are in the hallways (excluding ele)
                    extras += secCams(halls);
                }

                if (playerRole == 0)
                {
                    if (this.crews == 1)
                    {
                        extras += "\nThere is a crewmate the room with you...";
                    }

                    else if (this.crews > 1)
                    {
                        extras += $"\nThere are {this.crews + this.imps} crewmates in the room with you...";
                    }

                    if (this.imps == 1)
                    {
                        extras += "\nThere is an impostor the room with you...";
                    }

                    else if (this.imps > 1)
                    {
                        extras += $"\nThere are {this.crews + this.imps} impostors in the room with you...";
                    }
                }

                else if (playerRole == 1)
                {
                    if (this.crews + this.imps == 1)
                    {
                        extras += "There is someone else in the room with you...";
                    }

                    else if (this.crews + this.imps > 1)
                    {
                        extras += $"There are {this.crews + this.imps} people in the room with you...";
                    }
                }

                int input = askQuestion(extras, playerRole);

                bool dead = false;

                if (input != -1) // waiting
                {

                    if (this.crews + 1 < this.imps)
                    {
                        printLine("The impostors were able to kill all the crewmates in the room in one swift motion...", 10);

                        dead = true;
                    }
                }

                else if (input == -2)
                {
                    if (playerRole == 1)
                    {

                        if (this.crews + 1 >= this.imps)
                        {
                            printLine("You successfully completed one task!", 8);

                            points += 1;
                            this.tasks -= 1;
                        }

                        if (this.crews + 1 < this.imps)
                        {
                            printLine("The impostors were able to kill all the crewmates in the room in one swift motion...", 10);

                            dead = true;
                        }
                    }

                    else if (playerRole == 0)
                    {
                        if (this.imps + 1 >= this.crews)
                        {
                            if (this.imps != 0)
                            {
                                printLine("You and the other impostors in the room killed all the crewmates in the room simultaneously!", 10);
                            }

                            else
                            {
                                printLine("You killed the lone crewmate.", 12);
                            }

                            points += 1;
                            this.crews = 0;
                        }

                        if (this.imps + 1 < this.crews)
                        {
                            printLine("There weren't enough impostors in the room to kill all of the crewmates, and you were caught...", 10);

                            dead = true;
                        }
                    }
                }

                var room = this.results[input];

                if (room.GetType() == typeof(Room))
                {
                    turn(rooms);
                }

                if (checkAllRooms(rooms, playerRole))
                {
                    if (playerRole == 0)
                    {
                        printLine("All of the crewmates have been killed!", 10);
                    }

                    else if (playerRole == 0)
                    {
                        printLine("All of your tasks have been completed!", 10);
                    }

                    dead = true;
                }

                // if they are dead, they will go back to the end of start game and then into the endgame function
                if (!dead)
                {
                    room.answerToQuestion(rooms, specials, adminRooms, halls, playerRole, points);
                }

            }
        }


        public class Room : Question
        {
            public string name;
            public int newImps;
            public int newCrews;

            public Room(string name, string question, string[] answers, Room[] rooms) : base(question, answers)
            {
                this.name = name;
                rooms.Append(this);
            }
            
            public void moveNPCs()
            {
                Random random = new Random();

                // finding which questions npcs can go into
                Room[] movableRooms = new Room[] { };

                foreach (Question question in this.results) 
                { 
                    if (question is Room)
                    {
                        movableRooms.Append(question);
                    }
                }

                // auto impostor kill
                if (this.imps > this.crews)
                {
                    this.crews = 0;
                }

                // setting crews
                for (int c = 0; c < this.crews; c++)
                {

                    int num = random.Next(0, movableRooms.Length + 1);

                    if (num == 0)
                    {
                        this.newCrews += 1;
                    }

                    else
                    {
                        movableRooms[num - 1].newCrews += 1;
                    }

                }

                for (int i = 0; i < this.imps; i++)
                {
                    int num = random.Next(0, movableRooms.Length + 1);

                    if (num == 0)
                    {
                        this.newImps += 1;
                    }
                    else
                    {
                        movableRooms[num - 1].newImps += 1;
                    }
                }
            }

            public void setNPCs()
            {
                this.crews = newCrews;
                this.imps = newImps;

                this.newCrews = 0;
                this.newImps = 0;
            }

        }

        static void Main(string[] args)
        {

            // An array for all of the rooms, so that the npcs 
            Room[] rooms = new Room[] { };

            // Making all of the rooms a player can be

            Room cafe = new Room("cafeteria", "The cafeteria, where the crewmates go to eat and meetings are held. Would you like to go to Weapons, the admin hallway or medbay hallway?", new string[] { "w", "a", "m" }, rooms);
            Room adminHall = new Room("admin hall", "The admin hall connects the cafeteria, admin room and the storage room, where would you like to go?", new string[] { "c", "a", "s" }, rooms);
            Room medHall = new Room("medbay hall", "The medbay hall connects the medbay, cafeteria and upper engine, where would you like to go?", new string[] { "c", "m", "e" }, rooms);
            Room reacHall = new Room("reactor hall", "The reactor hallway connects the upper and lower engines, the reactor and the security room. Where would you like to go?", new string[] { "u", "l", "r", "c" }, rooms);
            Room navHall = new Room("navigation hall", "The navigation hallway connects the oxygen, weapons, navigation and shields. Where would you like to go?", new string[] { "w", "o", "n", "s" }, rooms);
            Room commsHall = new Room("communications hall", "The comunications hallway connects the shields, comunications and storage room. Where would you like to go?", new string[] { "c", "st", "sh" }, rooms);
            Room elecHall = new Room("electric hall", "The elecrtical hall connects the lower engine, electrical and storage rooms. Where would you like to go?", new string[] { "e", "s", "l" }, rooms);
            Room weapons = new Room("weapons", "A seat sits in the middle of the room, on its armrests are control sticks to move the laser outside the ship. You can go to either the cafeteria or the navigation hall from here.", new string[] { "c", "n" }, rooms);
            Room oxygen = new Room("oxygen", "The oxygen room circulates and filters the oxygen for the entire ship. From here you can go back into the navigation hall.", new string[] { "h" }, rooms);
            Room nav = new Room("navigations", "The navigation room controls where the ship is going, you can go back into the hallway from here.", new string[] { "h" }, rooms);
            Room shields = new Room("shields", "The shields room protects the ship from outside dangers, you can either go into the communications hallway or the navigation hallway.", new string[] { "c", "n" }, rooms);
            Room comms = new Room("communications", "You forgot why you came in here... You can go back into the hall.", new string[] { "h" }, rooms);
            Room storage = new Room("storage", "This room stores important cargo from the ship and connects multiple hallways. You can either go into the electrical hallway, the communications hallway or the admin hallway.", new string[] { "c", "a", "e" }, rooms);
            Room admin = new Room("admin", "The admin room is where the crewmates unsuccessfully swipe their cards and also shows the number of crewmates in each room. You can look at the table or go back into the hallway.", new string[] { "h", "t" }, rooms);
            Room elec = new Room("electrical", "Electrical... spooky. The only way you can go is back into the hallway.", new string[] { "h" }, rooms);
            Room lowerE = new Room("lower engine", "The lower of the two engines, it connects the electrical hallway to the reactor hallway.", new string[] { "e", "r" }, rooms);
            Room upperE = new Room("upper engine", "The upper engine, it connects the reactor hallway to the medbay hallway.", new string[] { "m", "r" }, rooms);
            Room sec = new Room("security", "Security, where you are able to spy on everybody.... wherever there is a camera. You can look at the cameras or go back into the hallway.", new string[] { "h", "s" }, rooms);
            Room reac = new Room("reactor", "Reactor - the powerhouse of the ce- ship, I meant ship. You can go back into the hallway from here.", new string[] { "h" }, rooms);
            Room med = new Room("medbay", "The medbay, where hurt crewmates are housed and nursed back to health. You can go into the hallway from here.", new string[] { "h" }, rooms);

            // Special
            Question adminT = new Question("You check the admin table...", new string[] { "l" });
            Question secC = new Question("You check the security cameras...", new string[] { "l" });

            cafe.setNewResults(new Question[] { weapons, adminHall, medHall });
            adminHall.setNewResults(new Question[] { cafe, admin, storage }); // add admin function
            medHall.setNewResults(new Question[] { cafe, med });
            reacHall.setNewResults(new Question[] { upperE, lowerE, reac, sec });
            navHall.setNewResults(new Question[] { weapons, oxygen, nav, shields });
            commsHall.setNewResults(new Question[] { comms, storage, shields });
            elecHall.setNewResults(new Question[] { elec, storage, lowerE });
            weapons.setNewResults(new Question[] { cafe, navHall });
            oxygen.setNewResults(new Question[] { navHall });
            nav.setNewResults(new Question[] { navHall });
            shields.setNewResults(new Question[] { commsHall, navHall });
            comms.setNewResults(new Question[] { commsHall });
            storage.setNewResults(new Question[] { commsHall, adminHall, elecHall });
            admin.setNewResults(new Question[] { adminHall, adminT });
            elec.setNewResults(new Question[] { elecHall });
            lowerE.setNewResults(new Question[] { elecHall, reacHall });
            upperE.setNewResults(new Question[] { medHall, reacHall });
            sec.setNewResults(new Question[] { reacHall, secC }); // add security function
            reac.setNewResults(new Question[] { reacHall });
            med.setNewResults(new Question[] { medHall });
            adminT.setNewResults(new Question[] { admin });
            secC.setNewResults(new Question[] { sec });

            Room[] npcSpawn = { cafe, elec, weapons, reac, sec, med, shields, admin, nav, oxygen, storage, upperE, lowerE };
            Room[] taskRooms = new Room[] { cafe, reacHall, adminHall, elec, navHall, storage, reac, admin, nav, reac, med, weapons, upperE, lowerE };

            Question[] specials = { adminT, secC };
            Room[] adminRooms = { cafe, admin, storage, comms, shields, nav, oxygen, weapons, med, upperE, lowerE, reac, sec, elec };
            Room[] halls = { adminHall, medHall, reacHall };

            // tutorial

            printLine("Welcome to the Among Us Text Game!\n", 10);
            tutorial();

            startGame(cafe, npcSpawn, taskRooms, rooms, specials, adminRooms, halls);
        }

        public static void startGame(Room startRoom, Room[] npcSpawn, Room[] taskRooms, Room[] rooms, Question[] specials, Room[] adminRooms, Room[] halls)
        {
            // points counter, either task for crew or kills for imp
            int points = 0;

            int playerRole = questionLoop("Would you like to be an impostor or a crewmate?", "", new string[] { "i", "c"}); // 0 is impostor 1 is crewmate

            bool done = false;

            while (!done)
            {
                print("How many crewmates would you like to be on the ship with you? : ", 10);

                string crews = Console.ReadLine();

                Console.Clear();

                try
                {
                    int crewNum = int.Parse(crews);
                    assignCrews(crewNum, npcSpawn);
                    done = true;
                }
                    
                catch (Exception)
                {
                    printLine("You did not input a number, please try again.", 10);
                }
            }

            done = false;

            while (!done)
            {
                print("How many impostors would you like to be on the ship with you? : ", 10);

                string impss = Console.ReadLine();

                Console.Clear();

                try
                {
                    int impNum = int.Parse(impss);
                    assignImps(impNum, npcSpawn);
                    done = true;
                }

                catch (Exception)
                {
                    printLine("You did not input a number, please try again.", 10);
                }
            }

            if (playerRole == 1)
            {
                done = false;

                while (!done)
                {
                    print("How many tasks would you like there to be? : ", 10);

                    string tasks = Console.ReadLine();

                    Console.Clear();

                        int taskNum = int.Parse(tasks);

                        int length = taskRooms.Length;

                        assignTasks(taskNum, taskRooms, taskRooms[length - 2], taskRooms[length - 1]);
                        done = true;
                    try
                    {
                    }

                    catch (Exception)
                    {
                        printLine("You did not input a number please try again.", 10);
                    }
                }
            }

            // This one line of code uses all of the objects to go through the game
            // it stops when the player gets killed or have no crewmates to kill or tasks to do, which stops the next room from getting ran, stopping all the codes that its in and comes back to this to go to the end code
            startRoom.answerToQuestion(rooms, specials, adminRooms, halls, playerRole, points);

            // ends the game if the player dies
            endGame(startRoom, npcSpawn, taskRooms, rooms, specials, adminRooms, halls, playerRole, points);
        }

        public static void endGame(Room startRoom, Room[] npcSpawn, Room[] taskRooms, Room[] rooms, Question[] specials, Room[] adminRooms, Room[] halls, int playerRole, int points)
        {
            if (playerRole == 1)
            {
                printLine($"As a crewmate, you completed {points} tasks!", 10);
            }

            else if (playerRole == 0)
            {
                printLine($"As an impostor, you killed {points} crewmates!", 10);
            }

            startGame(startRoom, npcSpawn, taskRooms, rooms, specials, adminRooms, halls);
        }

        public static void assignTasks(int taskNum, Room[] questionArray, Room upperE, Room lowerE) //Assigns tasks to rooms based on a number
        {
            Random random = new Random();

            for (int num = 0; num <= taskNum; num++)
            {
                int randNum = random.Next(0, questionArray.Length);


                if ((questionArray[randNum] == upperE | questionArray[randNum] == lowerE) & num <= taskNum - 2)
                {
                    upperE.tasks += 1;
                    lowerE.tasks += 1;
                    num++;


                }
                else
                {
                    questionArray[randNum].tasks += 1;
                }
            }
        }

        public static void assignCrews(int crewNum, Room[] questionArray) //Assigns tasks to rooms based on a number
        {
            Random random = new Random();

            for (int num = 0; num <= crewNum; num++)
            {
                int randNum = random.Next(0, questionArray.Length);

                questionArray[randNum].crews += 1;

            }
        }

        public static void assignImps(int impNum, Room[] questionArray) //Assigns tasks to rooms based on a number
        {
            Random random = new Random();

            for (int num = 0; num <= impNum; num++)
            {
                int randNum = random.Next(0, questionArray.Length);

                questionArray[randNum].imps += 1;

            }
        }

        // everytime someone does something, a turn is made
        public static void turn(Room[] rooms)
        {
            foreach (Room room in rooms)
            {
                room.moveNPCs();
                room.setNPCs();
            }
        }

        public static bool checkAllRooms(Room[] rooms, int playerRole)
        {
            bool status = false; // the status of the game, whether the player has completed everything or not

            int totalGoals = 0;
            
            if (playerRole == 0)
            {
                foreach (Room room in rooms)
                {
                    totalGoals += room.crews;
                }
            }

            else if (playerRole == 1)
            {
                foreach (Room room in rooms)
                {
                    totalGoals += room.tasks;
                }
            }

            if (totalGoals == 0)
            {
                status = true;
            }

            return status;
        }

        // prints all of the npcs in all rooms
        public static string adminTable(Room[] adminRooms)
        {
            string message = "";

            foreach (Room room in adminRooms)
            {
                message += $"\nThere are {room.crews + room.imps} people in {room.name}.";
            }

            return message;
        }

        // prints all of the npcs in all hallways (except elec)
        public static string secCams(Room[] halls)
        {
            string message = "";

            foreach (Room room in halls)
            {
                message += $"\nThere are {room.crews + room.imps} people in {room.name}.";
            }

            return message;
        }

        public static void tutorial()
        {
            int tutorialSpeed = 1;

            printLine("Tutorial:\n", tutorialSpeed);
            printLine("There are two roles you can be: a crewmate and an impostor.", tutorialSpeed);
            printLine("As a crewmate, your goal is to complete as many tasks as possible without getting killed.", tutorialSpeed);
            printLine("As an impostor, your goal is kill as many crewmates as possible without getting caught.", tutorialSpeed);
            printLine("You are also competing with your fellow impostors, as they will kill crewmates when there are more impostors than crewmates in the room with them.", tutorialSpeed);
            printLine("\nRules: ", tutorialSpeed);
            printLine("When trying to complete a task or waiting in a room as a crewmate, if there are more impostors than crewmates in the room, then the impostors will be able to attack. Otherwise you will be able to do your task.", tutorialSpeed);
            printLine("When trying to kill as an impostor, if there are more crewmates than impostors, the impostors will get caught. Otherwise all crewmates will get killed", tutorialSpeed);
            printLine("You can use 'wait' to stay in the current room or 'interact' to attempt to kill or complete a task.\n", tutorialSpeed);

        }

        // code by zoe - prints things like someone is typing
        public static void print(string text, int speed)
        {
            foreach (char t in text)
            {
                Console.Write(t);
                Thread.Sleep(speed);
            }
        }
        public static void printLine(string text, int speed)
        {
            print(text + '\n', speed);
        }
    }
}