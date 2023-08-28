using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sussy_impostor_text_adventure
{
    internal class Program
    {
        // This class handels all of the questions in the game, it makes sure that the user won't input any wrong inputs and sends them to and from rooms
        // Will handle any interactions with impostors and crewmates
        public class Question
        {
            private string question;
            private string[] answers;
            private Question[] results;
            private string[] npcs;

            public Question(string question, string[] answers, string[] npcs) 
            {
                this.question = question;
                this.answers = answers;
            }

            public void setNewResults(Question[] results)
            {
                this.results = results;
            }

            public void questionLoop()
            {
                bool done = false;

                while (!done)
                {
                    Console.Write(this.question + " : ");

                    string input = Console.ReadLine();

                    for (int num = 0; num < this.answers.Length; num++)
                    {
                        if (this.answers[num] == input)
                        {
                            Console.WriteLine("Add code here");
                            done = true;
                            break;
                        }
                    }

                    if (!done)
                    {
                        Console.WriteLine("Your input was not able to be detected, please try again.");
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            // Text adventure where you are either a crewmate or an impostor
            // youll get a summary at the end to show how many people you killed or tasks you completed
            Question newquestion = new Question();

        }
        
    }
}
