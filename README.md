# FileCabinet -	Console application
The application implements various design patterns to present the OOP advantages
(such as Strategy, Template method etc). Data may be stored both in memory and file.
The commands are executed with the help of command handlers (Chain of Responsebility pattern).
Records may be validated using json validation file and enumerated using custom iterator.
Records may be generated using FileCbinetGenerator app. Also custom logging may be used during 
progam execution.

- Manual:
1) Open Console Command Line;
2) Choose application directory;
3) Type "filecabinetapp", press Enter;
4) Optional:
   - validation rule "-v custom" or "-v default";
   - storage source "-s memory" or "-s file";
   - stopwatch "-use-stopwatch"
   - logging "-use-logger"
   
   or find and start filecabinetapp.exe, memory storage and default validation rules will be set.
   
- Commands:
1) "help" - prints the help screen;
2) "exit" - exits the application;
3) "select" - prints record list (use parameters to print separate columns. Ex.: select firstname, lastname);
4) "create" - creates new record;
5) "export" - exports records into chosen file and format (csv or xml). Ex: export csv D:\\file.csv";
6) "find" - finds records by specified parameter. Ex: find firstname "Vadim";
7) "import" - Imports records from csv or xml file. Ex: import csv d:\\file.csv";
8) "delete" - Deletes specific record from record list (uses record field parameter, Ex: delete where id = '1');
9) "update" - Updates records using specific parameters (Ex: update set DateOfBirth = '5/18/1986' where FirstName='Stan' and LastName='Smith');
10) "purge" - Deletes record from *.db file in FilesystemService;
11) "stat" - Displays record list statistics;
12) "insert" - Inserts new record using delegated parameters. Ex: insert (id, firstname, lastname, dateofbirth...) values ('1', 'John', 'Doe', '5/18/1986'...)

# FileCabinetGenerator - Console application
This application generates records and stores them in .csv or .xml file.

- Manual:
1) Open Console Command Line;
2) Choose application directory;
3) Type "filecabinetgenerator --output-type (format) --output (path) --records-amount (count) --start-id (id)"
   - format: xml or csv;
   - path: file storage path (Ex.: d:\file.xml);
   - count: count of records to be generated;
   - id: start record id.

- Commands:
1) "help" - prints the help screen;
2) "exit" - exits the application;
3) "list" - prints generated record list;
4) "generate" - generates record list;
5) "export" - exports records into chosen file and format.