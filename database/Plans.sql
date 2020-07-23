USE [ToDo]

GO

CREATE TABLE [Plans](
    ID INT PRIMARY KEY NOT NULL,
    [Name] VARCHAR(7) NOT NULL,
    [MaxContributors] INT NOT NULL,
    [MaxLists] INT NOT NULL,
    [CanAddDueDates] BIT NOT NULL,
    [CanNotifyViaEmail] BIT NOT NULL,
)

GO