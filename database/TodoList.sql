USE [ToDo]

GO

CREATE TABLE [TodoLists](
    [ID] int PRiMARY KEY IDENTITY(1,1) NOT NULL,
    [ListTitle] VARCHAR(50) NOT NULL,
    [AccountID] int not null,
    FOREIGN KEY ([AccountID]) REFERENCES [Accounts](ID)
)

GO