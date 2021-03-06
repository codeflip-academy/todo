USE [ToDo]

GO

CREATE TABLE [TodoListItems]
(
    [ID] UNIQUEIDENTIFIER PRIMARY KEY,
    [Notes] NVARCHAR(200),
    [Completed] BIT NOT NULL DEFAULT(0),
    [Name] NVARCHAR(50),
    [DueDate] DATETIME,
    [ListID] UNIQUEIDENTIFIER,
    [HasSubItems] BIT NOT NULL DEFAULT(0),
    [Important] BIT,
    FOREIGN KEY (ListID) REFERENCES TodoLists (ID) ON DELETE CASCADE
)

GO