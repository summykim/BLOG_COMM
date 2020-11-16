CREATE FUNCTION [dbo].[friendkey]
(

)
RETURNS INT
AS
BEGIN
	RETURN (select count(*)+1 from Friends)
END