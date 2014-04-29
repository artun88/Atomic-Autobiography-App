/*Artun Kircali - 4/29/14
* Applicant class:
* Contains information relating to the user entering responses into
*   the form's text boxes.
* Properties should include: user ID, save/load/submit file directory
*   location, array of strings to hold info of form questions written
*   on windows form labels, array of strings to hold user input received 
*   from rich text boxes, and a value of bool type that detects if the
*   user had made a change has been made to the form.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Security.Permissions;

namespace Atomic_Object_Job_Application
{
    public class Applicant
    {
        private string userID = null;
        private string pathDir = @"C:\Atomic Object Autobiography\";
        private string[] appQs = new string[23];
        private string[] answers = new string[24];
        private bool propChange = false;

        public string UserID
        {
            get
            {
                return userID;
            }
            set
            {
                userID = value;
            }
        }

        public string PathDir
        {
            get
            {
                return pathDir;
            }
        }

        public bool PropChange
        {
            get
            {
                return propChange;
            }
            set
            {
                propChange = value;
            }
        }

        public string[] Answers
        {
            get
            {
                return answers;
            }
            set
            {
                answers = value;
            }
        }

        public string[] AppQuestions
        {
            get
            {
                return appQs;
            }
            set
            {
                appQs = value;
            }
        }

        //Records user's responses to form questions
        public void addResponse(int i, string str)
        {
            answers[i] = str;
        }

        //Removes all instances of new lines so file
        //can be saved properly
        public void removeNewLines(ref string[] answers)
        {
            for (int i = 0; i < answers.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(answers[i]))
                    answers[i] = answers[i].Replace('\n', ' ');
            }
        }

        //Recursively edits answers for readability
        //by extending each line only 70 characters from left
        public string putInParagraphs(string str)
        {
            int count = 70;

            if (string.IsNullOrWhiteSpace(str) | count >= str.Length)
                return str;

            int idx = str.IndexOf(' ', count);

            if (idx < 0)
                return str;

            string first = str.Substring(0, idx);
            string second = str.Substring(idx, str.Length - idx);
            second = second.Remove(0, 1);

            first += Environment.NewLine + putInParagraphs(second);

            return first;
        }

        //Formats submitted document by adding answer
        //array to array of questions. For readability.
        private string[] mergeQsAs()
        {
            string[] filledApp = new string[appQs.Length];
            int i = 0;

            for (i = 0; i < 7; i++)
            {
                filledApp[i] = appQs[i] + answers[i];
            }

            if (answers[7] == "Yes")
            {
                filledApp[7] = appQs[7];
                filledApp[8] = "Yes, " + answers[8] + Environment.NewLine;
            }
            else
                filledApp[7] = appQs[7] + answers[7];


            for (i = 9; i < appQs.Length; i++)
            {
                filledApp[i] = appQs[i] +
                    Environment.NewLine + answers[i] + Environment.NewLine;
            }

            return filledApp;
        }

        private static FileAttributes putAttributes(FileAttributes attribute, 
                                                    FileAttributes attRemove)
        {
            return attribute & ~attRemove;
        }

        //Creates data files containing non-submitted form data
        //for user to return to and continue editing before submission
        public bool saveFormData()
        {
            propChange = false;
            string pathFile = pathDir + @"Saved Form Data\" + userID + ".txt";

            UTF8Encoding enc = new UTF8Encoding();

            //Allows file to be saved again without error
            if (File.Exists(pathFile))
                File.SetAttributes(pathFile,
                    File.GetAttributes(pathFile) & ~FileAttributes.ReadOnly);

            removeNewLines(ref answers);

            try
            {
                File.WriteAllLines(pathFile, answers, enc);
                File.SetAttributes(pathFile, FileAttributes.ReadOnly);
                return true;
            }
            catch 
            {
                MessageBox.Show("There was an error saving the file", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        //Retrieves contents of autobiography from saved data file
        public bool loadFormData()
        {
            string pathFile = pathDir + @"Saved Form Data\";

            if (!Directory.Exists(pathFile))
            {
                MessageBox.Show("Missing directory.", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            pathFile += userID + ".txt";
            UTF8Encoding enc = new UTF8Encoding();

            try
            {
                File.SetAttributes(pathFile, 
                    File.GetAttributes(pathFile) & ~FileAttributes.ReadOnly);
                answers = File.ReadAllLines(pathFile, enc);
            }
            catch
            {
                MessageBox.Show("File could not be opened. Incorrect e-mail entered.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        //Creates a formatted text file displaying applicant's
        //autobiography information received from this form
        public bool submitApplication()
        {
            //Formats answer file for ease of reading
            removeNewLines(ref answers);
            try
            {
                for (int i = 0; i < answers.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(answers[i]))
                    {
                        answers[i] = putInParagraphs(answers[i]);
                        answers[i].TrimEnd();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Sorry, there was an issue submitting your " +
                    "autobiography.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return false;
            }
            
            string[] filledApp = mergeQsAs();
            string[] appName = answers[0].Split(' ');

            //id's user submission using their e-mail
            string pathFile = pathDir + @"Submits\" + appName.First() +
                "_" + appName.Last() + "_" + userID + ".txt";

            //Checks if user already submitted autobiography
            if (File.Exists(pathFile))
            {
                MessageBox.Show("This user has already submitted an autobiography",
                    "Failed to Submit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                File.WriteAllLines(pathFile, filledApp);
                File.SetAttributes(pathFile, FileAttributes.ReadOnly);
            }
            catch
            {
                MessageBox.Show("There was an error submitting the file.", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
    }
}
    
