USE [Todo]

GO

CREATE TABLE [TodoLists]
(
    [ID] UNIQUEIDENTIFIER PRIMARY KEY,
    [ListTitle] NVARCHAR(50) NOT NULL,
    [Completed] BIT NOT NULL DEFAULT(0),
    [Contributors] NVARCHAR(max) NOT NULL
)

GO