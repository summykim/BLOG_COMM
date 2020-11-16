CREATE TABLE [dbo].[LoginUser]
(
	[LoginId] NVARCHAR(50) NOT NULL PRIMARY KEY, 
    [LoginPwd] NVARCHAR(50) NULL, 
    [UserName] NVARCHAR(100) NULL, 
    [reg_dtm] DATETIME NULL DEFAULT getdate()
)
