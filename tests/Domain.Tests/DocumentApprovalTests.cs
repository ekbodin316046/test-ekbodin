using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Tests;

public class DocumentApprovalTests
{
    private static readonly DateTime Now = new(2026, 8, 13, 21, 0, 0, DateTimeKind.Utc);
    private const string Program = "IT03";

    private static Document PendingDocument()
    {
        var document = new Document
        {
            Id = 1,
            DocumentName = "รายการที่ 1",
            Reason = "xxxxx",
            StatusId = (int)DocumentStatusCode.Pending,
        };
        document.StampCreated("SYSTEM", "SEED", Now.AddDays(-1));

        return document;
    }

    private static Document DocumentWith(DocumentStatusCode status)
    {
        var document = PendingDocument();
        document.StatusId = (int)status;
        return document;
    }

    [Fact]
    public void Approving_a_pending_document_moves_it_to_approved()
    {
        var document = PendingDocument();

        document.ChangeStatus(DocumentStatusCode.Approved, "งบประมาณถูกต้อง", "somchai", Program, Now);

        Assert.Equal((int)DocumentStatusCode.Approved, document.StatusId);
        Assert.False(document.IsPending);
        Assert.Equal("งบประมาณถูกต้อง", document.Reason);
        Assert.Equal(Now, document.UpdatedDate);
    }

    [Fact]
    public void Rejecting_a_pending_document_moves_it_to_rejected()
    {
        var document = PendingDocument();

        document.ChangeStatus(DocumentStatusCode.Rejected, "เอกสารไม่ครบ", "somchai", Program, Now);

        Assert.Equal((int)DocumentStatusCode.Rejected, document.StatusId);
        Assert.False(document.IsPending);
        Assert.Equal("เอกสารไม่ครบ", document.Reason);
    }

    [Fact]
    public void Transition_is_recorded_in_the_approval_log()
    {
        var document = PendingDocument();

        var log = document.ChangeStatus(
            DocumentStatusCode.Approved, "  ผ่านการตรวจสอบ  ", "somchai", Program, Now);

        Assert.Equal(1, log.DocumentId);
        Assert.Equal((int)DocumentStatusCode.Pending, log.FromStatusId);
        Assert.Equal((int)DocumentStatusCode.Approved, log.ToStatusId);
        Assert.Equal("ผ่านการตรวจสอบ", log.Reason);
        Assert.Equal("somchai", log.CreatedBy);
        Assert.Equal(Now, log.CreatedDate);
        Assert.Single(document.ApprovalLogs);
    }

    [Fact]
    public void A_decision_stamps_the_actor_and_screen_on_both_rows()
    {
        var document = PendingDocument();

        var log = document.ChangeStatus(
            DocumentStatusCode.Approved, "ผ่านการตรวจสอบ", "somchai", Program, Now);

        Assert.Equal(Program, log.CreatedProgram);
        Assert.Equal(Program, log.UpdatedProgram);
        Assert.Equal("somchai", log.UpdatedBy);

        // The document keeps the stamp of whoever created it and gains the
        // approver only on the update columns.
        Assert.Equal("SYSTEM", document.CreatedBy);
        Assert.Equal("SEED", document.CreatedProgram);
        Assert.Equal("somchai", document.UpdatedBy);
        Assert.Equal(Program, document.UpdatedProgram);
    }

    // The rule the exam calls out: "การที่อนุมัติแล้ว จะไม่สามารถเลือกอนุมัติซ้ำได้"
    [Theory]
    [InlineData(DocumentStatusCode.Approved, DocumentStatusCode.Approved)]
    [InlineData(DocumentStatusCode.Approved, DocumentStatusCode.Rejected)]
    [InlineData(DocumentStatusCode.Rejected, DocumentStatusCode.Approved)]
    [InlineData(DocumentStatusCode.Rejected, DocumentStatusCode.Rejected)]
    public void A_decided_document_cannot_be_acted_on_again(
        DocumentStatusCode current,
        DocumentStatusCode attempted)
    {
        var document = DocumentWith(current);

        var error = Assert.Throws<BusinessRuleException>(
            () => document.ChangeStatus(attempted, "เหตุผล", "somchai", Program, Now));

        Assert.Contains("ไม่สามารถดำเนินการซ้ำได้", error.Message);
        Assert.Equal((int)current, document.StatusId);
        Assert.Empty(document.ApprovalLogs);
    }

    [Fact]
    public void Pending_is_not_a_decision_a_caller_may_apply()
    {
        var document = PendingDocument();

        Assert.Throws<BusinessRuleException>(
            () => document.ChangeStatus(DocumentStatusCode.Pending, "เหตุผล", "somchai", Program, Now));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void A_reason_is_required(string reason)
    {
        var document = PendingDocument();

        var error = Assert.Throws<BusinessRuleException>(
            () => document.ChangeStatus(DocumentStatusCode.Approved, reason, "somchai", Program, Now));

        Assert.Equal("กรุณากรอกเหตุผล", error.Message);
        Assert.True(document.IsPending);
    }

    [Fact]
    public void A_failed_transition_leaves_the_document_untouched()
    {
        var document = DocumentWith(DocumentStatusCode.Approved);
        var reasonBefore = document.Reason;
        var updatedBefore = document.UpdatedDate;
        var updatedByBefore = document.UpdatedBy;

        Assert.Throws<BusinessRuleException>(
            () => document.ChangeStatus(DocumentStatusCode.Rejected, "เปลี่ยนใจ", "somchai", Program, Now));

        Assert.Equal(reasonBefore, document.Reason);
        Assert.Equal(updatedBefore, document.UpdatedDate);
        Assert.Equal(updatedByBefore, document.UpdatedBy);
    }
}
