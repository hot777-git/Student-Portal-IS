-- ============================================================
--  HAWASSA UNIVERSITY STUDENT INFORMATION SYSTEM
--  Full SQL Script for SQL Server Management Studio (SSMS)
--  Run this entire script once to set up the database
-- ============================================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'HawassaUniversityDB')
    DROP DATABASE HawassaUniversityDB;
GO

CREATE DATABASE HawassaUniversityDB;
GO

USE HawassaUniversityDB;
GO

-- ─────────────────────────────────────────────────────────────
-- TABLE: Students
-- ─────────────────────────────────────────────────────────────
CREATE TABLE Students (
    StudentID       VARCHAR(20)  PRIMARY KEY,   -- e.g. HwU/0001/16
    FirstName       NVARCHAR(80) NOT NULL,
    FatherName      NVARCHAR(80) NOT NULL,
    GrandFatherName NVARCHAR(80) NOT NULL,
    Gender          VARCHAR(10)  NOT NULL,
    DateOfBirth     DATE         NOT NULL,
    Email           NVARCHAR(120) UNIQUE NOT NULL,
    PhoneNumber     VARCHAR(20),
    Region          NVARCHAR(80),
    Woreda          NVARCHAR(80),
    Department      NVARCHAR(100) NOT NULL,
    YearOfStudy     INT           NOT NULL DEFAULT 1,
    Semester        INT           NOT NULL DEFAULT 1,
    AcademicYear    VARCHAR(20)   NOT NULL,
    CGPA            DECIMAL(4,2)  DEFAULT 0.00,
    Status          VARCHAR(20)   DEFAULT 'Active',
    IsScholarship   BIT           DEFAULT 0,
    EnrollmentDate  DATE          DEFAULT GETDATE(),
    Username        VARCHAR(50)   UNIQUE NOT NULL,
    PasswordHash    NVARCHAR(256) NOT NULL
);
GO

-- ─────────────────────────────────────────────────────────────
-- TABLE: Teachers
-- ─────────────────────────────────────────────────────────────
CREATE TABLE Teachers (
    TeacherID   INT IDENTITY(1,1) PRIMARY KEY,
    FullName    NVARCHAR(150) NOT NULL,
    Username    VARCHAR(50)   UNIQUE NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    Email       NVARCHAR(120) UNIQUE NOT NULL,
    PhoneNumber VARCHAR(20),
    Department  NVARCHAR(100),
    Qualification NVARCHAR(100),
    IsHead      BIT DEFAULT 0,
    IsActive    BIT DEFAULT 1,
    RegisteredAt DATETIME DEFAULT GETDATE()
);
GO

-- ─────────────────────────────────────────────────────────────
-- TABLE: HeadTeacherCredentials (fixed, set by Registrar)
-- ─────────────────────────────────────────────────────────────
CREATE TABLE HeadCredentials (
    CredID      INT IDENTITY(1,1) PRIMARY KEY,
    Username    VARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    Department  NVARCHAR(100) NOT NULL,
    AssignedBy  VARCHAR(100),
    CreatedAt   DATETIME DEFAULT GETDATE()
);
GO

-- ─────────────────────────────────────────────────────────────
-- TABLE: Courses
-- ─────────────────────────────────────────────────────────────
CREATE TABLE Courses (
    CourseID    INT IDENTITY(1,1) PRIMARY KEY,
    CourseCode  VARCHAR(15)   UNIQUE NOT NULL,
    CourseName  NVARCHAR(150) NOT NULL,
    Department  NVARCHAR(100) NOT NULL,
    CreditHours INT NOT NULL DEFAULT 3,
    YearOffered INT NOT NULL DEFAULT 1,
    Semester    INT NOT NULL DEFAULT 1
);
GO

-- ─────────────────────────────────────────────────────────────
-- TABLE: TeacherCourseAssignment  (Head assigns teacher to course)
-- ─────────────────────────────────────────────────────────────
CREATE TABLE TeacherCourseAssignment (
    AssignmentID INT IDENTITY(1,1) PRIMARY KEY,
    TeacherID    INT NOT NULL REFERENCES Teachers(TeacherID),
    CourseID     INT NOT NULL REFERENCES Courses(CourseID),
    AcademicYear VARCHAR(20) NOT NULL,
    Semester     INT NOT NULL,
    AssignedAt   DATETIME DEFAULT GETDATE(),
    UNIQUE(TeacherID, CourseID, AcademicYear, Semester)
);
GO

-- ─────────────────────────────────────────────────────────────
-- TABLE: Enrollments
-- ─────────────────────────────────────────────────────────────
CREATE TABLE Enrollments (
    EnrollmentID INT IDENTITY(1,1) PRIMARY KEY,
    StudentID    VARCHAR(20) NOT NULL REFERENCES Students(StudentID),
    CourseID     INT NOT NULL REFERENCES Courses(CourseID),
    AcademicYear VARCHAR(20) NOT NULL,
    Semester     INT NOT NULL,
    EnrolledOn   DATETIME DEFAULT GETDATE(),
    UNIQUE(StudentID, CourseID, AcademicYear, Semester)
);
GO

-- ─────────────────────────────────────────────────────────────
-- TABLE: Grades
-- ─────────────────────────────────────────────────────────────
CREATE TABLE Grades (
    GradeID      INT IDENTITY(1,1) PRIMARY KEY,
    EnrollmentID INT NOT NULL REFERENCES Enrollments(EnrollmentID),
    TeacherID    INT REFERENCES Teachers(TeacherID),
    Midterm      DECIMAL(5,2) DEFAULT 0,   -- /30
    Final        DECIMAL(5,2) DEFAULT 0,   -- /50
    Assignment   DECIMAL(5,2) DEFAULT 0,   -- /10
    Quiz         DECIMAL(5,2) DEFAULT 0,   -- /10
    Total        DECIMAL(5,2) DEFAULT 0,   -- /100
    LetterGrade  VARCHAR(5),
    GradePoint   DECIMAL(3,1) DEFAULT 0,
    IsFinalized  BIT DEFAULT 0,
    GradedOn     DATETIME DEFAULT GETDATE()
);
GO

-- ─────────────────────────────────────────────────────────────
-- TABLE: PaymentHistory
-- ─────────────────────────────────────────────────────────────
CREATE TABLE PaymentHistory (
    PaymentID     INT IDENTITY(1,1) PRIMARY KEY,
    StudentID     VARCHAR(20) NOT NULL REFERENCES Students(StudentID),
    FeeType       NVARCHAR(80) NOT NULL,   -- Tuition, Dormitory, Cafeteria
    Amount        DECIMAL(10,2) NOT NULL,
    AmountPaid    DECIMAL(10,2) DEFAULT 0,
    AcademicYear  VARCHAR(20) NOT NULL,
    Semester      INT NOT NULL,
    DueDate       DATE NOT NULL,
    PaidDate      DATE,
    ReceiptNumber VARCHAR(50),
    Status        VARCHAR(20) DEFAULT 'Unpaid'
);
GO

-- ─────────────────────────────────────────────────────────────
-- TABLE: AcademicCalendar
-- ─────────────────────────────────────────────────────────────
CREATE TABLE AcademicCalendar (
    CalendarID   INT IDENTITY(1,1) PRIMARY KEY,
    EventTitle   NVARCHAR(200) NOT NULL,
    EventDate    DATE          NOT NULL,
    EndDate      DATE,
    Description  NVARCHAR(500),
    AcademicYear VARCHAR(20),
    EventType    VARCHAR(50)   -- Registration, Exam, Holiday, Graduation
);
GO

-- ─────────────────────────────────────────────────────────────
-- TABLE: RulesAndRegulations
-- ─────────────────────────────────────────────────────────────
CREATE TABLE RulesAndRegulations (
    RuleID      INT IDENTITY(1,1) PRIMARY KEY,
    Category    NVARCHAR(100) NOT NULL,
    Title       NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    CreatedAt   DATETIME DEFAULT GETDATE()
);
GO

-- ─────────────────────────────────────────────────────────────
-- STORED PROCEDURES
-- ─────────────────────────────────────────────────────────────

-- Student login
CREATE PROCEDURE sp_StudentLogin
    @Username    VARCHAR(50),
    @PasswordHash NVARCHAR(256)
AS
BEGIN
    SELECT * FROM Students
    WHERE Username = @Username AND PasswordHash = @PasswordHash AND Status = 'Active';
END
GO

-- Teacher login
CREATE PROCEDURE sp_TeacherLogin
    @Username    VARCHAR(50),
    @PasswordHash NVARCHAR(256)
AS
BEGIN
    SELECT * FROM Teachers
    WHERE Username = @Username AND PasswordHash = @PasswordHash AND IsActive = 1;
END
GO

-- Head teacher login
CREATE PROCEDURE sp_HeadLogin
    @Username    VARCHAR(50),
    @PasswordHash NVARCHAR(256)
AS
BEGIN
    SELECT * FROM HeadCredentials
    WHERE Username = @Username AND PasswordHash = @PasswordHash;
END
GO

-- Register student (by Registrar)
CREATE PROCEDURE sp_RegisterStudent
    @StudentID   VARCHAR(20),
    @FirstName   NVARCHAR(80),
    @FatherName  NVARCHAR(80),
    @GrandFatherName NVARCHAR(80),
    @Gender      VARCHAR(10),
    @DateOfBirth DATE,
    @Email       NVARCHAR(120),
    @PhoneNumber VARCHAR(20),
    @Region      NVARCHAR(80),
    @Woreda      NVARCHAR(80),
    @Department  NVARCHAR(100),
    @YearOfStudy INT,
    @Semester    INT,
    @AcademicYear VARCHAR(20),
    @IsScholarship BIT,
    @Username    VARCHAR(50),
    @PasswordHash NVARCHAR(256)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Students WHERE Username=@Username OR Email=@Email OR StudentID=@StudentID)
    BEGIN
        RAISERROR('Student already exists.', 16, 1);
        RETURN;
    END
    INSERT INTO Students (StudentID,FirstName,FatherName,GrandFatherName,Gender,DateOfBirth,
        Email,PhoneNumber,Region,Woreda,Department,YearOfStudy,Semester,AcademicYear,
        IsScholarship,Username,PasswordHash)
    VALUES (@StudentID,@FirstName,@FatherName,@GrandFatherName,@Gender,@DateOfBirth,
        @Email,@PhoneNumber,@Region,@Woreda,@Department,@YearOfStudy,@Semester,@AcademicYear,
        @IsScholarship,@Username,@PasswordHash);
END
GO

-- Register teacher (self-registration)
CREATE PROCEDURE sp_RegisterTeacher
    @FullName    NVARCHAR(150),
    @Username    VARCHAR(50),
    @PasswordHash NVARCHAR(256),
    @Email       NVARCHAR(120),
    @PhoneNumber VARCHAR(20),
    @Department  NVARCHAR(100),
    @Qualification NVARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Teachers WHERE Username=@Username OR Email=@Email)
    BEGIN
        RAISERROR('Teacher already exists.', 16, 1);
        RETURN;
    END
    INSERT INTO Teachers (FullName,Username,PasswordHash,Email,PhoneNumber,Department,Qualification)
    VALUES (@FullName,@Username,@PasswordHash,@Email,@PhoneNumber,@Department,@Qualification);
END
GO

-- Get student grades
CREATE PROCEDURE sp_GetStudentGrades
    @StudentID VARCHAR(20)
AS
BEGIN
    SELECT c.CourseCode, c.CourseName, c.CreditHours, e.AcademicYear, e.Semester,
           g.Midterm, g.Final, g.Assignment, g.Quiz, g.Total,
           g.LetterGrade, g.GradePoint, g.IsFinalized
    FROM Enrollments e
    INNER JOIN Courses c ON e.CourseID = c.CourseID
    LEFT JOIN  Grades  g ON g.EnrollmentID = e.EnrollmentID
    WHERE e.StudentID = @StudentID
    ORDER BY e.AcademicYear, e.Semester, c.CourseCode;
END
GO

-- Submit/Update grade
CREATE PROCEDURE sp_SubmitGrade
    @EnrollmentID INT,
    @TeacherID    INT,
    @Midterm      DECIMAL(5,2),
    @Final        DECIMAL(5,2),
    @Assignment   DECIMAL(5,2),
    @Quiz         DECIMAL(5,2)
AS
BEGIN
    DECLARE @Total DECIMAL(5,2) = @Midterm + @Final + @Assignment + @Quiz;
    DECLARE @Letter VARCHAR(5);
    DECLARE @Point  DECIMAL(3,1);

    -- Ethiopian HEI grading
    IF @Total >= 90  BEGIN SET @Letter='A+';  SET @Point=4.0; END
    ELSE IF @Total >= 85 BEGIN SET @Letter='A';  SET @Point=4.0;  END
    ELSE IF @Total >= 80 BEGIN SET @Letter='A-'; SET @Point=3.75; END
    ELSE IF @Total >= 75 BEGIN SET @Letter='B+'; SET @Point=3.5;  END
    ELSE IF @Total >= 70 BEGIN SET @Letter='B';  SET @Point=3.0;  END
    ELSE IF @Total >= 65 BEGIN SET @Letter='B-'; SET @Point=2.75; END
    ELSE IF @Total >= 60 BEGIN SET @Letter='C+'; SET @Point=2.5;  END
    ELSE IF @Total >= 55 BEGIN SET @Letter='C';  SET @Point=2.0;  END
    ELSE IF @Total >= 50 BEGIN SET @Letter='C-'; SET @Point=1.75; END
    ELSE IF @Total >= 45 BEGIN SET @Letter='D';  SET @Point=1.0;  END
    ELSE BEGIN SET @Letter='F'; SET @Point=0.0; END

    IF EXISTS (SELECT 1 FROM Grades WHERE EnrollmentID = @EnrollmentID)
        UPDATE Grades SET
            TeacherID=@TeacherID, Midterm=@Midterm, Final=@Final,
            Assignment=@Assignment, Quiz=@Quiz, Total=@Total,
            LetterGrade=@Letter, GradePoint=@Point, GradedOn=GETDATE()
        WHERE EnrollmentID = @EnrollmentID AND IsFinalized=0;
    ELSE
        INSERT INTO Grades (EnrollmentID,TeacherID,Midterm,Final,Assignment,Quiz,Total,LetterGrade,GradePoint)
        VALUES (@EnrollmentID,@TeacherID,@Midterm,@Final,@Assignment,@Quiz,@Total,@Letter,@Point);
END
GO

-- Generate next StudentID
CREATE PROCEDURE sp_GenerateStudentID
    @Year INT,
    @NewID VARCHAR(20) OUTPUT
AS
BEGIN
    DECLARE @Count INT;
    SELECT @Count = COUNT(*) FROM Students WHERE AcademicYear LIKE CAST(@Year AS VARCHAR) + '%';
    SET @NewID = 'HwU/' + RIGHT('0000' + CAST(@Count+1 AS VARCHAR), 4) + '/' + RIGHT(CAST(@Year AS VARCHAR), 2);
END
GO

-- Assign head credentials
CREATE PROCEDURE sp_AssignHeadCredentials
    @Username    VARCHAR(50),
    @PasswordHash NVARCHAR(256),
    @Department  NVARCHAR(100),
    @AssignedBy  VARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM HeadCredentials WHERE Username=@Username)
        UPDATE HeadCredentials SET PasswordHash=@PasswordHash, Department=@Department WHERE Username=@Username;
    ELSE
        INSERT INTO HeadCredentials (Username,PasswordHash,Department,AssignedBy)
        VALUES (@Username,@PasswordHash,@Department,@AssignedBy);
END
GO

-- ─────────────────────────────────────────────────────────────
-- SEED DATA
-- ─────────────────────────────────────────────────────────────

-- Courses
INSERT INTO Courses (CourseCode,CourseName,Department,CreditHours,YearOffered,Semester) VALUES
('CS1011','Introduction to Programming','Computer Science',3,1,1),
('CS1022','Discrete Mathematics','Computer Science',3,1,2),
('CS2011','Data Structures and Algorithms','Computer Science',4,2,1),
('CS2022','Database Management Systems','Computer Science',3,2,2),
('CS3011','Operating Systems','Computer Science',4,3,1),
('CS3022','Computer Networks','Computer Science',3,3,2),
('SE2011','Software Engineering I','Software Engineering',3,2,1),
('MATH1011','Calculus I','Mathematics',4,1,1),
('MATH1022','Calculus II','Mathematics',3,1,2),
('EE2011','Circuit Theory','Electrical Engineering',4,2,1),
('MGT1011','Principles of Management','Management',3,1,1),
('ECON1011','Microeconomics','Economics',3,1,1);
GO

-- Academic Calendar
INSERT INTO AcademicCalendar (EventTitle,EventDate,EndDate,Description,AcademicYear,EventType) VALUES
('Student Registration - Semester 1','2024-09-01','2024-09-07','New and continuing student registration','2016/2017','Registration'),
('Semester 1 Classes Begin','2024-09-08',NULL,'First day of academic classes','2016/2017','Academic'),
('Mid-Semester Break','2024-10-28','2024-11-01','Break between exam periods','2016/2017','Holiday'),
('Midterm Examinations','2024-11-04','2024-11-15','Midterm exam period for all departments','2016/2017','Exam'),
('Ethiopian Christmas (Gena)','2025-01-07',NULL,'Public holiday - Gena','2016/2017','Holiday'),
('Final Examinations - Semester 1','2025-01-20','2025-02-05','Final exam period Semester 1','2016/2017','Exam'),
('Ethiopian Easter (Fasika)','2025-04-20',NULL,'Public holiday - Fasika','2016/2017','Holiday'),
('Semester 2 Registration','2025-02-10','2025-02-14','Registration for Semester 2','2016/2017','Registration'),
('Semester 2 Classes Begin','2025-02-17',NULL,'Second semester classes start','2016/2017','Academic'),
('Final Examinations - Semester 2','2025-06-02','2025-06-20','Final exam period Semester 2','2016/2017','Exam'),
('Graduation Ceremony','2025-07-15',NULL,'Annual graduation ceremony','2016/2017','Graduation'),
('Summer Semester','2025-07-21','2025-08-29','Optional summer semester','2016/2017','Academic');
GO

-- Rules and Regulations
INSERT INTO RulesAndRegulations (Category,Title,Description) VALUES
('Attendance','Minimum Attendance Requirement','Students must attend a minimum of 75% of classes for each course. Students who fail to meet this requirement will not be allowed to sit for the final examination.'),
('Attendance','Absence Notification','Students who cannot attend classes due to illness or emergency must notify the department office within 48 hours and provide a medical certificate or supporting document.'),
('Grading','Grade Distribution','Assessment is distributed as follows: Midterm Examination = 30%, Final Examination = 50%, Assignment = 10%, Quiz = 10%. Total = 100%.'),
('Grading','GPA Requirements','Students must maintain a minimum CGPA of 2.00 to continue their studies. Students with CGPA below 2.00 will be placed on academic probation.'),
('Grading','Grade Appeal','Students may appeal a grade within 10 working days of grade release. Appeals must be submitted in writing to the Department Head.'),
('Examinations','Exam Conduct','Students must bring their student ID card to all examinations. Mobile phones and electronic devices are strictly prohibited during exams. Cheating will result in automatic failure and disciplinary action.'),
('Examinations','Supplementary Exam','Students who score between 40-44% in the final examination may apply for a supplementary exam. Supplementary exams are held once per semester.'),
('Academic Integrity','Plagiarism Policy','All academic work must be original. Plagiarism, cheating, and academic dishonesty are serious offences. Penalties range from zero marks to expulsion depending on severity.'),
('Academic Integrity','Dress Code','Students are expected to dress decently and respectfully within university premises. Inappropriate attire will not be tolerated.'),
('Registration','Course Registration','Students must register for courses at the beginning of each semester within the designated registration period. Late registration incurs a penalty fee.'),
('Registration','Credit Load','Full-time students are expected to carry between 15-21 credit hours per semester. Overloading beyond 21 credits requires Dean''s approval.'),
('Fees','Tuition Payment','Tuition fees must be paid in full or via an approved installment plan before the end of the first month of each semester.'),
('Fees','Scholarship','Merit scholarships are awarded to students with CGPA of 3.75 and above. Government scholarships are renewed each semester based on academic performance.'),
('Conduct','Code of Conduct','Students are expected to maintain respect for fellow students, staff, and university property. Any form of harassment, violence, or discrimination is grounds for suspension or expulsion.'),
('Conduct','Library Rules','Library materials must be returned on time. Overdue materials incur fines. Damage or loss of library materials must be compensated by the responsible student.');
GO

-- Head credentials (password: Head@HwU2024)
-- In the app this is SHA256 hashed. For SSMS seed we use a known hash.
INSERT INTO HeadCredentials (Username,PasswordHash,Department,AssignedBy) VALUES
('head.cs','8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918','Computer Science','Registrar'),
('head.se','8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918','Software Engineering','Registrar');
GO

PRINT 'HawassaUniversityDB created successfully!';
GO