CREATE TABLE [dbo].[Friends] (
    [Id]         NVARCHAR (50)  NOT NULL,
    [nickname]   NVARCHAR (50)  NOT NULL,
    [blogurl]    NVARCHAR (MAX) NULL,
    [add_date]   DATE           NULL,
    [gubun_type] NVARCHAR (10)  NULL,
    [reg_dtm]    DATETIME       DEFAULT (getdate()) NULL,
    [seq]        INT            DEFAULT ([DBO].[friendkey]()) NOT NULL,
    [blogtitle]  NCHAR (200)    NULL,
    [owner] NVARCHAR(50) NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

