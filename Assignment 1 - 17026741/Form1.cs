using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Assignment_1___17026741
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //17026741
        int maxEntry = 30; //number of entry's allowed
        int maxValue; //largest value in the list
        int minValue; //smallest value in the list
        int probeCount = 0; //number of probes during search
        int location = 0; //location of searched value
        bool detected; //indicates whether value has been found or not
        int searchValue; //value that is being searched for
        
        private void Form1_Load(object sender, EventArgs e)
        {
            maxEntry_txt.Text = Convert.ToString(maxEntry); //set max number of entries allowed in the list
            delete_Btn.AllowDrop = true; //allow for drops onto the delete button
        }
        
        //"Statistics" and update functions - 
        enum UPDATE {searchResults = 1, generalStats = 0, clearStats = -1 } //update types
        private void updateButtonStates()
        {
            if (Value_List_Box.Items.Count != maxEntry) { insert_Btn.Enabled = true; } //if the list box isn't full, then the the insert button must be enables to allow more entry
            else { insert_Btn.Enabled = false; } //otherwise, the list must be full, disable the the insert button to prevent more entry
            if (Value_List_Box.Items.Count >= 1)  //if there are more than 0 items in the list box...
            {
                delete_Btn.Enabled = true; //...enable the delete button
                clear_Btn.Enabled = true; //...enable the clear button
                searchBtn.Enabled = true; //...enable the search button
            }
            else
            {
                delete_Btn.Enabled = false; //...otherwise there is nothing to delete so disable the delete button...
                clear_Btn.Enabled = false; //...nothing to clear so disable the clear button...
                searchBtn.Enabled = false; //...nothing to search so disable the search button...
            }

            //if there's more than 1 value in the list box then enable the shuffle button...disable if there isn't
            if (Value_List_Box.Items.Count >= 2) { shuffle_btn.Enabled = true; }
            else { shuffle_btn.Enabled = false; }
        } 
        private void updateStats(int updateType)
        {
            //if update type is 1 (search results), display/update search results
            if (updateType == 1) 
            {
                searchValue_txt.Text = Convert.ToString(searchValue); //display which value is being searched
                detected_txt.Text = Convert.ToString(detected); //indicate whether value exists or not
                probes_txt.Text = Convert.ToString(probeCount); //display number of probes it took to find value within list
                if (location == -1) { location_Txt.Text = "N/A"; } //if location is -1 then this value doesn not exist...display N/A
                else { location_Txt.Text = Convert.ToString(location); }; //otherwise display location of value withing the list
            }

            //if update tye is 0 (general stats) or update type is -1 (clear stats)
            if (updateType == 0 || updateType == -1) 
            {
                entryCount_txt.Text = Convert.ToString(Value_List_Box.Items.Count); //display entry count
                if (Value_List_Box.Items.Count < 1) { entryCount_txt.Text = ""; } //leave "entry count" blank if list is empty

                if (Value_List_Box.Items.Count >= 1) //if the list is populated...
                {
                    firstEntry_txt.Text = Convert.ToString(Value_List_Box.Items[0]); //...display first entry...
                    lastEntry_txt.Text = Convert.ToString(Value_List_Box.Items[Value_List_Box.Items.Count - 1]); //...display last entry...
                }
                else { firstEntry_txt.Text = ""; lastEntry_txt.Text = ""; } //leave them blank if list is empty...

                getMaxMin(); //get smallest and larget value
                minValue_txt.Text = Convert.ToString(minValue); //diplay the smallest value
                maxValue_txt.Text = Convert.ToString(maxValue); //diplay the largest value
            }

            //if update type is -1 (clear stats), everything should be blank and/or 0
            if(updateType == -1)
            {
                minValue_txt.Text = "";
                maxValue_txt.Text = "";
                searchValue_txt.Text = "--";
                detected_txt.Text = "--";

                probeCount = 0;
                probes_txt.Text = "--";

                location = 0;
                location_Txt.Text = "--";
            }
        }
        private void getMaxMin()
        {
            int count;

            if (Value_List_Box.Items.Count >= 1)
            {
                //box max and min (by default) are the first indexed value in the list
                maxValue = Convert.ToInt32(Value_List_Box.Items[0]);
                minValue = Convert.ToInt32(Value_List_Box.Items[0]);

                //go through the list, if there is a number smaller than the current min value, set this as the new min value and vice versa
                for (count = 0; count < Value_List_Box.Items.Count; count++) 
                {
                    
                    if (Convert.ToInt32(Value_List_Box.Items[count]) < minValue) { minValue = Convert.ToInt32(Value_List_Box.Items[count]); }
                    if (Convert.ToInt32(Value_List_Box.Items[count]) > maxValue) { maxValue = Convert.ToInt32(Value_List_Box.Items[count]); }
                }
            }

        }

        //Sort(insertion sort) and insert algorithms ... note: try merge
        private void sortList()
        {
            int count;
            int pCount;
            int tmp;

            for (count = 1; count < Value_List_Box.Items.Count; count++) //go throug the list...(count represents the current index)
            {
                //keep record of the current value
                tmp = Convert.ToInt32(Value_List_Box.Items[count]);

                //swap the previous indexed value for the current value if the previous value is greater
                for (pCount = count - 1; pCount >= 0 && Convert.ToInt32(Value_List_Box.Items[pCount]) > tmp; pCount--) //pcount represents the index preceding the current value
                {
                    Value_List_Box.Items[pCount + 1] = Value_List_Box.Items[pCount]; 
                }
                Value_List_Box.Items[pCount + 1] = tmp;
            }
            updateStats(Convert.ToInt32(UPDATE.generalStats)); //update stats
        }
        private void insert(int value)
        {
            int count;
            int pCount;
            int tmp;

            Value_List_Box.Items.Add(-1); //add a blank space/object
            if (Value_List_Box.Items.Count < 1) { Value_List_Box.Items.Add(value); } //simply add the value if its the first

            else
            {
                if (sorted_Btn.Checked) //if the sorted button is checked...
                {
                    Value_List_Box.Items[Convert.ToInt32(Value_List_Box.Items.Count - 2) + 1] = value; //...place this value in the end of the list

                    //...move it through the list until the right position for it is found (within a sorted list)
                    for (count = Convert.ToInt32(Value_List_Box.Items.Count - 1); count < Value_List_Box.Items.Count; count++)
                    {
                        tmp = Convert.ToInt32(Value_List_Box.Items[count]);
                        for (pCount = count - 1; pCount >= 0 && Convert.ToInt32(Value_List_Box.Items[pCount]) > tmp; pCount--)
                        {
                            Value_List_Box.Items[pCount + 1] = Value_List_Box.Items[pCount];
                        }
                        Value_List_Box.Items[pCount + 1] = tmp;
                    }
                }
                else
                {
                    Value_List_Box.Items[Convert.ToInt32(Value_List_Box.Items.Count - 2) + 1] = value; //otherwsie place at end of the list
                }
            }
        }
        private void shuffleList()
        {
            if (!unsorted_Btn.Checked) { unsorted_Btn.Checked = true; } //check the unsorted button if it isn't already

            Random randnum = new Random();
            int count;
            int temp;

            for (count = 0; count <= Value_List_Box.Items.Count; count++)
            {
                int randIndex = randnum.Next(0, Value_List_Box.Items.Count); //pick a random index
                temp = Convert.ToInt32(Value_List_Box.Items[0]); //keep record of the value in the first index
                Value_List_Box.Items[0] = Value_List_Box.Items[randIndex]; //swap the values in the first index and the random index...
                Value_List_Box.Items[randIndex] = temp;
            }
            
        }

        //delete algorithm and features(i.e drag and drop)
        private void delete()
        {
            int count;
            if (Value_List_Box.SelectedIndex >= 0) //if the value has been selected
            {
                Value_List_Box.Items[Value_List_Box.SelectedIndex] = -1; //set selected value to -1...
                //...move to the end of the list and remove it
                for (count = Value_List_Box.SelectedIndex; count < Value_List_Box.Items.Count - 1; count++)
                {
                    Value_List_Box.Items[count] = Value_List_Box.Items[count + 1];
                }
                Value_List_Box.Items.RemoveAt(Value_List_Box.Items.Count - 1);
            }

            else
            {
                //otherwise just remove the last value in the list
                if (Value_List_Box.Items.Count >= 1)
                {
                    Value_List_Box.Items.RemoveAt(Value_List_Box.Items.Count - 1);
                }
            }

            //update general stats if the list is still populated
            if (Value_List_Box.Items.Count >= 1) { getMaxMin(); updateStats(Convert.ToInt32(UPDATE.generalStats)); }
            else { updateStats(Convert.ToInt32(UPDATE.clearStats)); } //otherwise, clear stats
            updateButtonStates(); //update button states
        }
        private void Value_List_Box_MouseDown(object sender, MouseEventArgs e)
        {
            if (Value_List_Box.Items.Count > 0)
            {
                Value_List_Box.DoDragDrop(Value_List_Box.SelectedItem, DragDropEffects.Move);
            }
        }
        private void delete_Btn_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        private void delete_Btn_DragDrop(object sender, DragEventArgs e)
        {
            delete();
        }

        //Search algorithms
        private bool linearSearch(int value)
        {
            bool exists = false;
            int count;

            //go through the list...
            for (count = 0; count < Value_List_Box.Items.Count; count++)
            {
                if (Convert.ToInt32(Value_List_Box.Items[count]) == value)//...compare each value in the list to the one requested and if it exists...
                {
                    exists = true; //set exists to true
                    probeCount = count; //set probe count to the the number of comparisons that it took
                    location = count + 1; //set location of the value
                    break;
                }
            }
            if (exists == false) { probeCount = count; location = -1; } //if item doesnt exist, set its location to -1;

            return exists;
        }
        private bool binarySearch(int value)
        {
            bool exists = false;
            int count = 0;

            int midIndex;
            int leftIndex = 0; //left index is the 0 (first index)
            int rightIndex = Value_List_Box.Items.Count - 1; //right index index is the last index

            while (leftIndex <= rightIndex)
            {
                count++;
                midIndex = leftIndex + ((rightIndex - leftIndex) / 2); //mid index is the mid point between right and lef index

                if (Convert.ToInt32(Value_List_Box.Items[midIndex]) == value) //compare the requested value to the mid index and if they are thesame
                {
                    exists = true; //set exists to true
                    probeCount = count; //set probe count to the number of comparisons made
                    location = midIndex + 1; //set location of value
                    break;
                }

                else 
                {
                    //if the mid index is greater than the value, narrow the search parameters toward the top of the list and vice versa if it less
                    if (Convert.ToInt32(Value_List_Box.Items[midIndex]) > value) { rightIndex = midIndex - 1; }
                    else { leftIndex = midIndex + 1; } 
                }
            }
            if (exists == false) { probeCount = count; location = -1; } //if item doesnt exist, set its location to -1;

            return exists;
        }
        private bool searchList(int value)
        {
            bool exists = false;

            if(linearS_Btn.Checked == true ) { exists = linearSearch(value); } //run linear search if the linear search button is checked

            if (binaryS_Btn.Checked == true ) { exists = binarySearch(value); } //run binary search if the binry search button is checked

            return exists;

        }
        
        //Button click functions
        private void Initialise_btn_Click(object sender, EventArgs e)
        {
            Random randNum = new Random();

            //if there are already values in the list then clear the list...
            if (Value_List_Box.Items.Count != 0) { Value_List_Box.Items.Clear(); }
            //...generate random numbers and only input them to the list if they aren't aleady in the list...
            for (int count = 0; count < maxEntry; count++)
            {
                int value = randNum.Next(0, 101);
                if (!linearSearch(value)) { insert(value); }
                else { count--; }
            }
            
            //update general stats and button states
            updateStats(Convert.ToInt32(UPDATE.generalStats));
            updateButtonStates();
        }
        private void insert_Btn_Click(object sender, EventArgs e)
        {
            //check to make sure attempted input is an integer...
            bool validInput = true;
            try { int.Parse(input_text.Text); }
            catch { validInput = false; }

            //...if not an integer display an error message...
            if (validInput == false) { MessageBox.Show("invalid input!"); }
            //...otherwise check if there's space for an input, if not display error message. *shouldnt come to this, but in case it does*...
            else if (Value_List_Box.Items.Count >= maxEntry) { MessageBox.Show("Error!"); }
            //...now check if the attempted input is already in the list box, if so, display an error message...
            else if (linearSearch(Convert.ToInt32(input_text.Text)) == true)
            {
                MessageBox.Show("This value is already in the list, insert a different number!");
            }

            //...push the integer on to the list
            else { insert( Convert.ToInt32(input_text.Text)); input_text.Text = ""; }

            //update general stats and button states
            updateStats(Convert.ToInt32(UPDATE.generalStats));
            updateButtonStates();
        }
        private void delete_Btn_Click(object sender, EventArgs e)
        {
            //if the List is empty then there's nothing to remove, soooo error message...
            if (Value_List_Box.Items.Count < 1) { MessageBox.Show("No Values To Remove!"); }

            delete(); //run delete algorithm
        }
        private void clear_Btn_Click(object sender, EventArgs e)
        {
            //shouldn't come to this but jus incase...if the list is empty then theres nothing to clear, soooo error message...
            if(Value_List_Box.Items.Count < 1) { MessageBox.Show("No Items To Clear!"); }
            //...clear the list if populated
            Value_List_Box.Items.Clear();

            //clear stats and update button states
            updateStats(Convert.ToInt32(UPDATE.clearStats));
            updateButtonStates();
        }
        private void Exit_Btn_Click(object sender, EventArgs e)
        {
            //close the form
            this.Close();
        }
        private void searchBtn_Click(object sender, EventArgs e)
        {
            //check to make sure attempted input is an integer...
            bool validInput = true;
            try { int.Parse(input_text.Text); }
            catch { validInput = false; }

            //...if not an integer display an error message...
            if (validInput == false) { MessageBox.Show("invalid input!"); }

            else
            {
                detected = searchList(Convert.ToInt32(input_text.Text)); //search the list for the value
                searchValue = Convert.ToInt32(input_text.Text); //set search value
                updateStats(Convert.ToInt32(UPDATE.searchResults)); //update search results
                input_text.Text = ""; //clear the text box
            }
        }
        private void shuffle_btn_Click(object sender, EventArgs e)
        {
            shuffleList(); //run shuffle algorithm
            updateStats(Convert.ToInt32(UPDATE.generalStats)); //update general stats
        }

        //Radio buttons functions
        private void sorted_Btn_CheckedChanged(object sender, EventArgs e)
        {
            if (Value_List_Box.Items.Count > 1) { sortList(); } //sort the list
            if (sorted_Btn.Checked==true) //if the sorted button is checked
            {
                linearS_Btn.Enabled = true; //enable the linear search button
                binaryS_Btn.Enabled = true; //enable the binary search button
            }
            
        }
        private void unsorted_Btn_CheckedChanged(object sender, EventArgs e)
        {
            if (unsorted_Btn.Checked == true) //if the unsorted button is checked
            {
                linearS_Btn.Enabled = true; //enable the linear search button
                linearS_Btn.Checked = true; //check the linear search button
                binaryS_Btn.Enabled = false; //disable the binary search button
            }
        }
    }
}
