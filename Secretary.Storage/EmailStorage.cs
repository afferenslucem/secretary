using Dapper;
using Secretary.Storage.Models;

namespace Secretary.Storage;

public class EmailStorage: Storage, IEmailStorage
{
    public EmailStorage(string dbFile) : base(dbFile)
    {
    }

    public async Task SaveForDocument(long documentId, IEnumerable<Email> emails)
    {
        using var connection = this.GetConnection();

        await connection.OpenAsync();

        await connection.ExecuteAsync("delete from Emails where DocumentId = @documentId", new { documentId });

        foreach (var email in emails)
        {
            email.DocumentId = documentId;
        }
        
        await connection.ExecuteAsync(
            "insert into Emails (DocumentId, Address, DisplayName) " +
            "values (@DocumentId, @Address, @DisplayName);",
            emails);
    }

    public async Task<IEnumerable<Email>> GetForDocument(long documentId) 
    {
        using var connection = this.GetConnection();

        await connection.OpenAsync();

        var result = await connection.QueryAsync<Email>(
            "select DocumentId, Address, DisplayName from Emails where DocumentId = @documentId",
            new { documentId }
        );

        return result;
    }

    public async Task<long> GetCountForDocument(long documentId) 
    {
        using var connection = this.GetConnection();

        await connection.OpenAsync();

        var result = await connection.QueryFirstAsync<long>(
            "select count(*) from Emails where DocumentId = @documentId",
            new { documentId }
        );

        return result;
    }

    public void CreateTable()
    {
        using var connection = GetConnection();
        
        connection.Open();
        connection.Execute(
            @"create table Emails
            (
                DocumentId          integer not null ,
                Address             varchar(128) not null,
                DisplayName         varchar(128),
                FOREIGN KEY(DocumentId) REFERENCES Documents(Id)
            )");
        
        connection.Execute(
            @"CREATE INDEX Emails_DocumentId_idx
                ON Emails (DocumentId);");
    }
}