SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE  [ORNG.].[IsRegistered](@Subject BIGINT = NULL, @Uri nvarchar(255) = NULL, @AppID INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @nodeid bigint
	
	IF (@Subject IS NOT NULL) 
		SET @nodeId = @Subject
	ELSE		
		SELECT @nodeid = [RDF.].[fnURI2NodeID](@Uri);

	SELECT * from [ORNG.].AppRegistry where AppID=@AppID AND NodeID = @NodeID 
END


GO
