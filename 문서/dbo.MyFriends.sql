CREATE TABLE [dbo].[MyFriends] (
    [Id]         NVARCHAR (50) NOT NULL,
    [nickname]   NVARCHAR (50) NOT NULL,
    [gubun_type] NVARCHAR (10) NULL,
    [reg_dtm]    DATETIME      DEFAULT (getdate()) NULL,
    [owner] NVARCHAR(50) NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

