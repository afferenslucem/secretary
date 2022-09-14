using Dapper;
using Secretary.Storage.Models;

namespace Secretary.Storage;

public class DocumentStorage: Storage, IDocumentStorage
{
    public DocumentStorage(string dbFile) : base(dbFile)
    {
    }

    public async Task<Document> GetOrCreateDocument(long chatId, string documentName)
    {
        var document = await this.GetDocument(chatId, documentName);

        if (document != null) return document;

        document = await this.SaveDocument(chatId, documentName);

        return document;
    }

    private async Task<Document> GetDocument(long chatId, string documentName)
    {
        using var connection = this.GetConnection();

        await connection.OpenAsync();

        var document = await connection.QueryFirstOrDefaultAsync<Document>(
            @"select Id, ChatId, DocumentName from Documents where ChatId = @chatId and DocumentName = @documentName",
            new { chatId, documentName }
        );

        return document;
    }

    private async Task<Document> SaveDocument(long chatId, string documentName)
    {
        using var connection = this.GetConnection();

        await connection.OpenAsync();

        var document = new Document(chatId, documentName);
        
        var id = await connection.QueryFirstOrDefaultAsync<long>(
            @"insert into Documents 
                (ChatId, DocumentName) values 
                (@ChatId, @DocumentName);" + 
            "select last_insert_rowid()",
            document
        );

        document.Id = id;
        
        return document;
    }

    public void CreateTable()
    {
        using var connection = GetConnection();
        
        connection.Open();
        connection.Execute(
            @"create table Documents
            (
                ChatId              integer not null,
                DocumentName        varchar(128) not null,
                Id                  integer primary key autoincrement not null
            )");
        
        connection.Execute(
            @"CREATE INDEX Documents_ChatId_idx
                ON Documents (ChatId);");
    }
}