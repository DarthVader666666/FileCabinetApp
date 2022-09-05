# FileCabinetTask -	Console application
	The application implements various design patterns to present the OOP advantages
(such as Strategy, Template method etc). Data may be stored both in memory and file.
The commands are executed with the help of command handlers (Chain of Responsebility pattern).
Records are being validated using json validation file and enumerated using custom iterator.
Records may be generated using FileCbinetGenerator.dll. Also custom logging is being used during 
progam execution.

# Manual:
1) Open Console Command Line
2) Choose application directory
3) Type "filecabinetapp", press Enter;
   Optional:
   - validation rule "-v custom" or "-v default";
   - storage source "-s memory" or "-s file";
   - stopwatch "-use-stopwatch"
   - logging "-use-logger"
   
   or find and start filecabinetapp.exe, memory storage and default validation rules will be set.
   
# Commands:
"help" - prints the help screen;
"exit" - exits the application;
"select" - prints record list (use parameters to print separate columns. Ex.: select firstname, lastname);
"create" - creates new record;
"export" - exports records into chosen file and format (csv or xml). Ex: export csv D:\\file.csv";
"find" - finds records by specified parameter. Ex: find firstname "Vadim";
"import" - Imports records from csv or xml file. Ex: import csv d:\\file.csv";
"delete" - Deletes specific record from record list (uses record field parameter, Ex: delete where id = '1');
"update" - Updates records using specific parameters (Ex: update set DateOfBirth = '5/18/1986' where FirstName='Stan' and LastName='Smith');
"purge" - Deletes record from *.db file in FilesystemService;
"stat" - Displays record list statistics;
"insert" - Inserts new record using delegated parameters. Ex: insert (id, firstname, lastname, dateofbirth...) values ('1', 'John', 'Doe', '5/18/1986'...)

