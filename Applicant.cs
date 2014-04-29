/* Atomic Autobiography App: by Artun Kircali, 4/29/14
 * ---------------------------------------------------
 * Applicant Class:
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

        //Records & keeps track of user's responses.
        public void addResponse(int i, string str)
        {
            answers[i] = str;
        }

        //Removes all instances of new lines so that
        //  answers[] can be properly refilled during loadFormData().
        //If new lines aren't removed, responses can't be saved
        //  to correct indices of answers[].
        public void removeNewLines(ref string[] answers)
        {
            for (int i = 0; i < answers.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(answers[i]))
                    answers[i] = answers[i].Replace('\n', ' ');
            }
        }

        //Removes extraneous whitespace between words in user's
        //  responses to maintain neat consistent formatting in submission.
        public string trimSpaceBtwStrings(string str)
        {
            string[] words = str.Split(new string[] {" "},
                StringSplitOptions.None);
            string trimmedStr = null;
            
            //Cycle through entire resposne, adding each word to
            //  a new string with only one space between each word.
            for(int i = 0; i < words.Length; i++)
            {
                string result = words[i].Trim();

                if(!string.IsNullOrWhiteSpace(result))
                    trimmedStr += result + " ";
            }

            return trimmedStr.Trim();
        }

        //Recursively edits answers for readability in submission
        //  by extending each line only 70 characters from left.
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

        //Fills out autobiography document.
        //During submission, responses from answers[] are added to
        //  set questions in appQs[] so they can be written to file.
        //Basically returns appQs[] + answers[].
        private string[] mergeQsAs()
        {
            string[] filledApp = new string[appQs.Length];
            int i = 0;

            //Page 1 questions
            for (i = 0; i < 7; i++)
            {
                filledApp[i] = appQs[i] + answers[i];
            }

            //Page 2 question
            if (answers[7] == "Yes")
            {
                filledApp[7] = appQs[7];
                filledApp[8] = "Yes, " + answers[8] + Environment.NewLine;
            }
            else
                filledApp[7] = appQs[7] + answers[7];

            //Page 2 - 10 questions
            for (i = 9; i < appQs.Length; i++)
            {
                filledApp[i] = appQs[i] +
                    Environment.NewLine + answers[i] + Environment.NewLine;
            }

            return filledApp;
        }

        //Saves user's current responses to a read-only 
        //  file in hidden folder.
        //Returns false: If set directory is missing or incorrect.
        //Returns true: If file is opened and written successfully.
        public bool saveFormData()
        {
            propChange = false;
            string pathFile = pathDir + @"Saved Form Data\" + userID + ".txt";

            //Changes file's permissions so it can be read
            if (File.Exists(pathFile))
            {
                File.SetAttributes(pathFile,
                    File.GetAttributes(pathFile) & ~FileAttributes.ReadOnly);
            }

            //Saves to a UTF8 encoded text file
            removeNewLines(ref answers);
            UTF8Encoding enc = new UTF8Encoding();
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

        //Loads user's responses from a previous session.
        //Returns false: If a directory is missing, a bad username was
        //  given, or the file couldn't be opened (i.e. missing, corrupt).
        //Returns true: If file successfully opened & read.
        public bool loadFormData()
        {
            string pathFile = pathDir + @"Saved Form Data\";

            //Checks if intended target directory is set
            if (!Directory.Exists(pathFile))
            {
                MessageBox.Show("Missing directory.", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //Loads from a UTF8 encoded text file
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

        //Saves a 'filled out' version of the form to a local directory
        //  as a neatly organized, formatted text document.
        //Returns false: if response formatting process fails, a file of
        //  the same name exists (user submits with same email twice),
        //  or the file could not be opened.
        //Returns true: If file saves successfully to specified directory.
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
                        answers[i] = trimSpaceBtwStrings(answers[i]);
                        answers[i] = putInParagraphs(answers[i]);
                        answers[i].TrimEnd();
                    }
                }
            }
            //answers[] has gone out of bounds during formatting process
            catch
            {
                MessageBox.Show("Sorry, there was an issue submitting " +
                    "your autobiography.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return false;
            }

            string[] filledApp = mergeQsAs(); //Fills out form
            
            //Name's file using user's name and e-mail 
            string[] appName = answers[0].Split(new string[] {" "},
                StringSplitOptions.None);
            string pathFile = pathDir + @"Submits\" + appName.First() +
                "_" + appName.Last() + "_" + userID + ".txt";
                 
            //Checks if user already submitted using this e-mail address
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
    
