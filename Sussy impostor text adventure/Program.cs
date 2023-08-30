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
                    if (answers[num] == input)
                    {
                        done = true;
                        endNum = num;
                    }
                }

                if (input == "w")
                {
                    done = true;
                }

                if (!done)
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
            public bool task = false;

            public Question(string question, string[] answers) 
            {
                this.question = question;
                this.answers = answers;
            }

            public void setNewResults(Question[] results)
            {
                this.results = results;
            }

            public int askQuestion(string extras)
            {
                return questionLoop(this.question, extras, this.answers);
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

                int input = askQuestion(extras);

                var room = this;

                if (input != -1)
                {
                    room = this.results[input];
                }

                else if (input == -2)
                {
                    if (playerRole == 1)
                    {
                        this.task = false;
                        points += 1;

                        // add getting killed if impostor in room
                    }

                    else if (playerRole == 0)
                    {
                        // add impostor kill code
                    }
                }

                if (room.GetType() == typeof(Room))
                {
                    turn(rooms);

                }

                if (this.GetType() == typeof(Room))
                {
                    if (playerRole == 0)
                    {
                        if (room.crews == 1)
                        {
                            extras += "\nThere is a crewmate the room with you...";
                        }

                        else if (room.crews > 1)
                        {
                            extras += $"\nThere are {room.crews + room.imps} crewmates in the room with you...";
                        }

                        if (room.imps == 1)
                        {
                            extras += "\nThere is an impostor the room with you...";
                        }

                        else if (room.imps > 1)
                        {
                            extras += $"\nThere are {room.crews + room.imps} impostors in the room with you...";
                        }
                    }

                    else if (playerRole == 1)
                    {
                        if (room.crews + room.imps == 1)
                        {
                            extras += "There is someone else in the room with you...";
                        }

                        else if (room.crews + room.imps > 1)
                        {
                            extras += $"There are {room.crews + room.imps} people in the room with you...";
                        }
                    }
                }

                room.answerToQuestion(rooms, specials, adminRooms, halls, playerRole, points);
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

                // setting crews
                for (int c = 0; c < this.crews; c++)
                {

                    int num = random.Next(0, movableRooms.Length);

                    movableRooms[num].newCrews += 1;

                }

                for (int i = 0; i < this.imps; i++)
                {
                    int num = random.Next(0, movableRooms.Length);

                    movableRooms[num].newImps += 1;
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

            // points counter, either task for crew or kills for imp
            int points = 0;

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
            Question adminT = new Question("You check the admin table...", new string[] {"l"});
            Question secC = new Question("You check the security cameras...", new string[] {"l"});

            cafe.setNewResults(new Question[] {weapons, adminHall, medHall});
            adminHall.setNewResults(new Question[] {cafe, admin, storage}); // add admin function
            medHall.setNewResults(new Question[] {cafe, med});
            reacHall.setNewResults(new Question[] {upperE, lowerE, reac, sec});
            navHall.setNewResults(new Question[] {weapons, oxygen, nav, shields});
            commsHall.setNewResults(new Question[] {comms, storage, shields});
            elecHall.setNewResults(new Question[] {elec, storage, lowerE});
            weapons.setNewResults(new Question[] {cafe, navHall});
            oxygen.setNewResults(new Question[] {navHall});
            nav.setNewResults(new Question[] {navHall});
            shields.setNewResults(new Question[] {commsHall, navHall});
            comms.setNewResults(new Question[] {commsHall});
            storage.setNewResults(new Question[] {commsHall, adminHall, elecHall});
            admin.setNewResults(new Question[] {adminHall, adminT});
            elec.setNewResults(new Question[] {elecHall});
            lowerE.setNewResults(new Question[] {elecHall, reacHall});
            upperE.setNewResults(new Question[] {medHall, reacHall});
            sec.setNewResults(new Question[] {reacHall, secC}); // add security function
            reac.setNewResults(new Question[] {reacHall});
            med.setNewResults(new Question[] {medHall});
            adminT.setNewResults(new Question[] {admin});
            secC.setNewResults(new Question[] {sec});

            // Text adventure where you are either a crewmate or an impostor
            // youll get a summary at the end to show how many people you killed or tasks you completed
            printLine("Welcome to the Among Us Text Game!", 10);

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
                    assignCrews(crewNum, new Room[] { cafe, elec, weapons, reac, sec, med, shields, admin, nav, oxygen, storage, upperE, lowerE});
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
                    assignImps(impNum, new Room[] { cafe, elec, weapons, reac, sec, med, shields, admin, nav, oxygen, storage, upperE, lowerE });
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
                    print("How many tasks would you like there to be? (Max of 14) : ", 10);

                    string tasks = Console.ReadLine();

                    Console.Clear();

                    try
                    {
                        int taskNum = int.Parse(tasks);

                        if (taskNum > 14)
                        {
                            throw new Exception();
                        }

                        assignTasks(taskNum, new Room[] {cafe, reacHall, adminHall, elec, navHall, storage, reac, admin, nav, reac, med, weapons, upperE, lowerE}, upperE, lowerE);
                        done = true;
                    }

                    catch (Exception)
                    {
                        printLine("You did not input a number or a number under 15, please try again.", 10);
                    }
                }
            }

            Room[] adminRooms = { cafe, admin, storage, comms, shields, nav, oxygen, weapons, med, upperE, lowerE, reac, sec, elec };
            Room[] halls = { adminHall, medHall, reacHall };

            cafe.answerToQuestion(rooms, new Question[] { adminT, secC }, adminRooms, halls, playerRole, points);

        }

        public static void assignTasks(int taskNum, Room[] questionArray, Room upperE, Room lowerE) //Assigns tasks to rooms based on a number
        {
            Random random = new Random();

            Room[] newArray = questionArray;

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

                    newArray = new Room[] { };

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

                    newArray = new Room[] { };

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
            foreach (char t in text)
            {
                Console.Write(t);
                Thread.Sleep(speed);
            }

            Console.Write('\n');
        }
    }
}