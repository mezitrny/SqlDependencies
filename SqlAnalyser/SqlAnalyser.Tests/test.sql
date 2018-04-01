CREATE TABLE dbo.Test(
  Id INT,
  Name VARCHAR(MAX),
  Surname VARCHAR(MAX),
  Age INT
)

GO

EXECUTE PROCEDURE dbo.AddColumn @Schema ='dbo', @Table = 'Test', @Column = 'Sex', @Type = 'INT', 
  @Updrade = 'UPDATE dbo.Test SET Sex = 1',
  @Downgrade = 'UPDATE dbo.Test SET Sex = 0'