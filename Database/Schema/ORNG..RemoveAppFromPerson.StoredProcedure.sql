SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ORNG.].[RemoveAppFromPerson]
@SubjectID BIGINT=NULL, @SubjectURI nvarchar(255)=NULL, @AppID INT, @SessionID UNIQUEIDENTIFIER=NULL, @Error BIT=NULL OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @AppInstanceID BIGINT
	DECLARE @TripleID BIGINT
	DECLARE @PersonID INT	
	DECLARE @PERSON_FILTER_ID INT
	DECLARE @InternalUserName nvarchar(50)
	DECLARE @PersonFilter nvarchar(50)

	IF (@SubjectID IS NULL)
		SET @SubjectID = [RDF.].fnURI2NodeID(@SubjectURI)
	
	-- Lookup the PersonID
	SELECT @PersonID = cast(InternalID as INT)
		FROM [RDF.Stage].[InternalNodeMap]
		WHERE Class = 'http://xmlns.com/foaf/0.1/Person' AND InternalType = 'Person' AND NodeID = @SubjectID

	-- Lookup the App Instance's NodeID
	SELECT @AppInstanceID = NodeID
		FROM [RDF.Stage].[InternalNodeMap]
		WHERE Class = 'http://orng.info/ontology/orng#ApplicationInstance' AND InternalType = 'ORNG Application Instance'
			AND InternalID = CAST(@PersonID AS VARCHAR(50)) + '-' + CAST(@AppID AS VARCHAR(50))
	
		
	-- now delete it
	BEGIN TRAN

		-- Delete the triple using DeleteType = 1 (changing the security group to 0)
		EXEC [RDF.].DeleteTriple	@SubjectID = @SubjectID,
									@ObjectID = @AppInstanceID,
									@DeleteType = 1,
									@SessionID = @SessionID, 
									@Error = @Error

		-- remove any filters
		SELECT @PERSON_FILTER_ID = (SELECT PersonFilterID FROM Apps WHERE AppID = @AppID)
		IF (@PERSON_FILTER_ID IS NOT NULL) 
			BEGIN
				SELECT @PersonID = cast(InternalID as INT) FROM [RDF.Stage].[InternalNodeMap]
					WHERE [NodeID] = @SubjectID AND Class = 'http://xmlns.com/foaf/0.1/Person'

				SELECT @InternalUserName = InternalUserName FROM [Profile.Data].[Person] WHERE PersonID = @PersonID
				SELECT @PersonFilter = PersonFilter FROM [Profile.Data].[Person.Filter] WHERE PersonFilterID = @PERSON_FILTER_ID

				DELETE FROM [Profile.Import].[PersonFilterFlag] WHERE InternalUserName = @InternalUserName AND personfilter = @PersonFilter
				DELETE FROM [Profile.Data].[Person.FilterRelationship] WHERE PersonID = @PersonID AND personFilterId = @PERSON_FILTER_ID
			END
	COMMIT
END


GO
