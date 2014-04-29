/* Atomic Autobiography App: by Artun Kircali, 4/29/14
 * ---------------------------------------------------
 * Form 1:
 * Calls saving, loading, and submission functions at
 *   specfic times (only when Save, Load, Submit are pressed).
 * Specifies how to navigate form's 11 panels using 'Next',
 *   'Previous', 'Exit', 'Cancel', and 'Load' buttons.
 * Receives user input from controls located on form.
 * Checks for valid user input before any save is performed.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Atomic_Object_Job_Application
{
    public partial class page1 : Form
    {
        Applicant ap = new Applicant();
        bool firstClick = true; //Track entry to form after pg1_bNext clicked
        
        public void initDirectories()
        {
            string pathSave = ap.PathDir + @"Saved Form Data";
            string pathSubmit = ap.PathDir + @"Submits";

            if (!Directory.Exists(pathSave))
                Directory.CreateDirectory(pathSave);
           
            File.SetAttributes(pathSave, FileAttributes.Hidden);

            if (!Directory.Exists(pathSubmit))
                Directory.CreateDirectory(pathSubmit);
        }

        public page1()
        {
            InitializeComponent();
            initDirectories();
        }

        //Prepares format of autobiography text document
        //  & prepares proper default responses
        public void initAppQsAs()
        {
            ap.AppQuestions = new string[23]{"Name: ", 
                pg2_Lbl_Twitter.Text + ": ", pg2_Lbl_LinkedIn.Text + ": ",
                pg2_TL_GitHub.Text + ": ", pg2_Lbl_Blog.Text + ": ",
                pg2_Lbl_Pres.Text + ": ", pg2_Lbl_Pubs.Text + ": ",
                Environment.NewLine + pg3_TL_AppYesNo.Text + " ", 
                pg3_TL_AppList.Text + ": ", //Doesn't print this one
                pg3_TL_OtherLinks.Text + ": ", pg3_TL_Citizen.Text + " ",
                "1) " + pg4_TL_Q10.Text, "2) " + pg4_TL_Q11.Text,
                "3) " + pg5_TL_Q12.Text, "4) " + pg5_TL_Q13.Text,
                "5) " + pg6_TL_Q14.Text, "6) " + pg6_TL_Q15.Text,
                "7) " + pg7_TL_Q16.Text + pg7_TL_HintQ16.Text,
                "8) " + pg8_TL_Q17.Text, "9) " + pg8_TL_Q18.Text, 
                "10) " + pg9_TL_Q19.Text, "11) " + pg9_TL_Q20.Text, 
                "12) " + pg10_TL_Q21.Text};

            //Enters radio button's default setting as user input
            ap.addResponse(7, pg3_NoBox.Text);
        }
        
        //Checks if user entered a valid e-mail address
        //  & prepares file name specific to applicant.
        //string val: Specifies if form is being saved or submitted
        //  when providing an error message.
        public bool checkValidInput(string val)
        {
            string email = pg2_EMailQ.Text;
            string first = pg2_FirstNameQ1.Text.Trim();
            string last = pg2_LastNameQ1.Text.Trim();

            if(string.IsNullOrWhiteSpace(first) |
               string.IsNullOrWhiteSpace(last))
            {
                MessageBox.Show("Could not " + val + " application. " +
                    "'First/Last Name' fields required.","Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);     
                return false;
            }

            if(string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Could not " + val + " application. " +
                    "'E-Mail Address' field required.", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //Checks 'First Name' field for bad input
            for(int i = 0; i < first.Length; i++)
            {
                char chFirst = first[i];
                
                if (!char.IsLetter(chFirst) & chFirst != '-' & chFirst != '\'')                    
                {
                    MessageBox.Show("Could not " + val + " application. " +
                        "'First Name' field contains invalid input.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            
            //Checks 'Last Name' field for bad input
            for(int i = 0; i < last.Length; i++)
            {
                char chLast = last[i];
                
                if (!char.IsLetter(chLast) & chLast != '-' & chLast != '\'')
                {
                    MessageBox.Show("Could not " + val + " application. " +
                        "'Last Name' field contains invalid input.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            //Checks 'E-Mail' for valid input
            string[] emailParts = email.Split(new char[] {'@'}, 2);

            if(string.IsNullOrWhiteSpace(emailParts.First()) |
                string.IsNullOrWhiteSpace(emailParts.Last()) |
                emailParts.Last().Length < 5)
            {
                MessageBox.Show("Could not " + val + " application. " + 
                            "Invalid E-mail address entered.", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            for(int i = 0; i < emailParts.Length; i++)
            {
                for(int j = 0; j < emailParts[i].Length; j++)
                {
                    char chEmail = emailParts[i][j];
                    
                    if (!char.IsLetterOrDigit(chEmail) & chEmail != '_' 
                        & chEmail != '+' & chEmail != '-' & chEmail != '.')
                    {
                        MessageBox.Show("Could not " + val + " application. " +
                            "Invalid E-mail address entered.", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            return true;
        }

        //Checks if form contains any user-entered data
        public bool formEmpty()
        {
            for (int i = 0; i < ap.Answers.Length; i++)
            {
                if(i == 7 & ap.Answers[i] == pg3_DropDown.Text)
                {
                    //Do nothing because text is entered by default
                }
                else if (!string.IsNullOrWhiteSpace(ap.Answers[i]))
                {
                    return false; //Answer has been detected
                }
            }

            //Form is empty
            return true;
        }
        
        //Warns user if form isn't empty
        public void checkInputOnClose()
        {
            if (!formEmpty() & ap.PropChange)
            {
                if (MessageBox.Show("Would you like to save your work " +
                        "before exiting?", "Closing",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if(checkValidInput("save"))
                    {
                        if(ap.saveFormData())
                            MessageBox.Show("File was saved successfully.", "Form Saved");
                    }
                }
            }
        }
        
        //Warns user before closing form when hitting 'x' button
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown)
                return;

            checkInputOnClose();

            if (MessageBox.Show("Exit the application?", "Closing",
                  MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        //Exit Button: Allows exit, but checks for recorded reponses first
        private void buttonExit_Click(object sender, EventArgs e)
        {
            checkInputOnClose();

            if(MessageBox.Show("Exit the application?", "Closing", 
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.ExitThread();
            }
        }

        //**********Begin detection of user's form navigation**********//
        private void pg1_bNext_Click(object sender, EventArgs e)
        {
            page2.Visible = true;
            atomicLogo.Parent = page2;

            //Initializes question array once & only once form is entered
            if (firstClick)
            {
                firstClick = false;
                initAppQsAs();
            }
        }
               
        private void pg2_bPrev_Click(object sender, EventArgs e)
        {
            atomicLogo.Parent = this;
            page2.Visible = false;
        }

        private void pg2_bNext_Click(object sender, EventArgs e)
        {
            page3.Visible = true;
            bSave.Parent = page3; 
            atomicLogo.Parent = page3;
        }
                
        private void pg3_bPrev_Click(object sender, EventArgs e)
        {
            bSave.Parent = page2; 
            atomicLogo.Parent = page2;
            page3.Visible = false;
        }

        private void pg3_bNext_Click(object sender, EventArgs e)
        {
            page4.Visible = true;
            bSave.Parent = page4; 
            atomicLogo.Parent = page4;
        }

        private void pg4_bPrev_Click(object sender, EventArgs e)
        {
            bSave.Parent = page3; 
            atomicLogo.Parent = page3;
            page4.Visible = false;
        }

        private void pg4_bNext_Click(object sender, EventArgs e)
        {
            page5.Visible = true;
            bSave.Parent = page5; 
            atomicLogo.Parent = page5;
        }

        private void pg5_bPrev_Click(object sender, EventArgs e)
        {
            bSave.Parent = page4;
            atomicLogo.Parent = page4;
            page5.Visible = false;
        }

        private void pg5_bNext_Click(object sender, EventArgs e)
        {
            page6.Visible = true;
            bSave.Parent = page6; 
            atomicLogo.Parent = page6;
        }

        private void pg6_bPrev_Click(object sender, EventArgs e)
        {
            bSave.Parent = page5;
            atomicLogo.Parent = page5;
            page6.Visible = false;
        }

        private void pg6_bNext_Click(object sender, EventArgs e)
        {
            page7.Visible = true;
            bSave.Parent = page7; 
            atomicLogo.Parent = page7;
        }
        
        private void pg7_bPrev_Click(object sender, EventArgs e)
        {
            bSave.Parent = page6; 
            atomicLogo.Parent = page6;
            page7.Visible = false;
        }

        private void pg7_bNext_Click(object sender, EventArgs e)
        {
            page8.Visible = true;
            bSave.Parent = page8; 
            atomicLogo.Parent = page8;
        }

        private void pg8_bPrev_Click(object sender, EventArgs e)
        {
            atomicLogo.Parent = page7;
            bSave.Parent = page7; 
            page8.Visible = false;
        }

        private void pg8_bNext_Click(object sender, EventArgs e)
        {
            page9.Visible = true;
            bSave.Parent = page9; 
            atomicLogo.Parent = page9;
        }

        private void pg9_bPrev_Click(object sender, EventArgs e)
        {
            bSave.Parent = page8; 
            atomicLogo.Parent = page8;
            page9.Visible = false;
        }

        private void pg9_bNext_Click(object sender, EventArgs e)
        {
            page10.Visible = true;
            bSave.Parent = page10; 
            atomicLogo.Parent = page10;
        }

        private void pg10_bPrev_Click(object sender, EventArgs e)
        {
            bSave.Parent = page9; 
            atomicLogo.Parent = page9;
            page10.Visible = false;
        }
        //**********End detection of user's form navigation**********//

        //Submit button: 'Submits' form to local C:\ directory
        //specified by value of applicant's pathDir
        private void pg10_bSubmit_Click(object sender, EventArgs e)
        {
            if (checkValidInput("submit"))
            {
                if(MessageBox.Show("Are you sure you want to submit your "
                    + "application?", "Submitting", 
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (ap.submitApplication())
                    {
                        MessageBox.Show("Your form has been submitted.",
                            "Congratulations!", MessageBoxButtons.OK);
                        
                        Application.ExitThread();
                    }
                }
            }
        }
        
        //Save button: Saves user's progress and application
        private void bSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Save your application?", "Saving",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                    if (checkValidInput("save"))
                    {
                        if(ap.saveFormData())
                            MessageBox.Show("File was saved successfully.", "Form Saved");
                    }
            }
        }
        
        //Removes '@' and all '.'s to prep for file name
        public void formatUserID(string email)
        {
            string[] emailParts = email.Split('@');
            string first = emailParts.First();
            string last = emailParts.Last().Replace(".", "");
            ap.UserID = first + last;
        }

        //Loads user's previously saved responses from local file
        private void pgLoad_Login_Click(object sender, EventArgs e)
        {
            formatUserID(pgLoad_EMailQ.Text.Trim());
            
            if (ap.loadFormData())
            {
                //Now the answers array should be initialized
                //This section reloads data into the form
                //  by loading answers into their respective text boxes
                string[] tempName = ap.Answers[0].Split(' ');
                pg2_FirstNameQ1.Text = tempName.First();
                pg2_LastNameQ1.Text = tempName.Last();
                
                pg2_EMailQ.Text = ap.Answers[23];
                pg2_TwitterQ2.Text = ap.Answers[1];
                pg2_LinkedInQ3.Text = ap.Answers[2];
                pg2_GitHubQ4.Text = ap.Answers[3];
                pg2_BlogQ5.Text = ap.Answers[4];
                pg2_PresQ6.Text = ap.Answers[5];
                pg2_PubsQ7.Text = ap.Answers[6];
                pg3_AppListQ8.Text = ap.Answers[8];
                pg3_OtherLinksQ9.Text = ap.Answers[9];
                pg3_DropDown.Text = ap.Answers[10];
                pg4_AnswerQ10.Text = ap.Answers[11];
                pg4_AnswerQ11.Text = ap.Answers[12];
                pg5_AnswerQ12.Text = ap.Answers[13];
                pg5_AnswerQ13.Text = ap.Answers[14];
                pg6_AnswerQ14.Text = ap.Answers[15];
                pg6_AnswerQ15.Text = ap.Answers[16];
                pg7_AnswerQ16.Text = ap.Answers[17];
                pg8_AnswerQ17.Text = ap.Answers[18];
                pg8_AnswerQ18.Text = ap.Answers[19];
                pg9_AnswerQ19.Text = ap.Answers[20];
                pg9_AnswerQ20.Text = ap.Answers[21];
                pg10_AnswerQ21.Text = ap.Answers[22];
                
                if (ap.Answers[7] == pg3_YesBox.Text)
                    pg3_YesBox.Checked = true;
                
                //Closes login section and places user on page 2
                pgLoad_EMailQ.Clear();
                page2.Visible = true;
                atomicLogo.Parent = page2;
                loadPage.Visible = false;

                //Ensures question array is intialized once & only once
                if (firstClick)
                {
                    firstClick = false;
                    initAppQsAs();
                }

                ap.saveFormData();
                MessageBox.Show("File loaded successfully.");
            }
        }

        //Cancel button: Goes back to page 1 if user cancels Load page
        private void pgLoad_bCancel_Click(object sender, EventArgs e)
        {
            pgLoad_EMailQ.Clear();
            atomicLogo.Parent = this;
            loadPage.Visible = false;
        }

        //Load button: Display login page
        private void pg1_bLoad_Click(object sender, EventArgs e)
        {
            loadPage.Parent = this;
            loadPage.Visible = true;
            loadPage.BringToFront();
            atomicLogo.Parent = loadPage;
        }
        
        //Formats arrangement of page3 according to user input
        private void pg3_YesBox_CheckedChanged_1(object sender, EventArgs e)
        {
            pg3_TL_AppList.Visible = true;
            pg3_AppListQ8.Visible = true;
            pg3_TL_OtherLinks.Location = pg3_LblMove1.Location;
            pg3_OtherLinksQ9.Location = pg3_TboxMove.Location;
            pg3_TL_Citizen.Refresh(); 
            pg3_TL_Citizen.Location = pg3_LblMove2.Location;
            pg3_DropDown.Location = pg3_DDListMove.Location;
            bSave.Refresh();
            pg2_bNext.Refresh();
            
            //Records and updates user input
            ap.addResponse(7, pg3_YesBox.Text);
            ap.PropChange = true;
        }

        //Formats arrangement of page 3 according to user input
        private void pg3_NoBox_CheckedChanged_1(object sender, EventArgs e)
        {
            pg3_TL_OtherLinks.Location = pg3_TL_AppList.Location;
            pg3_OtherLinksQ9.Location = pg3_AppListQ8.Location;
            pg3_TL_Citizen.Location = pg3_TLCitizenOriginal.Location;
            pg3_DropDown.Location = pg3_DropListOriginal.Location;
            pg3_TL_AppList.Visible = false;
            pg3_AppListQ8.Visible = false;
            pg3_TL_Citizen.Refresh();
            bSave.Refresh();
            pg2_bNext.Refresh();

            //Records and updates user input
            ap.addResponse(7, pg3_NoBox.Text);
            ap.PropChange = true;
        }

        //**************Begin recording user responses**************//
        private void pg2_FirstNameQ1_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(0, pg2_FirstNameQ1.Text.Trim() + " " +
                pg2_LastNameQ1.Text.Trim());
            ap.PropChange = true;
        }

        private void pg2_LastNameQ1_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(0, pg2_FirstNameQ1.Text.Trim() + " " + 
                pg2_LastNameQ1.Text.Trim());
            ap.PropChange = true;
        }

        private void pg2_EMailQ_TextChanged(object sender, EventArgs e)
        {
            formatUserID(pg2_EMailQ.Text.Trim());
            ap.addResponse(23, pg2_EMailQ.Text.Trim());
            ap.PropChange = true;
        }

        private void pg2_TwitterQ2_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(1, pg2_TwitterQ2.Text.Trim());
            ap.PropChange = true;
        }

        private void pg2_LinkedInQ3_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(2, pg2_LinkedInQ3.Text.Trim());
            ap.PropChange = true;
        }

        private void pg2_GitHubQ4_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(3, pg2_GitHubQ4.Text.Trim());
            ap.PropChange = true;
        }

        private void pg2_BlogQ5_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(4, pg2_BlogQ5.Text.Trim());
            ap.PropChange = true;
        }

        private void pg2_PresQ6_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(5, pg2_PresQ6.Text.Trim());
            ap.PropChange = true;
        }

        private void pg2_PubsQ7_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(6, pg2_PubsQ7.Text.Trim());
            ap.PropChange = true;
        }

        private void pg3_AppListQ8_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(8, pg3_AppListQ8.Text.Trim());
            ap.PropChange = true;
        }

        private void pg3_OtherLinksQ9_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(9, pg3_OtherLinksQ9.Text.Trim());
            ap.PropChange = true;
        }

        private void pg3_DropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            ap.addResponse(10, pg3_DropDown.SelectedItem.ToString());
            ap.PropChange = true;
        }
        
        private void pg4_AnswerQ10_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(11, pg4_AnswerQ10.Text.Trim());
            ap.PropChange = true;
        }

        private void pg4_AnswerQ11_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(12, pg4_AnswerQ11.Text.Trim());
            ap.PropChange = true;
        }

        private void pg5_AnswerQ12_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(13, pg5_AnswerQ12.Text.Trim());
            ap.PropChange = true;
        }

        private void pg5_AnswerQ13_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(14, pg5_AnswerQ13.Text.Trim());
            ap.PropChange = true;
        }

        private void pg6_AnswerQ14_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(15, pg6_AnswerQ14.Text.Trim());
            ap.PropChange = true;
        }

        private void pg6_AnswerQ15_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(16, pg6_AnswerQ15.Text.Trim());
            ap.PropChange = true;
        }

        private void pg7_AnswerQ16_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(17, pg7_AnswerQ16.Text.Trim());
            ap.PropChange = true;
        }

        private void pg8_AnswerQ17_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(18, pg8_AnswerQ17.Text.Trim());
            ap.PropChange = true;
        }

        private void pg8_AnswerQ18_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(19, pg8_AnswerQ18.Text.Trim());
            ap.PropChange = true;
        }

        private void pg9_AnswerQ19_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(20, pg9_AnswerQ19.Text.Trim());
            ap.PropChange = true;
        }

        private void pg9_AnswerQ20_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(21, pg9_AnswerQ20.Text.Trim());
            ap.PropChange = true;
        }

        private void pg10_AnswerQ21_TextChanged(object sender, EventArgs e)
        {
            ap.addResponse(22, pg10_AnswerQ21.Text.Trim());
            ap.PropChange = true;
        }
        //**************End recording user responses**************//

        //Ensures page consistency after maximizing or minimizing window
        private void page1_Resize(object sender, EventArgs e)
        {
            if (page2.Visible & !page3.Visible)
            {
                atomicLogo.Refresh();
                pg2_TL_Email.Refresh();
                pg2_TL_GitHub.Refresh();
            }
            else if (page3.Visible & !page4.Visible)
            {
                atomicLogo.Refresh();
                pg3_TL_AppYesNo.Refresh();
                pg3_TL_AppList.Refresh();
                pg3_TL_OtherLinks.Refresh();
                pg3_TL_Citizen.Refresh();
                pg3_AppListQ8.Refresh();
                pg3_OtherLinksQ9.Refresh();
                bSave.Refresh();
                pg2_bNext.Refresh();
            }
            else if (page4.Visible & !page5.Visible)
            {
                pg4_TL_Q10.Refresh();
                pg4_TL_Q11.Refresh();
            }
            else if (page5.Visible & !page6.Visible)
            {
                pg5_TL_Q12.Refresh();
                pg5_TL_Q13.Refresh();
            }
            else if (page6.Visible & !page7.Visible)
            {
                pg6_TL_Q14.Refresh();
                pg6_TL_Q15.Refresh();
            }
            else if (page7.Visible & !page8.Visible)
            {
                pg7_TL_Q16.Refresh();
                pg7_TL_HintQ16.Refresh();
            }
            else if (page8.Visible & !page9.Visible)
            {
                pg8_TL_Q17.Refresh();
                pg8_TL_Q18.Refresh();
            }
            else if (page9.Visible & !page10.Visible)
            {
                pg9_TL_Q19.Refresh();
                pg9_TL_Q20.Refresh();
            }
            else if (page10.Visible)
            {
                pg10_TL_Q21.Refresh();
            }
        }
    }

    //Creates a label that doesn't obscure underlying objects
    public class TransparentLabel : Label
    {
        public TransparentLabel()
        {
            this.SetStyle(ControlStyles.Opaque, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
            //draws underlying object as background
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20; //sets WS_EX_TRANSPARENT flag to true
                return cp;
            }
        }
    }
}
