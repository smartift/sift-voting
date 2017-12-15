DROP PROCEDURE ReferendumGetSummaries
GO

CREATE PROCEDURE ReferendumGetAll
AS
BEGIN
	-- Turn off row count
	SET NOCOUNT ON

	-- Lookup the referendum, then answers then voters as three datasets
	SELECT Id, Question, StartTime, EndTime, CreateTime FROM Referendum ORDER BY EndTime DESC, Id DESC
	SELECT ReferendumId Id, Answer FROM ReferendumAnswer ORDER BY Id, DisplayOrder ASC
	SELECT ReferendumId Id, [Address], VoteCount, Vote, SignedVoteMessage FROM Voter ORDER BY Id, VoteCount DESC

	-- Return OK
	return 0
END
GO

ALTER PROCEDURE ReferendumVote
(
	@referendumId INT,
	@address CHAR(42),
	@vote INT,
	@signature CHAR(132)
)
AS
BEGIN
	-- Turn off row count
	SET NOCOUNT ON

	-- Update the row, if it exists
	UPDATE Voter SET Vote=@vote, SignedVoteMessage=@signature, UpdateTime = GETUTCDATE() WHERE [Address] = @address AND ReferendumId = @referendumId

	-- Return OK
END
GO


ALTER PROCEDURE ReferendumGet
(
	@id INT
)
AS
BEGIN
	-- Turn off row count
	SET NOCOUNT ON

	-- Lookup the referendum, then answers then voters as three datasets
	SELECT TOP 1 Id, Question, StartTime, EndTime, CreateTime FROM Referendum WHERE Id = @id
	SELECT @id Id, Answer FROM ReferendumAnswer WHERE ReferendumId = @id ORDER BY DisplayOrder ASC
	SELECT @id Id, [Address], VoteCount, Vote, SignedVoteMessage FROM Voter WHERE ReferendumId = @id ORDER BY VoteCount DESC

	-- Return OK
	return 0
END
GO