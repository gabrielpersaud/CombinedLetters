using System.Collections.Generic;
using System.IO;
public interface ILetterService{
    void CombineTwoLetters(string inputFile1, string inputFile2, string resultFile);
}
public class LetterService : ILetterService{
    public  void CombineTwoLetters(string inputFile1, string inputFile2, string resultFile) { //concatenates both types of letters and saves as .txt file
        string combinedLetter = inputFile1 +"\n\n"+ inputFile2;;
        File.WriteAllTextAsync(@"Output/"+currentDate()+"/"+resultFile, combinedLetter);
    }
    public static string currentDate(){ // returns string of current date of example format "20220317" for March 17 2022
        System.DateTime dateTime = System.DateTime.UtcNow.Date;
        string result = dateTime.ToString("yyyyMMdd");
        return result;
    }
    public static void archiveLetters(string currentDate){ //creates a copy of both letter types in a dated archive folder
        string fileName;
        string destFile;
        System.IO.Directory.CreateDirectory(@"Archive/Admission/"+currentDate); // create admission archive subfolder
        System.IO.Directory.CreateDirectory(@"Archive/Scholarship/"+currentDate); // create scholarship archive subfolder
        //get daily admission letters if they exist
        if (System.IO.Directory.Exists(@"Input/Admission/"+currentDate))
        {
            string[] admissionLetters = System.IO.Directory.GetFiles(@"Input/Admission/"+currentDate);

            //copy each admission letter to archive
            foreach (string s in admissionLetters){
                fileName = System.IO.Path.GetFileName(s);
                destFile = System.IO.Path.Combine(@"Archive/Admission/"+currentDate, fileName);
                System.IO.File.Copy(s, destFile, true);
            }
        }
        //get daily scholarship letters if they exist
        if (System.IO.Directory.Exists(@"Input/Scholarship/"+currentDate))
        {
            string[] scholarshipLetters = System.IO.Directory.GetFiles(@"Input/Scholarship/"+currentDate);
            //copy each scholarship letter to archive
            foreach (string s in scholarshipLetters){
                fileName = System.IO.Path.GetFileName(s);
                destFile = System.IO.Path.Combine(@"Archive/Scholarship/"+currentDate, fileName);
                System.IO.File.Copy(s, destFile, true);
            }
        }
    }
    public static List<string> getTwoLetterStudents(string currentDate){ //returns a list of student IDs who have both types of letters
        List<string> studentsCombinedLetters = new List<string>();
        string studentID;

        //get daily scholarship letters
        if (System.IO.Directory.Exists(@"Input/Scholarship/"+currentDate)){            
            string[] scholarshipLetters = System.IO.Directory.GetFiles(@"Input/Scholarship/"+currentDate);
            //if scholarship ID matches an admission letter ID then add ID to list
            foreach (string s in scholarshipLetters){
                studentID = System.IO.Path.GetFileName(s).Substring(12,8);
                if (System.IO.File.Exists(@"Input/Admission/"+currentDate+"/admission-"+studentID+".txt")){
                    studentsCombinedLetters.Add(studentID);
                }
            }
        }
        return studentsCombinedLetters;
    }
    public static void processIDList(List<string> IDList, string currentDate){ //combines letters for each ID and writes report
        LetterService myLetterService = new LetterService();
        string inputFile1;
        string inputFile2;
        string resultFile;

        //generate report
        string report = currentDate.Substring(4,2)+"/"+currentDate.Substring(6,2)+"/"+currentDate.Substring(0,4);
        report += " Report\n-------------------------------\n\nNumber of combined letters: "+IDList.Count.ToString()+"\n";
    
        //combine letters for necessary students and add ID to report file
        foreach (string id in IDList){
            inputFile1 = System.IO.File.ReadAllText(@"Input/Admission/"+currentDate+"/admission-"+id+".txt");
            inputFile2 = System.IO.File.ReadAllText(@"Input/Scholarship/"+currentDate+"/scholarship-"+id+".txt");
            resultFile = "combined-"+id+".txt";
            System.IO.Directory.CreateDirectory(@"Output/"+currentDate);

            myLetterService.CombineTwoLetters(inputFile1, inputFile2, resultFile);
            report += "    "+id+"\n";
        }
        //save report as .txt file
        File.WriteAllTextAsync(@"Output/"+currentDate+"/"+currentDate+"-REPORT.txt", report);
    }
    public static void Main(string[] args){
        string date = currentDate();
        archiveLetters(date);
        processIDList(getTwoLetterStudents(date), date);
        //takes a list of IDs of students with both letter types and a formatted date
        //then calls combineTwoLetters for each ID and generates .txt report
    }
}