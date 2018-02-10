CREATE ROLE VotingApi
GO

GRANT EXECUTE ON ReferendumGet TO VotingApi
GO

GRANT EXECUTE ON ReferendumGetAll TO VotingApi
GO

GRANT EXECUTE ON ReferendumVote TO VotingApi
GO