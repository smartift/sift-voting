-- Turn off row count
SET NOCOUNT ON
GO


-- Create initial table structure
CREATE TABLE Referendum
(
	Id INT NOT NULL IDENTITY(3, 1), -- SVP001 / SVP002 existed prior to this, let's keep it separate
	Question NVARCHAR(MAX) NOT NULL,
	StartTime DATETIME NOT NULL,
	EndTime DATETIME NOT NULL,
	CreateTime DATETIME NOT NULL
	PRIMARY KEY (Id)
)
GO

CREATE TABLE ReferendumAnswer
(
	ReferendumId INT NOT NULL,
	DisplayOrder TINYINT NOT NULL,
	Answer NVARCHAR(MAX) NOT NULL,
	PRIMARY KEY (ReferendumId, DisplayOrder),
	FOREIGN KEY (ReferendumId) REFERENCES Referendum(Id)
)
GO

CREATE TABLE Voter
(
	ReferendumId INT NOT NULL,
	[Address] CHAR(42) NOT NULL,
	VoteCount INT NOT NULL,
	Vote INT DEFAULT 0 NOT NULL,
	SignedVoteMessage NVARCHAR(MAX) NULL,
	PRIMARY KEY (ReferendumId, [Address]),
	FOREIGN KEY (ReferendumId) REFERENCES Referendum(Id)
)
GO

CREATE PROCEDURE ReferendumGet
(
	@id INT
)
AS
BEGIN
	-- Turn off row count
	SET NOCOUNT ON

	-- Lookup the referendum, then answers then voters as three datasets
	SELECT TOP 1 Question, StartTime, EndTime, CreateTime FROM Referendum WHERE Id = @id
	SELECT Answer FROM ReferendumAnswer WHERE ReferendumId = @id ORDER BY DisplayOrder ASC
	SELECT [Address], VoteCount, Vote, SignedVoteMessage FROM Voter WHERE ReferendumId = @id ORDER BY VoteCount DESC

	-- Return OK
	return 0
END
GO
