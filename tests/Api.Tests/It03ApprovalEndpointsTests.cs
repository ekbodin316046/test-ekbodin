using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Application.Features.IT.IT03.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Tests;

public class It03ApprovalEndpointsTests : IDisposable
{
    private const int PendingDocument = 1;
    private const int SecondPendingDocument = 4;
    private const int ApprovedDocument = 2;
    private const int RejectedDocument = 3;

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly It03ApiFactory _factory = new();
    private readonly HttpClient _client;

    public It03ApprovalEndpointsTests()
    {
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }

    private async Task<IReadOnlyList<DocumentListItemDto>> GetDocumentsAsync()
    {
        var documents = await _client.GetFromJsonAsync<List<DocumentListItemDto>>(
            "/api/it03/documents", Json);

        Assert.NotNull(documents);
        return documents;
    }

    private async Task<DocumentListItemDto> GetDocumentAsync(int id) =>
        (await GetDocumentsAsync()).Single(document => document.Id == id);

    private Task<HttpResponseMessage> DecideAsync(
        string action,
        IEnumerable<int> ids,
        string reason,
        string? actingUser = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/it03/documents/{action}")
        {
            Content = JsonContent.Create(new { documentIds = ids, reason }),
        };

        if (actingUser is not null)
        {
            request.Headers.Add("X-User", actingUser);
        }

        return _client.SendAsync(request);
    }

    private static async Task<ProblemDetails> ReadProblemAsync(HttpResponseMessage response)
    {
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(Json);

        Assert.NotNull(problem);
        return problem;
    }

    [Fact]
    public async Task Seeded_documents_match_the_mockup()
    {
        var documents = await GetDocumentsAsync();

        Assert.Equal(10, documents.Count);
        Assert.Equal(6, documents.Count(document => document.IsPending));
        Assert.Equal("รายการที่ 1", documents[0].DocumentName);
        Assert.Equal("อนุมัติ", documents.Single(d => d.Id == ApprovedDocument).StatusNameTh);
        Assert.Equal("ไม่อนุมัติ", documents.Single(d => d.Id == RejectedDocument).StatusNameTh);
    }

    [Fact]
    public async Task Approving_pending_documents_moves_every_one_of_them()
    {
        var response = await DecideAsync(
            "approve", [PendingDocument, SecondPendingDocument], "เอกสารครบถ้วน");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DecisionResultDto>(Json);
        Assert.NotNull(result);
        Assert.Equal(2, result.AffectedCount);
        Assert.Equal("อนุมัติ", result.StatusNameTh);

        var documents = await GetDocumentsAsync();
        foreach (var id in new[] { PendingDocument, SecondPendingDocument })
        {
            var document = documents.Single(d => d.Id == id);
            Assert.Equal("อนุมัติ", document.StatusNameTh);
            Assert.False(document.IsPending);
            Assert.Equal("เอกสารครบถ้วน", document.Reason);
        }
    }

    // The rule the brief calls out, enforced server-side rather than only by the
    // disabled checkbox in the UI.
    [Theory]
    [InlineData("approve", ApprovedDocument)]
    [InlineData("reject", ApprovedDocument)]
    [InlineData("approve", RejectedDocument)]
    [InlineData("reject", RejectedDocument)]
    public async Task Acting_on_a_decided_document_is_a_conflict(string action, int documentId)
    {
        var before = await GetDocumentAsync(documentId);

        var response = await DecideAsync(action, [documentId], "ลองดำเนินการซ้ำ");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var problem = await ReadProblemAsync(response);
        Assert.Equal("ไม่สามารถดำเนินการได้", problem.Title);
        Assert.Contains("ไม่สามารถดำเนินการซ้ำได้", problem.Detail);

        var after = await GetDocumentAsync(documentId);
        Assert.Equal(before.StatusNameTh, after.StatusNameTh);
        Assert.Equal(before.Reason, after.Reason);
    }

    // A batch is all-or-nothing: the caller selected these rows together, so
    // approving a subset would be a state they never asked for.
    [Fact]
    public async Task A_batch_holding_one_decided_document_changes_nothing()
    {
        var response = await DecideAsync(
            "approve", [PendingDocument, ApprovedDocument, SecondPendingDocument], "อนุมัติทั้งชุด");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var documents = await GetDocumentsAsync();
        Assert.True(documents.Single(d => d.Id == PendingDocument).IsPending);
        Assert.True(documents.Single(d => d.Id == SecondPendingDocument).IsPending);
        Assert.Equal(6, documents.Count(document => document.IsPending));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task A_decision_without_a_reason_is_rejected(string reason)
    {
        var response = await DecideAsync("approve", [PendingDocument], reason);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("ข้อมูลไม่ถูกต้อง", (await ReadProblemAsync(response)).Title);
        Assert.True((await GetDocumentAsync(PendingDocument)).IsPending);
    }

    [Fact]
    public async Task A_decision_without_any_document_is_rejected()
    {
        var response = await DecideAsync("approve", [], "ไม่ได้เลือกอะไรเลย");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task An_unknown_document_is_reported_as_missing()
    {
        var response = await DecideAsync("approve", [9999], "ทดสอบ");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("9999", (await ReadProblemAsync(response)).Detail);
    }

    [Fact]
    public async Task The_approval_log_records_who_decided_and_why()
    {
        await DecideAsync("reject", [PendingDocument], "  งบประมาณไม่พอ  ", actingUser: "somsri.t");

        var entries = await _client.GetFromJsonAsync<List<ApprovalLogDto>>(
            $"/api/it03/documents/{PendingDocument}/logs", Json);

        var entry = Assert.Single(entries!);
        Assert.Equal("รออนุมัติ", entry.FromStatusNameTh);
        Assert.Equal("ไม่อนุมัติ", entry.ToStatusNameTh);
        Assert.Equal("งบประมาณไม่พอ", entry.Reason);
        Assert.Equal("somsri.t", entry.ActionBy);
    }

    [Fact]
    public async Task History_is_empty_for_a_document_nobody_has_touched()
    {
        var entries = await _client.GetFromJsonAsync<List<ApprovalLogDto>>(
            $"/api/it03/documents/{PendingDocument}/logs", Json);

        Assert.Empty(entries!);
    }

    [Fact]
    public async Task History_of_a_missing_document_is_not_found()
    {
        var response = await _client.GetAsync("/api/it03/documents/9999/logs");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Status_master_reports_the_document_counts()
    {
        var statuses = await _client.GetFromJsonAsync<List<DocumentStatusDto>>(
            "/api/it03/statuses", Json);

        Assert.NotNull(statuses);
        Assert.Equal(3, statuses.Count);
        Assert.Equal(6, statuses.Single(status => status.Code == "PENDING").DocumentCount);
        Assert.Equal(2, statuses.Single(status => status.Code == "APPROVED").DocumentCount);
        Assert.Equal(2, statuses.Single(status => status.Code == "REJECTED").DocumentCount);
    }
}
