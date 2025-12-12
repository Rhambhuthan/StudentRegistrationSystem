
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spectre.Console;

namespace CPE_Final_Project_Another_Way
{
    class Program
    {
        //database files paths
        static string UsersFile = @"C:\Users\HF\source\repos\CPE_Final_Project_Another_Way\users.txt";//id, name,pass
        static string StudentsFile = @"C:\Users\HF\source\repos\CPE_Final_Project_Another_Way\students.txt"; //ID, name, age, course, year, gender
        static string SubjectsFile = @"C:\Users\HF\source\repos\CPE_Final_Project_Another_Way\subjects.txt";  //code, name, units, allowed course/year
        static string SchedulesFile = @"C:\Users\HF\source\repos\CPE_Final_Project_Another_Way\schedules.txt"; //day,time,room
        static string PendingFile = @"C:\Users\HF\source\repos\CPE_Final_Project_Another_Way\pending.txt";  //pendingsubs
        static string RegisteredFile = @"C:\Users\HF\source\repos\CPE_Final_Project_Another_Way\registered.txt"; //registered scheds/subs


        static void Main(string[] args)
        {
            InitializeFiles();//make sures that text file exists
            MainMenu();
        }

        //create files with headers if doesnt exist
        static void InitializeFiles()
        {
            if (!File.Exists(UsersFile))
            {
                File.WriteAllLines(UsersFile, new[] {
                    "ID Number    | Full Name                      | Password       ",
                    "----------------------------------------------------------------"
                });
            }

            if (!File.Exists(StudentsFile))
            {
                File.WriteAllLines(StudentsFile, new[] {
                    "ID Number | Full Name                      | Age | Course | Year | Gender",
                    "--------------------------------------------------------------------------------"
                });
            }

            if (!File.Exists(SubjectsFile))
            {
                File.WriteAllLines(SubjectsFile, new[] {
                    "Code     | Subject Name                   | Units | Course Allowed          | Year Allowed",
                    "----------------------------------------------------------------------------------------------"
                });
            }

            if (!File.Exists(SchedulesFile))
            {
                File.WriteAllLines(SchedulesFile, new[] {
                    "Subject Code | Day              | Start Time | End Time | Room Number",
                    "--------------------------------------------------------------------------"
                });
            }

            if (!File.Exists(PendingFile))
            {
                File.WriteAllLines(PendingFile, new[] {
                    "Student ID | Subject Code | Schedule Day     | Start Time | End Time | Room Number",
                    "--------------------------------------------------------------------------------------"
                });
            }

            if (!File.Exists(RegisteredFile))
            {
                File.WriteAllLines(RegisteredFile, new[] {
                    "Student ID | Subject Code | Schedule Day     | Start Time | End Time | Room Number",
                    "--------------------------------------------------------------------------------------"
                });
            }
        }

        #region Main Menu

        static void MainMenu()
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.Write(
                    new Panel("[bold cyan]STUDENT REGISTRATION SYSTEM[/]")
                    .Header("[yellow]MAIN MENU[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("green"))
                );

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an option:")
                        .AddChoices(new[] { "Login", "Sign Up", "Exit" })
                );

                Console.Clear();
                switch (choice)
                {
                    case "Login": Login(); break;
                    case "Sign Up": SignUp(); break;
                    case "Exit": return;
                }
            }
        }


        //user account functions
        static void SignUp()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan underline]=== Sign Up ===[/]\n");

            string name = GetValidName();
            if (name == "0") return;

            string id = GetValidID();
            if (id == "0") return;

            var allUsers = File.Exists(UsersFile) ? File.ReadAllLines(UsersFile).Skip(2) : Array.Empty<string>();
            if (allUsers.Any(u => u.Split('|')[0].Trim() == id))
            {
                AnsiConsole.MarkupLine("[red]\nID already registered![/]");
                Console.ReadKey();
                return;
            }

            if (id.StartsWith("24"))
            {
                var enrolled = File.Exists(StudentsFile) ? File.ReadAllLines(StudentsFile).Skip(2) : Array.Empty<string>();
                if (!enrolled.Any(s => s.Split('|')[0].Trim() == id))
                {
                    AnsiConsole.MarkupLine("[red]Student not found. Contact administrators.[/]");
                    Console.ReadKey();
                    return;
                }
            }

            string pw1 = AnsiConsole.Prompt(
                new TextPrompt<string>("Password (or 0 to cancel):").PromptStyle("green").Secret());
            if (pw1 == "0") return;

            string pw2 = AnsiConsole.Prompt(
                new TextPrompt<string>("Retype Password (or 0 to cancel):").PromptStyle("green").Secret());
            if (pw2 == "0") return;

            if (string.IsNullOrWhiteSpace(pw1) || string.IsNullOrWhiteSpace(pw2))
            {
                AnsiConsole.MarkupLine("[red]\nPassword cannot be blank![/]");
                Console.ReadKey();
                return;
            }

            if (pw1 != pw2)
            {
                AnsiConsole.MarkupLine("[red]\nPasswords do not match![/]");
                Console.ReadKey();
                return;
            }

            var lines = File.Exists(UsersFile) ? File.ReadAllLines(UsersFile).Skip(2).ToList() : new List<string>();
            lines.Add($"{id,-12} | {name,-30} | {pw1,-15}");

            File.WriteAllLines(UsersFile, new[] {
                "ID Number    | Full Name                      | Password       ",
                "----------------------------------------------------------------"
            }.Concat(lines));

            AnsiConsole.MarkupLine("\n[green]Account created successfully![/]");
            Console.ReadKey();
        }

        static void Login()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan underline]=== Login ===[/]\n");

            string id = AnsiConsole.Ask<string>("ID Number (or 0 to cancel):");
            if (id == "0") return;

            string pw = AnsiConsole.Prompt(
                new TextPrompt<string>("Password (or 0 to cancel):").PromptStyle("green").Secret());
            if (pw == "0") return;

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(pw))
            {
                AnsiConsole.MarkupLine("[red]\nMissing Credentials![/]");
                Console.ReadKey();
                return;
            }

            var allUsers = File.Exists(UsersFile) ? File.ReadAllLines(UsersFile).Skip(2) : Array.Empty<string>();
            if (!allUsers.Any(u => u.Split('|')[0].Trim() == id && u.Split('|')[2].Trim() == pw))
            {
                AnsiConsole.MarkupLine("[red]\nWrong Credentials! Try Again.[/]");
                Console.ReadKey();
                return;
            }

            AnsiConsole.MarkupLine("\n[green]Login successful![/]");
            Console.ReadKey();

            if (id.StartsWith("09")) AdminMenu(id);
            else StudentMenu(id);
        }

        #endregion

        #region Admin Menu

        static void AdminMenu(string adminId)
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.Write(
                    new Panel("[bold cyan]ADMIN PANEL[/]")
                    .Header("[yellow]ADMIN MENU[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("green"))
                );

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an option:")
                        .AddChoices(new[] { "Add Student", "Student Information", "Add Subjects with Schedules", "Subject Information", "Logout" })
                );

                Console.Clear();
                switch (choice)
                {
                    case "Add Student": AddStudent(); break;
                    case "Student Information": StudentInformation(); break;
                    case "Add Subjects with Schedules": AddSubjectWithSchedules(); break;
                    case "Subject Information": SubjectInformation(); break;
                    case "Logout": return;
                }
            }
        }

        static void AddStudent()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan underline]=== Add Student ===[/]\n");

            string name = GetValidNameOrExit("Full Name");
            if (name == "0") return;

            int age = GetValidAge();
            if (age == 0) return;

            string course = GetValidCourse();
            if (course == "0") return;

            int year = GetValidYear();
            if (year == 0) return;

            string gender = GetValidGender();
            if (gender == "0") return;

            string id = GetValidStudentID();
            if (id == "0") return;

            var students = File.Exists(StudentsFile) ? File.ReadAllLines(StudentsFile).Skip(2).ToList() : new List<string>();

            if (students.Any(s => s.Split('|')[0].Trim() == id))
            {
                AnsiConsole.MarkupLine("[red]Student ID already exists![/]");
                Console.ReadKey();
                return;
            }

            students.Add($"{id,-9} | {name,-30} | {age,-3} | {course,-6} | {year,-4} | {gender,-6}");

            File.WriteAllLines(StudentsFile, new[] {
                "ID Number | Full Name                      | Age | Course | Year | Gender",
                "--------------------------------------------------------------------------------"
            }.Concat(students));

            AnsiConsole.MarkupLine("\n[green]Student added successfully![/]");
            Console.ReadKey();
        }

        static void StudentInformation()
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[bold cyan underline]=== Student Information ===[/]\n");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an option:")
                        .AddChoices(new[] { "View All Students", "Remove All Students", "Back" })
                );

                switch (choice)
                {
                    case "View All Students": ViewAllStudents(); break;
                    case "Remove All Students": RemoveAllStudents(); break;
                    case "Back": return;
                }
            }
        }

        static void ViewAllStudents()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan underline]=== All Students ===[/]\n");

            var students = File.Exists(StudentsFile) ? File.ReadAllLines(StudentsFile).Skip(2).ToList() : new List<string>();

            if (students.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No students found.[/]");
                Console.ReadKey();
                return;
            }

            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn("[green]ID Number[/]");
            table.AddColumn("[green]Full Name[/]");
            table.AddColumn("[green]Age[/]");
            table.AddColumn("[green]Course[/]");
            table.AddColumn("[green]Year[/]");
            table.AddColumn("[green]Gender[/]");

            foreach (var student in students)
            {
                var parts = student.Split('|');
                if (parts.Length >= 6)
                {
                    table.AddRow(parts[0].Trim(), parts[1].Trim(), parts[2].Trim(), parts[3].Trim(), parts[4].Trim(), parts[5].Trim());
                }
            }

            AnsiConsole.Write(table);

            var studentId = AnsiConsole.Ask<string>("\nEnter Student ID to manage (or 0 to go back):");
            if (studentId == "0") return;

            var selectedStudent = students.FirstOrDefault(s => s.Split('|')[0].Trim() == studentId);
            if (selectedStudent == null)
            {
                AnsiConsole.MarkupLine("[red]Student not found![/]");
                Console.ReadKey();
                return;
            }

            ManageStudent(studentId);
        }

        static void ManageStudent(string studentId)
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine($"[bold cyan underline]=== Manage Student: {studentId} ===[/]\n");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an option:")
                        .AddChoices(new[] { "Edit Student", "Remove Student", "View Student", "Back" })
                );

                switch (choice)
                {
                    case "Edit Student": EditStudent(studentId); break;
                    case "Remove Student":
                        if (RemoveStudent(studentId)) return;
                        break;
                    case "View Student": ViewStudent(studentId); break;
                    case "Back": return;
                }
            }
        }

        static void EditStudent(string studentId)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan underline]=== Edit Student ===[/]\n");

            var students = File.ReadAllLines(StudentsFile).Skip(2).ToList();
            var studentIndex = students.FindIndex(s => s.Split('|')[0].Trim() == studentId);

            if (studentIndex == -1)
            {
                AnsiConsole.MarkupLine("[red]Student not found![/]");
                Console.ReadKey();
                return;
            }

            var parts = students[studentIndex].Split('|');
            string oldName = parts[1].Trim();
            int oldAge = int.Parse(parts[2].Trim());
            string oldCourse = parts[3].Trim();
            int oldYear = int.Parse(parts[4].Trim());
            string oldGender = parts[5].Trim();

            AnsiConsole.MarkupLine($"[yellow]Press Enter to keep current value, or type '0' to cancel[/]\n");

            string name = GetEditInput($"Full Name [{oldName}]:", oldName);
            if (name == "0") return;

            Console.Write($"Age [{oldAge}]: ");
            string ageInput = Console.ReadLine();
            if (ageInput == "0") return;
            int age = string.IsNullOrWhiteSpace(ageInput) ? oldAge : GetValidAgeFromInput(ageInput);
            if (age == -1) return;

            string course = GetEditCourse($"Course [{oldCourse}]:", oldCourse);
            if (course == "0") return;

            Console.Write($"Year [{oldYear}]: ");
            string yearInput = Console.ReadLine();
            if (yearInput == "0") return;
            int year = string.IsNullOrWhiteSpace(yearInput) ? oldYear : GetValidYearFromInput(yearInput);
            if (year == -1) return;

            string gender = GetEditGender($"Gender [{oldGender}]:", oldGender);
            if (gender == "0") return;

            students[studentIndex] = $"{studentId,-9} | {name,-30} | {age,-3} | {course,-6} | {year,-4} | {gender,-6}";

            File.WriteAllLines(StudentsFile, new[] {
                "ID Number | Full Name                      | Age | Course | Year | Gender",
                "--------------------------------------------------------------------------------"
            }.Concat(students));

            AnsiConsole.MarkupLine("\n[green]Student updated successfully![/]");
            Console.ReadKey();
        }

        static bool RemoveStudent(string studentId)
        {
            var confirm = AnsiConsole.Confirm("Are you sure you want to remove this student?");
            if (!confirm) return false;

            var students = File.ReadAllLines(StudentsFile).Skip(2).Where(s => s.Split('|')[0].Trim() != studentId).ToList();

            File.WriteAllLines(StudentsFile, new[] {
                "ID Number | Full Name                      | Age | Course | Year | Gender",
                "--------------------------------------------------------------------------------"
            }.Concat(students));

            var pending = File.ReadAllLines(PendingFile).Skip(2).Where(p => p.Split('|')[0].Trim() != studentId).ToList();
            File.WriteAllLines(PendingFile, new[] {
                "Student ID | Subject Code | Schedule Day     | Start Time | End Time | Room Number",
                "--------------------------------------------------------------------------------------"
            }.Concat(pending));

            var registered = File.ReadAllLines(RegisteredFile).Skip(2).Where(r => r.Split('|')[0].Trim() != studentId).ToList();
            File.WriteAllLines(RegisteredFile, new[] {
                "Student ID | Subject Code | Schedule Day     | Start Time | End Time | Room Number",
                "--------------------------------------------------------------------------------------"
            }.Concat(registered));

            AnsiConsole.MarkupLine("\n[green]Student removed successfully![/]");
            Console.ReadKey();
            return true;
        }

        static void ViewStudent(string studentId)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan underline]=== View Student ===[/]\n");

            var students = File.ReadAllLines(StudentsFile).Skip(2).ToList();
            var student = students.FirstOrDefault(s => s.Split('|')[0].Trim() == studentId);

            if (student == null)
            {
                AnsiConsole.MarkupLine("[red]Student not found![/]");
                Console.ReadKey();
                return;
            }

            var parts = student.Split('|');
            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn("[green]Field[/]");
            table.AddColumn("[green]Value[/]");
            table.AddRow("ID Number", parts[0].Trim());
            table.AddRow("Full Name", parts[1].Trim());
            table.AddRow("Age", parts[2].Trim());
            table.AddRow("Course", parts[3].Trim());
            table.AddRow("Year", parts[4].Trim());
            table.AddRow("Gender", parts[5].Trim());

            AnsiConsole.Write(table);

            AnsiConsole.MarkupLine("\n[bold yellow]Registered Subjects:[/]");

            var registered = File.ReadAllLines(RegisteredFile).Skip(2).Where(r => r.Split('|')[0].Trim() == studentId).ToList();

            if (registered.Count == 0)
            {
                AnsiConsole.MarkupLine("[dim]No subjects registered yet.[/]");
            }
            else
            {
                var subTable = new Table();
                subTable.Border(TableBorder.Rounded);
                subTable.AddColumn("[green]Code[/]");
                subTable.AddColumn("[green]Subject Name[/]");
                subTable.AddColumn("[green]Units[/]");
                subTable.AddColumn("[green]Schedule Day[/]");
                subTable.AddColumn("[green]Schedule Time[/]");
                subTable.AddColumn("[green]Room Number[/]");

                foreach (var reg in registered)
                {
                    var regParts = reg.Split('|');
                    string code = regParts[1].Trim();
                    string day = regParts[2].Trim();
                    string start = regParts[3].Trim();
                    string end = regParts[4].Trim();
                    string room = regParts[5].Trim();

                    var subjects = File.ReadAllLines(SubjectsFile).Skip(2).ToList();
                    var subject = subjects.FirstOrDefault(s => s.Split('|')[0].Trim() == code);

                    if (subject != null)
                    {
                        var subParts = subject.Split('|');
                        string name = subParts[1].Trim();
                        string units = subParts[2].Trim();

                        subTable.AddRow(code, name, units, day, $"{start} - {end}", room);
                    }
                }

                AnsiConsole.Write(subTable);
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        static void RemoveAllStudents()
        {
            var confirm = AnsiConsole.Confirm("Are you sure you want to remove ALL students?");
            if (!confirm) return;

            File.WriteAllLines(StudentsFile, new[] {
                "ID Number | Full Name                      | Age | Course | Year | Gender",
                "--------------------------------------------------------------------------------"
            });

            File.WriteAllLines(PendingFile, new[] {
                "Student ID | Subject Code | Schedule Day     | Start Time | End Time | Room Number",
                "--------------------------------------------------------------------------------------"
            });

            File.WriteAllLines(RegisteredFile, new[] {
                "Student ID | Subject Code | Schedule Day     | Start Time | End Time | Room Number",
                "--------------------------------------------------------------------------------------"
            });

            AnsiConsole.MarkupLine("\n[green]All students removed successfully![/]");
            Console.ReadKey();
        }

        static void AddSubjectWithSchedules()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan underline]=== Add Subject with Schedules ===[/]\n");

            string code = GetValidSubjectCode();
            if (code == "0") return;

            var subjects = File.Exists(SubjectsFile) ? File.ReadAllLines(SubjectsFile).Skip(2).ToList() : new List<string>();

            if (subjects.Any(s => s.Split('|')[0].Trim() == code))
            {
                AnsiConsole.MarkupLine("[red]Subject code already exists![/]");
                Console.ReadKey();
                return;
            }

            string name = GetValidSubjectName();
            if (name == "0") return;

            int units = GetValidUnits();
            if (units == 0) return;

            string courseAllowed = GetValidCourseAllowed();
            if (courseAllowed == "0") return;

            string yearAllowed = GetValidYearAllowed();
            if (yearAllowed == "0") return;

            List<string> schedules = new List<string>();
            bool addingSchedules = true;

            while (addingSchedules)
            {
                Console.Clear();
                AnsiConsole.MarkupLine($"[bold cyan]Adding schedule for {code} - {name}[/]\n");

                string day = GetValidScheduleDay(code, schedules);
                if (day == "0") return;

                var times = GetValidScheduleTime();
                if (times.start == "0" || times.end == "0") return;

                string room = GetValidRoomNumber();
                if (room == "0") return;

                if (HasScheduleConflict(code, day, times.start, times.end, room, schedules))
                {
                    AnsiConsole.MarkupLine("[red]Schedule conflict! Same day, time, and room already exist.[/]");
                    Console.ReadKey();
                    continue;
                }

                schedules.Add($"{code,-12} | {day,-16} | {times.start,-10} | {times.end,-8} | {room,-11}");

                AnsiConsole.MarkupLine("[green]Schedule added![/]");

                addingSchedules = AnsiConsole.Confirm("Would you like to add another schedule?");
            }

            if (schedules.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Cannot save subject without schedules![/]");
                Console.ReadKey();
                return;
            }

            subjects.Add($"{code,-8} | {name,-30} | {units,-5} | {courseAllowed,-23} | {yearAllowed,-12}");

            File.WriteAllLines(SubjectsFile, new[] {
                "Code     | Subject Name                   | Units | Course Allowed          | Year Allowed",
                "----------------------------------------------------------------------------------------------"
            }.Concat(subjects));

            var existingSchedules = File.ReadAllLines(SchedulesFile).Skip(2).ToList();
            existingSchedules.AddRange(schedules);

            File.WriteAllLines(SchedulesFile, new[] {
                "Subject Code | Day              | Start Time | End Time | Room Number",
                "--------------------------------------------------------------------------"
            }.Concat(existingSchedules));

            AnsiConsole.MarkupLine("\n[green]Subject and schedules saved successfully![/]");
            Console.ReadKey();
        }

        static void SubjectInformation()
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[bold cyan underline]=== Subject Information ===[/]\n");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an option:")
                        .AddChoices(new[] { "View All Subjects", "Remove All Subjects", "Back" })
                );

                switch (choice)
                {
                    case "View All Subjects": ViewAllSubjects(); break;
                    case "Remove All Subjects": RemoveAllSubjects(); break;
                    case "Back": return;
                }
            }
        }

        static void ViewAllSubjects()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan underline]=== All Subjects ===[/]\n");

            var subjects = File.Exists(SubjectsFile) ? File.ReadAllLines(SubjectsFile).Skip(2).ToList() : new List<string>();

            if (subjects.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No subjects found.[/]");
                Console.ReadKey();
                return;
            }

            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn("[green]Code[/]");
            table.AddColumn("[green]Subject Name[/]");
            table.AddColumn("[green]Units[/]");
            table.AddColumn("[green]Course Allowed[/]");
            table.AddColumn("[green]Year Allowed[/]");

            foreach (var subject in subjects)
            {
                var parts = subject.Split('|');
                if (parts.Length >= 5)
                {
                    table.AddRow(parts[0].Trim(), parts[1].Trim(), parts[2].Trim(), parts[3].Trim(), parts[4].Trim());
                }
            }

            AnsiConsole.Write(table);

            var subjectCode = AnsiConsole.Ask<string>("\nEnter Subject Code to manage (or 0 to go back):");
            if (subjectCode == "0") return;

            var selectedSubject = subjects.FirstOrDefault(s => s.Split('|')[0].Trim() == subjectCode);
            if (selectedSubject == null)
            {
                AnsiConsole.MarkupLine("[red]Subject not found![/]");
                Console.ReadKey();
                return;
            }

            ManageSubject(subjectCode);
        }

        static void ManageSubject(string subjectCode)
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine($"[bold cyan underline]=== Manage Subject: {subjectCode} ===[/]\n");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an option:")
                        .AddChoices(new[] { "Edit Subject", "Remove Subject", "View Subject", "Back" })
                );

                switch (choice)
                {
                    case "Edit Subject": EditSubject(subjectCode); break;
                    case "Remove Subject":
                        if (RemoveSubject(subjectCode)) return;
                        break;
                    case "View Subject": ViewSubject(subjectCode); break;
                    case "Back": return;
                }
            }
        }

        static void EditSubject(string subjectCode)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan underline]=== Edit Subject ===[/]\n");

            var subjects = File.ReadAllLines(SubjectsFile).Skip(2).ToList();
            var subjectIndex = subjects.FindIndex(s => s.Split('|')[0].Trim() == subjectCode);

            if (subjectIndex == -1)
            {
                AnsiConsole.MarkupLine("[red]Subject not found![/]");
                Console.ReadKey();
                return;
            }

            var parts = subjects[subjectIndex].Split('|');
            string oldName = parts[1].Trim();
            int oldUnits = int.Parse(parts[2].Trim());
            string oldCourseAllowed = parts[3].Trim();
            string oldYearAllowed = parts[4].Trim();

            AnsiConsole.MarkupLine($"[yellow]Press Enter to keep current value, or type '0' to cancel[/]\n");

            string name = GetEditInput($"Subject Name [{oldName}]:", oldName);
            if (name == "0") return;

            Console.Write($"Units [{oldUnits}]: ");
            string unitsInput = Console.ReadLine();
            if (unitsInput == "0") return;
            int units = string.IsNullOrWhiteSpace(unitsInput) ? oldUnits : GetValidUnitsFromInput(unitsInput);
            if (units == -1) return;

            string courseAllowed = GetEditCourseAllowed($"Course Allowed [{oldCourseAllowed}]:", oldCourseAllowed);
            if (courseAllowed == "0") return;

            string yearAllowed = GetEditYearAllowed($"Year Allowed [{oldYearAllowed}]:", oldYearAllowed);
            if (yearAllowed == "0") return;

            subjects[subjectIndex] = $"{subjectCode,-8} | {name,-30} | {units,-5} | {courseAllowed,-23} | {yearAllowed,-12}";

            File.WriteAllLines(SubjectsFile, new[] {
                "Code     | Subject Name                   | Units | Course Allowed          | Year Allowed",
                "----------------------------------------------------------------------------------------------"
            }.Concat(subjects));

            AnsiConsole.MarkupLine("\n[green]Subject updated successfully![/]");
            Console.ReadKey();


        }

        static bool RemoveSubject(string subjectCode)
        {
            var confirm = AnsiConsole.Confirm("Are you sure you want to remove this subject?");
            if (!confirm) return false;

            var subjects = File.ReadAllLines(SubjectsFile).Skip(2).Where(s => s.Split('|')[0].Trim() != subjectCode).ToList();

            File.WriteAllLines(SubjectsFile, new[] {
                "Code     | Subject Name                   | Units | Course Allowed          | Year Allowed",
                "----------------------------------------------------------------------------------------------"
            }.Concat(subjects));

            var schedules = File.ReadAllLines(SchedulesFile).Skip(2).Where(s => s.Split('|')[0].Trim() != subjectCode).ToList();
            File.WriteAllLines(SchedulesFile, new[] {
            "Subject Code | Day              | Start Time | End Time | Room Number",
            "--------------------------------------------------------------------------"
        }.Concat(schedules));

            var pending = File.ReadAllLines(PendingFile).Skip(2).Where(p => p.Split('|')[1].Trim() != subjectCode).ToList();
            File.WriteAllLines(PendingFile, new[] {
            "Student ID | Subject Code | Schedule Day     | Start Time | End Time | Room Number",
            "--------------------------------------------------------------------------------------"
        }.Concat(pending));

            var registered = File.ReadAllLines(RegisteredFile).Skip(2).Where(r => r.Split('|')[1].Trim() != subjectCode).ToList();
            File.WriteAllLines(RegisteredFile, new[] {
            "Student ID | Subject Code | Schedule Day     | Start Time | End Time | Room Number",
            "--------------------------------------------------------------------------------------"
        }.Concat(registered));

            AnsiConsole.MarkupLine("\n[green]Subject removed successfully![/]");
            Console.ReadKey();
            return true;
        }

        static void ViewSubject(string subjectCode)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan underline]=== View Subject ===[/]\n");

            var subjects = File.ReadAllLines(SubjectsFile).Skip(2).ToList();
            var subject = subjects.FirstOrDefault(s => s.Split('|')[0].Trim() == subjectCode);

            if (subject == null)
            {
                AnsiConsole.MarkupLine("[red]Subject not found![/]");
                Console.ReadKey();
                return;
            }

            var parts = subject.Split('|');
            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn("[green]Field[/]");
            table.AddColumn("[green]Value[/]");
            table.AddRow("Code", parts[0].Trim());
            table.AddRow("Subject Name", parts[1].Trim());
            table.AddRow("Units", parts[2].Trim());
            table.AddRow("Course Allowed", parts[3].Trim());
            table.AddRow("Year Allowed", parts[4].Trim());

            AnsiConsole.Write(table);

            AnsiConsole.MarkupLine("\n[bold yellow]Schedules:[/]");

            var schedules = File.ReadAllLines(SchedulesFile).Skip(2).Where(s => s.Split('|')[0].Trim() == subjectCode).ToList();

            if (schedules.Count == 0)
            {
                AnsiConsole.MarkupLine("[dim]No schedules added yet.[/]");
            }
            else
            {
                var schTable = new Table();
                schTable.Border(TableBorder.Rounded);
                schTable.AddColumn("[green]Day[/]");
                schTable.AddColumn("[green]Start Time[/]");
                schTable.AddColumn("[green]End Time[/]");
                schTable.AddColumn("[green]Room Number[/]");

                foreach (var schedule in schedules)
                {
                    var schParts = schedule.Split('|');
                    schTable.AddRow(schParts[1].Trim(), schParts[2].Trim(), schParts[3].Trim(), schParts[4].Trim());
                }

                AnsiConsole.Write(schTable);
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        static void RemoveAllSubjects()
        {
            var confirm = AnsiConsole.Confirm("Are you sure you want to remove ALL subjects?");
            if (!confirm) return;

            File.WriteAllLines(SubjectsFile, new[] {
            "Code     | Subject Name                   | Units | Course Allowed          | Year Allowed",
            "----------------------------------------------------------------------------------------------"
        });

            File.WriteAllLines(SchedulesFile, new[] {
            "Subject Code | Day              | Start Time | End Time | Room Number",
            "--------------------------------------------------------------------------"
        });

            File.WriteAllLines(PendingFile, new[] {
            "Student ID | Subject Code | Schedule Day     | Start Time | End Time | Room Number",
            "--------------------------------------------------------------------------------------"
        });

            File.WriteAllLines(RegisteredFile, new[] {
            "Student ID | Subject Code | Schedule Day     | Start Time | End Time | Room Number",
            "--------------------------------------------------------------------------------------"
        });

            AnsiConsole.MarkupLine("\n[green]All subjects removed successfully![/]");
            Console.ReadKey();
        }

        #endregion

        #region Student Menu

        static void StudentMenu(string studentId)
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.Write(
                    new Panel("[bold cyan]STUDENT PANEL[/]")
                    .Header("[yellow]STUDENT MENU[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("green"))
                );

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an option:")
                        .AddChoices(new[] { "Add Subject", "Pending", "View Schedule", "Logout" })
                );

                Console.Clear();
                switch (choice)
                {
                    case "Add Subject": AddSubjectStudent(studentId); break;
                    case "Pending": PendingMenu(studentId); break;
                    case "View Schedule": ViewSchedule(studentId); break;
                    case "Logout": return;
                }
            }
        }

        static void AddSubjectStudent(string studentId)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan underline]=== Add Subject ===[/]\n");

            var students = File.ReadAllLines(StudentsFile).Skip(2).ToList();
            var student = students.FirstOrDefault(s => s.Split('|')[0].Trim() == studentId);

            if (student == null)
            {
                AnsiConsole.MarkupLine("[red]Student information not found![/]");
                Console.ReadKey();
                return;
            }

            var studentParts = student.Split('|');
            string studentCourse = studentParts[3].Trim();
            int studentYear = int.Parse(studentParts[4].Trim());

            var subjects = File.ReadAllLines(SubjectsFile).Skip(2).ToList();

            if (subjects.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No subjects available.[/]");
                Console.ReadKey();
                return;
            }

            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn("[green]Code[/]");
            table.AddColumn("[green]Subject Name[/]");
            table.AddColumn("[green]Units[/]");
            table.AddColumn("[green]Course Allowed[/]");
            table.AddColumn("[green]Year Allowed[/]");
            table.AddColumn("[green]Available[/]");

            foreach (var subject in subjects)
            {
                var parts = subject.Split('|');
                string code = parts[0].Trim();
                string name = parts[1].Trim();
                string units = parts[2].Trim();
                string courseAllowed = parts[3].Trim();
                string yearAllowed = parts[4].Trim();

                bool canAdd = CanStudentAddSubject(studentCourse, studentYear, courseAllowed, yearAllowed);
                string availability = canAdd ? "[green]Yes[/]" : "[red]No[/]";

                table.AddRow(code, name, units, courseAllowed, yearAllowed, availability);
            }

            AnsiConsole.Write(table);

            var subjectCode = AnsiConsole.Ask<string>("\nEnter Subject Code to add (or 0 to go back):");
            if (subjectCode == "0") return;

            var selectedSubject = subjects.FirstOrDefault(s => s.Split('|')[0].Trim() == subjectCode);
            if (selectedSubject == null)
            {
                AnsiConsole.MarkupLine("[red]Subject not found![/]");
                Console.ReadKey();
                return;
            }

            var subjectParts = selectedSubject.Split('|');
            string courseAllowedForSubject = subjectParts[3].Trim();
            string yearAllowedForSubject = subjectParts[4].Trim();

            if (!CanStudentAddSubject(studentCourse, studentYear, courseAllowedForSubject, yearAllowedForSubject))
            {
                AnsiConsole.MarkupLine("[red]You are not eligible to add this subject![/]");
                Console.ReadKey();
                return;
            }

            var pending = File.ReadAllLines(PendingFile).Skip(2).ToList();
            var registered = File.ReadAllLines(RegisteredFile).Skip(2).ToList();

            if (pending.Any(p => p.Split('|')[0].Trim() == studentId && p.Split('|')[1].Trim() == subjectCode))
            {
                AnsiConsole.MarkupLine("[red]Subject already in pending list![/]");
                Console.ReadKey();
                return;
            }

            if (registered.Any(r => r.Split('|')[0].Trim() == studentId && r.Split('|')[1].Trim() == subjectCode))
            {
                AnsiConsole.MarkupLine("[red]Subject already registered![/]");
                Console.ReadKey();
                return;
            }

            var confirm = AnsiConsole.Confirm($"Are you sure you want to add this subject?");
            if (!confirm) return;

            var schedules = File.ReadAllLines(SchedulesFile).Skip(2).Where(s => s.Split('|')[0].Trim() == subjectCode).ToList();

            if (schedules.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No schedules available for this subject![/]");
                Console.ReadKey();
                return;
            }

            bool scheduleSelected = false;
            while (!scheduleSelected)
            {
                Console.Clear();
                AnsiConsole.MarkupLine($"[bold cyan]Select Schedule for {subjectCode}[/]\n");

                var schTable = new Table();
                schTable.Border(TableBorder.Rounded);
                schTable.AddColumn("[green]No.[/]");
                schTable.AddColumn("[green]Day[/]");
                schTable.AddColumn("[green]Time[/]");
                schTable.AddColumn("[green]Room Number[/]");

                for (int i = 0; i < schedules.Count; i++)
                {
                    var schParts = schedules[i].Split('|');
                    string day = schParts[1].Trim();
                    string start = schParts[2].Trim();
                    string end = schParts[3].Trim();
                    string room = schParts[4].Trim();

                    schTable.AddRow((i + 1).ToString(), day, $"{start} - {end}", room);
                }

                AnsiConsole.Write(schTable);

                int scheduleChoice = AnsiConsole.Ask<int>("\nEnter schedule number:");

                if (scheduleChoice < 1 || scheduleChoice > schedules.Count)
                {
                    AnsiConsole.MarkupLine("[red]Invalid choice![/]");
                    Console.ReadKey();
                    continue;
                }

                var selectedSchedule = schedules[scheduleChoice - 1].Split('|');
                string selectedDay = selectedSchedule[1].Trim();
                string selectedStart = selectedSchedule[2].Trim();
                string selectedEnd = selectedSchedule[3].Trim();
                string selectedRoom = selectedSchedule[4].Trim();

                var proceedConfirm = AnsiConsole.Confirm("Proceed with this schedule?");
                if (!proceedConfirm) continue;

                if (HasStudentScheduleConflict(studentId, selectedDay, selectedStart, selectedEnd))
                {
                    AnsiConsole.MarkupLine("[red]Schedule conflict with existing subjects![/]");
                    Console.ReadKey();
                    continue;
                }

                var newPending = $"{studentId,-10} | {subjectCode,-12} | {selectedDay,-16} | {selectedStart,-10} | {selectedEnd,-8} | {selectedRoom,-11}";
                pending.Add(newPending);

                File.WriteAllLines(PendingFile, new[] {
                "Student ID | Subject Code | Schedule Day     | Start Time | End Time | Room Number",
                "--------------------------------------------------------------------------------------"
            }.Concat(pending));

                AnsiConsole.MarkupLine("\n[green]Subject added to pending list![/]");
                Console.ReadKey();
                scheduleSelected = true;
            }
        }

        static void PendingMenu(string studentId)
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[bold cyan underline]=== Pending Subjects ===[/]\n");

                var pending = File.ReadAllLines(PendingFile).Skip(2)
                                 .Where(p => p.Split('|')[0].Trim() == studentId)
                                 .ToList();

                if (pending.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No pending subjects.[/]");
                    Console.ReadKey();
                    return;
                }

                var table = new Table().Border(TableBorder.Rounded);
                table.AddColumn("[green]No.[/]");
                table.AddColumn("[green]Code[/]");
                table.AddColumn("[green]Subject Name[/]");
                table.AddColumn("[green]Units[/]");

                var subjects = File.ReadAllLines(SubjectsFile).Skip(2).ToList();

                for (int i = 0; i < pending.Count; i++)
                {
                    var pendingParts = pending[i].Split('|');
                    string code = pendingParts[1].Trim();

                    var subject = subjects.FirstOrDefault(s => s.Split('|')[0].Trim() == code);
                    if (subject != null)
                    {
                        var subParts = subject.Split('|');
                        table.AddRow(
                            (i + 1).ToString(),
                            code,
                            subParts[1].Trim(),
                            subParts[2].Trim()
                        );
                    }
                }

                AnsiConsole.Write(table);
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("\n[yellow]Choose an option:[/]")
                        .AddChoices(
                            "Register All Pending Subjects",
                            "Manage a Pending Subject",
                            "Back"
                        )
                        .WrapAround()
                );

                if (choice == "Register All Pending Subjects")
                {
                    RegisterPendingSubjects(studentId);
                    return;
                }
                else if (choice == "Manage a Pending Subject")
                {
                    // Choose which subject to manage
                    var subjectChoice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[cyan]Select a subject to manage:[/]")
                            .AddChoices(
                                pending.Select((p, idx) =>
                                {
                                    var c = p.Split('|')[1].Trim();
                                    var s = subjects.FirstOrDefault(x => x.Split('|')[0].Trim() == c);
                                    string name = s?.Split('|')[1].Trim() ?? "Unknown";
                                    return $"{c} - {name}";
                                })
                            )
                            .WrapAround()
                    );

                    string selectedCode = subjectChoice.Split('-')[0].Trim();
                    ManagePendingSubject(studentId, selectedCode);
                }
                else if (choice == "Back")
                {
                    return;
                }
            }
        }


        static void ManagePendingSubject(string studentId, string subjectCode)
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine($"[bold cyan underline]=== Manage Pending: {subjectCode} ===[/]\n");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an option:")
                        .AddChoices(new[] { "Remove", "View", "Back" })
                );

                switch (choice)
                {
                    case "Remove":
                        if (RemovePendingSubject(studentId, subjectCode)) return;
                        break;
                    case "View": ViewPendingSubject(studentId, subjectCode); break;
                    case "Back": return;
                }
            }
        }

        static bool RemovePendingSubject(string studentId, string subjectCode)
        {
            var confirm = AnsiConsole.Confirm("Are you sure you want to remove this subject?");
            if (!confirm) return false;

            var pending = File.ReadAllLines(PendingFile).Skip(2)
                .Where(p => !(p.Split('|')[0].Trim() == studentId && p.Split('|')[1].Trim() == subjectCode))
                .ToList();

            File.WriteAllLines(PendingFile, new[] {
            "Student ID | Subject Code | Schedule Day     | Start Time | End Time | Room Number",
            "--------------------------------------------------------------------------------------"
        }.Concat(pending));

            AnsiConsole.MarkupLine("\n[green]Subject removed from pending![/]");
            Console.ReadKey();
            return true;
        }

        static void ViewPendingSubject(string studentId, string subjectCode)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan underline]=== View Pending Subject ===[/]\n");

            var subjects = File.ReadAllLines(SubjectsFile).Skip(2).ToList();
            var subject = subjects.FirstOrDefault(s => s.Split('|')[0].Trim() == subjectCode);

            if (subject == null)
            {
                AnsiConsole.MarkupLine("[red]Subject not found![/]");
                Console.ReadKey();
                return;
            }

            var subParts = subject.Split('|');

            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn("[green]Field[/]");
            table.AddColumn("[green]Value[/]");
            table.AddRow("Code", subParts[0].Trim());
            table.AddRow("Subject Name", subParts[1].Trim());
            table.AddRow("Units", subParts[2].Trim());
            table.AddRow("Course Allowed", subParts[3].Trim());
            table.AddRow("Year Allowed", subParts[4].Trim());

            AnsiConsole.Write(table);

            AnsiConsole.MarkupLine("\n[bold yellow]Selected Schedule:[/]");

            var pending = File.ReadAllLines(PendingFile).Skip(2)
                .FirstOrDefault(p => p.Split('|')[0].Trim() == studentId && p.Split('|')[1].Trim() == subjectCode);

            if (pending != null)
            {
                var pendingParts = pending.Split('|');
                var schTable = new Table();
                schTable.Border(TableBorder.Rounded);
                schTable.AddColumn("[green]Day[/]");
                schTable.AddColumn("[green]Time[/]");
                schTable.AddColumn("[green]Room Number[/]");

                schTable.AddRow(
                    pendingParts[2].Trim(),
                    $"{pendingParts[3].Trim()} - {pendingParts[4].Trim()}",
                    pendingParts[5].Trim()
                );

                AnsiConsole.Write(schTable);
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        static void RegisterPendingSubjects(string studentId)
        {
            var pending = File.ReadAllLines(PendingFile).Skip(2).Where(p => p.Split('|')[0].Trim() == studentId).ToList();

            if (pending.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No pending subjects to register.[/]");
                Console.ReadKey();
                return;
            }

            int totalUnits = 0;
            var subjects = File.ReadAllLines(SubjectsFile).Skip(2).ToList();

            foreach (var pend in pending)
            {
                string code = pend.Split('|')[1].Trim();
                var subject = subjects.FirstOrDefault(s => s.Split('|')[0].Trim() == code);
                if (subject != null)
                {
                    int units = int.Parse(subject.Split('|')[2].Trim());
                    totalUnits += units;
                }
            }

            if (totalUnits < 9)
            {
                AnsiConsole.MarkupLine($"[red]Total units ({totalUnits}) is below minimum (9 units)![/]");
                Console.ReadKey();
                return;
            }

            if (totalUnits > 24)
            {
                AnsiConsole.MarkupLine($"[red]Total units ({totalUnits}) exceeds maximum (24 units)![/]");
                Console.ReadKey();
                return;
            }

            var confirm = AnsiConsole.Confirm($"Register all pending subjects? (Total: {totalUnits} units)");
            if (!confirm) return;

            var registered = File.ReadAllLines(RegisteredFile).Skip(2).ToList();
            registered.AddRange(pending);

            File.WriteAllLines(RegisteredFile, new[] {
            "Student ID | Subject Code | Schedule Day     | Start Time | End Time | Room Number",
            "--------------------------------------------------------------------------------------"
        }.Concat(registered));

            var remainingPending = File.ReadAllLines(PendingFile).Skip(2).Where(p => p.Split('|')[0].Trim() != studentId).ToList();

            File.WriteAllLines(PendingFile, new[] {
            "Student ID | Subject Code | Schedule Day     | Start Time | End Time | Room Number",
            "--------------------------------------------------------------------------------------"
        }.Concat(remainingPending));

            AnsiConsole.MarkupLine("\n[green]All subjects registered successfully![/]");
            Console.ReadKey();
        }

        static void ViewSchedule(string studentId)
        {
            Console.Clear();

            var registered = File.ReadAllLines(RegisteredFile).Skip(2).Where(r => r.Split('|')[0].Trim() == studentId).ToList();

            var entries = new List<(string Day, TimeSpan Start, TimeSpan End, string Subject, string Room)>();

            foreach (var reg in registered)
            {
                var parts = reg.Split('|');
                if (parts.Length < 6) continue;

                string day = parts[2].Trim();
                if (!TimeSpan.TryParse(parts[3].Trim(), out TimeSpan start)) continue;
                if (!TimeSpan.TryParse(parts[4].Trim(), out TimeSpan end)) continue;
                string subject = parts[1].Trim();
                string room = parts[5].Trim();

                entries.Add((day, start, end, subject, room));
            }

            string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

            var table = new Table()
                .Border(TableBorder.Rounded)
                .Title("[bold yellow]SCHEDULE[/]");

            foreach (var d in days)
                table.AddColumn($"[bold yellow]{d}[/]");

            if (entries.Count > 0)
            {
                List<string> row = new();
                foreach (var d in days)
                {
                    var subjectsForDay = entries
                        .Where(e => e.Day.Contains(d, StringComparison.OrdinalIgnoreCase))
                        .Select(e => $"[green]{e.Subject}[/]\n[blue]{e.Room}[/]\n[dim]{e.Start:hh\\:mm}-{e.End:hh\\:mm}[/]")
                        .ToArray();

                    row.Add(subjectsForDay.Length > 0 ? string.Join("\n\n", subjectsForDay) : "");
                }

                table.AddRow(row.ToArray());
            }
            else
            {
                table.AddRow("", "", "", "", "", "");
            }

            AnsiConsole.Write(table);

            if (entries.Count == 0)
            {
                AnsiConsole.WriteLine();
                var messagePanel = new Panel("[bold red]No Schedules Registered[/]")
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = new Style(Color.Red),
                    Padding = new Padding(2, 0)
                };
                AnsiConsole.Write(messagePanel);
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        #endregion

        #region Validation Helpers
        //ensures all input from admin and students are correct
        static string GetValidName()
        {
            Console.Write("Full Name (or 0 to cancel): ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            while (true)
            {
                string input = Console.ReadLine();

                if (input == "0") return "0";

                if (!string.IsNullOrWhiteSpace(input) && input.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                    return input;

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                AnsiConsole.MarkupLine("[red]Invalid Name![/]");
                Console.ReadKey();

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(left, top);
                Console.Write(new string(' ', Console.WindowWidth - left));
                Console.SetCursorPosition(left, top);
            }
        }

        static string GetValidID()
        {
            Console.Write("ID Number (or 0 to cancel): ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            while (true)
            {
                string input = Console.ReadLine();

                if (input == "0") return "0";

                if ((input.StartsWith("09") && input.Length == 10 && input.All(char.IsDigit)) ||
                    (input.StartsWith("24") && input.Length == 9 && input.All(char.IsDigit)))
                    return input;

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                AnsiConsole.MarkupLine("[red]Invalid ID number![/]");
                Console.ReadKey();

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(left, top);
                Console.Write(new string(' ', Console.WindowWidth - left));
                Console.SetCursorPosition(left, top);
            }
        }

        static string GetValidNameOrExit(string prompt)
        {
            Console.Write(prompt + " (or 0 to cancel): ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            while (true)
            {
                string input = Console.ReadLine();
                if (input == "0") return "0";

                if (!string.IsNullOrWhiteSpace(input) && input.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                    return input;

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 2);
                Console.Write(new string(' ', Console.WindowWidth));

                Console.SetCursorPosition(0, top + 1);
                AnsiConsole.MarkupLine("[red]Invalid Name![/]");
                Console.ReadKey();

                Console.SetCursorPosition(0, top);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 2);
                Console.Write(new string(' ', Console.WindowWidth));

                Console.SetCursorPosition(0, top);
                Console.Write(prompt + " (or 0 to cancel): ");
                left = Console.CursorLeft;
            }
        }

        static int GetValidAge()
        {
            Console.Write("Age (or 0 to cancel): ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            while (true)
            {
                string input = Console.ReadLine();

                if (input == "0") return 0;

                if (int.TryParse(input, out int age) && age > 0 && age < 150)
                    return age;

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                AnsiConsole.MarkupLine("[red]Invalid Age![/]");
                Console.ReadKey();

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(left, top);
                Console.Write(new string(' ', Console.WindowWidth - left));
                Console.SetCursorPosition(left, top);
            }
        }

        static int GetValidAgeFromInput(string input)
        {
            if (input == "0") return -1;

            if (int.TryParse(input, out int age) && age > 0 && age < 150)
                return age;

            AnsiConsole.MarkupLine("[red]Invalid Age![/]");
            Console.ReadKey();
            return -1;
        }

        static string GetValidCourse()
        {
            string[] validCourses = { "BSN", "BSCE", "BSARCH", "BSA", "BSHM" };

            var course = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select Course (or press Escape to cancel):")
                    .AddChoices(validCourses)
                    .AddChoices(new[] { "[red]Cancel[/]" })
            );

            if (course == "[red]Cancel[/]") return "0";

            return course;
        }

        static string GetEditCourse(string prompt, string oldValue)
        {
            string[] validCourses = { "BSN", "BSCE", "BSARCH", "BSA", "BSHM" };

            Console.Write(prompt + " ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            while (true)
            {
                string input = Console.ReadLine()?.ToUpper();

                if (input == "0") return "0";
                if (string.IsNullOrWhiteSpace(input)) return oldValue;

                if (validCourses.Contains(input))
                    return input;

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 2);
                Console.Write(new string(' ', Console.WindowWidth));

                Console.SetCursorPosition(0, top + 1);
                AnsiConsole.MarkupLine("[red]Invalid Course![/]");
                Console.ReadKey();

                Console.SetCursorPosition(0, top);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 2);
                Console.Write(new string(' ', Console.WindowWidth));

                Console.SetCursorPosition(0, top);
                Console.Write(prompt + " ");
                left = Console.CursorLeft;
            }
        }

        static int GetValidYear()
        {
            var yearChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select Year:")
                    .AddChoices(new[] { "1", "2", "3", "4", "[red]Cancel[/]" })
            );

            if (yearChoice == "[red]Cancel[/]") return 0;

            return int.Parse(yearChoice);
        }

        static int GetValidYearFromInput(string input)
        {
            if (input == "0") return -1;

            if (int.TryParse(input, out int year) && year >= 1 && year <= 4)
                return year;

            AnsiConsole.MarkupLine("[red]Invalid Year![/]");
            Console.ReadKey();
            return -1;
        }

        static string GetValidGender()
        {
            var gender = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select Gender:")
                    .AddChoices(new[] { "MALE", "FEMALE", "[red]Cancel[/]" })
            );

            if (gender == "[red]Cancel[/]") return "0";

            return gender;
        }

        static string GetEditGender(string prompt, string oldValue)
        {
            Console.Write(prompt + " ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            while (true)
            {
                string input = Console.ReadLine()?.ToUpper();

                if (input == "0") return "0";
                if (string.IsNullOrWhiteSpace(input)) return oldValue;

                if (input == "F") return "FEMALE";
                if (input == "M") return "MALE";

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 2);
                Console.Write(new string(' ', Console.WindowWidth));

                Console.SetCursorPosition(0, top + 1);
                AnsiConsole.MarkupLine("[red]Invalid Gender! Enter M or F[/]");
                Console.ReadKey();

                Console.SetCursorPosition(0, top);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 2);
                Console.Write(new string(' ', Console.WindowWidth));

                Console.SetCursorPosition(0, top);
                Console.Write(prompt + " ");
                left = Console.CursorLeft;
            }
        }

        static string GetValidStudentID()
        {
            Console.Write("ID Number [24XXXXXXX] (or 0 to cancel): ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            while (true)
            {
                string input = Console.ReadLine();

                if (input == "0") return "0";

                if (!string.IsNullOrWhiteSpace(input) && input.StartsWith("24") && input.Length == 9 && input.All(char.IsDigit))
                    return input;

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                AnsiConsole.MarkupLine("[red]Wrong ID number! Must start with 24 and be 9 digits.[/]");
                Console.ReadKey();

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(left, top);
                Console.Write(new string(' ', Console.WindowWidth - left));
                Console.SetCursorPosition(left, top);
            }
        }

        static string GetEditInput(string prompt, string oldValue)
        {
            Console.Write(prompt + " ");
            string input = Console.ReadLine();

            if (input == "0") return "0";
            if (string.IsNullOrWhiteSpace(input)) return oldValue;

            return input;
        }

        static string GetValidSubjectCode()
        {
            Console.Write("Subject Code (or 0 to cancel): ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            while (true)
            {
                string input = Console.ReadLine()?.ToUpper();

                if (input == "0") return "0";

                if (!string.IsNullOrWhiteSpace(input) && input.Length <= 8)
                    return input;

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                AnsiConsole.MarkupLine("[red]Invalid Subject Code![/]");
                Console.ReadKey();

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(left, top);
                Console.Write(new string(' ', Console.WindowWidth - left));
                Console.SetCursorPosition(left, top);
            }
        }

        static string GetValidSubjectName()
        {
            Console.Write("Subject Name (or 0 to cancel): ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            while (true)
            {
                string input = Console.ReadLine();

                if (input == "0") return "0";

                if (!string.IsNullOrWhiteSpace(input))
                    return input;

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                AnsiConsole.MarkupLine("[red]Invalid Subject Name![/]");
                Console.ReadKey();

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(left, top);
                Console.Write(new string(' ', Console.WindowWidth - left));
                Console.SetCursorPosition(left, top);
            }
        }

        static int GetValidUnits()
        {
            Console.Write("Units (or 0 to cancel): ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            while (true)
            {
                string input = Console.ReadLine();

                if (input == "0") return 0;

                if (int.TryParse(input, out int units) && units > 0 && units <= 10)
                    return units;

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                AnsiConsole.MarkupLine("[red]Invalid Units! Must be 1-10.[/]");
                Console.ReadKey();

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(left, top);
                Console.Write(new string(' ', Console.WindowWidth - left));
                Console.SetCursorPosition(left, top);
            }
        }

        static int GetValidUnitsFromInput(string input)
        {
            if (input == "0") return -1;

            if (int.TryParse(input, out int units) && units > 0 && units <= 10)
                return units;

            AnsiConsole.MarkupLine("[red]Invalid Units! Must be 1-10.[/]");
            Console.ReadKey();
            return -1;
        }

        static string GetValidCourseAllowed()
        {
            string[] validCourses = { "BSN", "BSCE", "BSARCH", "BSA", "BSHM", "ALL" };

            Console.Write("Course Allowed [BSN, BSCE, BSARCH, BSA, BSHM, ALL, or comma-separated] (or 0 to cancel): ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            while (true)
            {
                string input = Console.ReadLine()?.ToUpper();

                if (input == "0") return "0";
                if (input == "ALL") return "ALL";

                var courses = input.Split(',').Select(c => c.Trim()).ToArray();
                if (courses.All(c => validCourses.Contains(c)))
                    return string.Join(", ", courses);

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                AnsiConsole.MarkupLine("[red]Invalid Course![/]");
                Console.ReadKey();

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(left, top);
                Console.Write(new string(' ', Console.WindowWidth - left));
                Console.SetCursorPosition(left, top);
            }
        }

        static string GetEditCourseAllowed(string prompt, string oldValue)
        {
            string[] validCourses = { "BSN", "BSCE", "BSARCH", "BSA", "BSHM", "ALL" };

            Console.Write(prompt + " ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            while (true)
            {
                string input = Console.ReadLine()?.ToUpper();

                if (input == "0") return "0";
                if (string.IsNullOrWhiteSpace(input)) return oldValue;
                if (input == "ALL") return "ALL";

                var courses = input.Split(',').Select(c => c.Trim()).ToArray();
                if (courses.All(c => validCourses.Contains(c)))
                    return string.Join(", ", courses);

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 2);
                Console.Write(new string(' ', Console.WindowWidth));

                Console.SetCursorPosition(0, top + 1);
                AnsiConsole.MarkupLine("[red]Invalid Course![/]");
                Console.ReadKey();

                Console.SetCursorPosition(0, top);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 2);
                Console.Write(new string(' ', Console.WindowWidth));

                Console.SetCursorPosition(0, top);
                Console.Write(prompt + " ");
                left = Console.CursorLeft;
            }
        }

        static string GetValidYearAllowed()
        {
            Console.Write("Year Allowed [1, 2, 3, 4, ALL, or comma-separated] (or 0 to cancel): ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            while (true)
            {
                string input = Console.ReadLine()?.ToUpper();

                if (input == "0") return "0";
                if (input == "ALL") return "ALL";

                var years = input.Split(',').Select(y => y.Trim()).ToArray();
                if (years.All(y => new[] { "1", "2", "3", "4" }.Contains(y)))
                    return string.Join(", ", years);

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                AnsiConsole.MarkupLine("[red]Invalid Year![/]");
                Console.ReadKey();

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(left, top);
                Console.Write(new string(' ', Console.WindowWidth - left));
                Console.SetCursorPosition(left, top);
            }
        }

        static string GetEditYearAllowed(string prompt, string oldValue)
        {
            Console.Write(prompt + " ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            while (true)
            {
                string input = Console.ReadLine()?.ToUpper();

                if (input == "0") return "0";
                if (string.IsNullOrWhiteSpace(input)) return oldValue;
                if (input == "ALL") return "ALL";

                var years = input.Split(',').Select(y => y.Trim()).ToArray();
                if (years.All(y => new[] { "1", "2", "3", "4" }.Contains(y)))
                    return string.Join(", ", years);

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 2);
                Console.Write(new string(' ', Console.WindowWidth));

                Console.SetCursorPosition(0, top + 1);
                AnsiConsole.MarkupLine("[red]Invalid Year![/]");
                Console.ReadKey();

                Console.SetCursorPosition(0, top);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 2);
                Console.Write(new string(' ', Console.WindowWidth));

                Console.SetCursorPosition(0, top);
                Console.Write(prompt + " ");
                left = Console.CursorLeft;
            }
        }

        static string GetValidScheduleDay(string subjectCode, List<string> existingSchedules)
        {
            var dayPairs = new Dictionary<string, string>
        {
            { "MONDAY", "THURSDAY" },
            { "THURSDAY", "MONDAY" },
            { "TUESDAY", "FRIDAY" },
            { "FRIDAY", "TUESDAY" },
            { "WEDNESDAY", "SATURDAY" },
            { "SATURDAY", "WEDNESDAY" }
        };

            Console.Write("Day [Monday-Thursday, Tuesday-Friday, Wednesday-Saturday] (or 0 to cancel): ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            while (true)
            {
                string input = Console.ReadLine()?.ToUpper().Trim();

                if (input == "0") return "0";

                var days = input.Split(new[] { ' ', '-', ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (days.Length == 2)
                {
                    string day1 = days[0];
                    string day2 = days[1];

                    if (dayPairs.ContainsKey(day1) && dayPairs[day1] == day2)
                        return $"{day1}-{day2}";
                }

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                AnsiConsole.MarkupLine("[red]Invalid Day Pair![/]");
                Console.ReadKey();

                Console.SetCursorPosition(0, top);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 2);
                Console.Write(new string(' ', Console.WindowWidth));

                Console.SetCursorPosition(0, top);
                Console.Write("Day [Monday-Thursday, Tuesday-Friday, Wednesday-Saturday] (or 0 to cancel): ");
                left = Console.CursorLeft;
            }
        }

        static (string start, string end) GetValidScheduleTime()
        {
            string start = "";
            int top1 = 0;

            while (true)
            {
                Console.Write("Start Time [HH:mm] (or 0 to cancel): ");
                int left1 = Console.CursorLeft;
                top1 = Console.CursorTop;
                start = Console.ReadLine();

                if (start == "0") return ("0", "0");

                if (string.IsNullOrWhiteSpace(start) || start.Length != 5 || start[2] != ':' || !TimeSpan.TryParse(start, out _))
                {
                    Console.SetCursorPosition(0, top1 + 1);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, top1 + 1);
                    AnsiConsole.MarkupLine("[red]Invalid Time Format! Use HH:mm[/]");
                    Console.ReadKey();

                    Console.SetCursorPosition(0, top1);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, top1 + 1);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, top1 + 2);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, top1);
                    continue;
                }

                break;
            }

            while (true)
            {
                Console.Write("End Time [HH:mm] (or 0 to cancel): ");
                int left2 = Console.CursorLeft;
                int top2 = Console.CursorTop;
                string end = Console.ReadLine();

                if (end == "0") return ("0", "0");

                if (string.IsNullOrWhiteSpace(end) || end.Length != 5 || end[2] != ':' || !TimeSpan.TryParse(end, out _))
                {
                    Console.SetCursorPosition(0, top2 + 1);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, top2 + 1);
                    AnsiConsole.MarkupLine("[red]Invalid Time Format! Use HH:mm[/]");
                    Console.ReadKey();

                    Console.SetCursorPosition(0, top2);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, top2 + 1);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, top2 + 2);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, top2);
                    continue;
                }

                return (start, end);
            }
        }

        static string GetValidRoomNumber()
        {
            Console.Write("Room Number (or 0 to cancel): ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            while (true)
            {
                string input = Console.ReadLine();

                if (input == "0") return "0";

                if (!string.IsNullOrWhiteSpace(input))
                    return input.Trim();

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top + 1);
                AnsiConsole.MarkupLine("[red]Invalid Room Number![/]");
                Console.ReadKey();

                Console.SetCursorPosition(0, top + 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(left, top);
                Console.Write(new string(' ', Console.WindowWidth - left));
                Console.SetCursorPosition(left, top);
            }
        }


        //concflict checking
        static bool HasScheduleConflict(string subjectCode, string day, string start, string end, string room, List<string> existingSchedules)
        {
            var allSchedules = File.Exists(SchedulesFile) ? File.ReadAllLines(SchedulesFile).Skip(2).ToList() : new List<string>();
            allSchedules.AddRange(existingSchedules);

            TimeSpan newStart = TimeSpan.Parse(start);
            TimeSpan newEnd = TimeSpan.Parse(end);

            foreach (var schedule in allSchedules)
            {
                var parts = schedule.Split('|');
                if (parts.Length < 5) continue;

                string schDay = parts[1].Trim();
                string schStart = parts[2].Trim();
                string schEnd = parts[3].Trim();
                string schRoom = parts[4].Trim();

                if (schDay != day || schRoom != room) continue;

                if (!TimeSpan.TryParse(schStart, out TimeSpan sStart)) continue;
                if (!TimeSpan.TryParse(schEnd, out TimeSpan sEnd)) continue;

                bool timeOverlap = newStart < sEnd && newEnd > sStart;

                if (timeOverlap)
                    return true;
            }

            return false;
        }

        static bool CanStudentAddSubject(string studentCourse, int studentYear, string courseAllowed, string yearAllowed)
        {
            if (courseAllowed == "ALL" && yearAllowed == "ALL") return true;

            bool courseMatch = courseAllowed == "ALL" || courseAllowed.Split(',').Select(c => c.Trim()).Contains(studentCourse);
            bool yearMatch = yearAllowed == "ALL" || yearAllowed.Split(',').Select(y => y.Trim()).Contains(studentYear.ToString());

            return courseMatch && yearMatch;
        }

        static bool HasStudentScheduleConflict(string studentId, string newDay, string newStart, string newEnd)
        {
            var pending = File.ReadAllLines(PendingFile).Skip(2).Where(p => p.Split('|')[0].Trim() == studentId).ToList();
            var registered = File.ReadAllLines(RegisteredFile).Skip(2).Where(r => r.Split('|')[0].Trim() == studentId).ToList();

            var allStudentSchedules = pending.Concat(registered).ToList();

            TimeSpan newStartTime = TimeSpan.Parse(newStart);
            TimeSpan newEndTime = TimeSpan.Parse(newEnd);

            var newDays = newDay.Split('-');

            foreach (var schedule in allStudentSchedules)
            {
                var parts = schedule.Split('|');
                if (parts.Length < 6) continue;

                string existingDay = parts[2].Trim();
                TimeSpan existingStart = TimeSpan.Parse(parts[3].Trim());
                TimeSpan existingEnd = TimeSpan.Parse(parts[4].Trim());

                var existingDays = existingDay.Split('-');

                bool dayOverlap = newDays.Any(nd => existingDays.Any(ed => ed.Equals(nd, StringComparison.OrdinalIgnoreCase)));

                if (dayOverlap)
                {
                    if (newStartTime < existingEnd && newEndTime > existingStart)
                        return true;
                }
            }

            return false;
        }

        #endregion
    }
}