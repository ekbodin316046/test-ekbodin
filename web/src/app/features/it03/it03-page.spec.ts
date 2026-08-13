import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentListItem } from '../../core/models/it03.models';
import { It03Page } from './it03-page';

function pending(id: number, name: string): DocumentListItem {
  return {
    id,
    documentName: name,
    reason: null,
    statusId: 1,
    statusCode: 'PENDING',
    statusNameTh: 'รออนุมัติ',
    isPending: true,
    createdAt: '2026-08-01T09:00:00Z',
    updatedAt: '2026-08-01T09:00:00Z',
  };
}

const DECIDED: DocumentListItem = {
  id: 2,
  documentName: 'ใบเบิกวัสดุสำนักงาน',
  reason: 'เอกสารครบถ้วน',
  statusId: 2,
  statusCode: 'APPROVED',
  statusNameTh: 'อนุมัติ',
  isPending: false,
  createdAt: '2026-08-01T09:00:00Z',
  updatedAt: '2026-08-02T10:00:00Z',
};

const DOCUMENTS: DocumentListItem[] = [
  pending(1, 'ใบขออนุมัติซื้อครุภัณฑ์'),
  DECIDED,
  pending(4, 'ใบขออนุมัติเดินทางไปราชการ'),
];

describe('It03Page', () => {
  let fixture: ComponentFixture<It03Page>;
  let http: HttpTestingController;

  async function settle(): Promise<void> {
    fixture.detectChanges();
    await fixture.whenStable();
  }

  function rowCheckboxes(): HTMLInputElement[] {
    return Array.from(fixture.nativeElement.querySelectorAll('tbody input[type="checkbox"]'));
  }

  function approveButton(): HTMLButtonElement {
    return fixture.nativeElement.querySelector('.btn-approve');
  }

  function rejectButton(): HTMLButtonElement {
    return fixture.nativeElement.querySelector('.btn-reject');
  }

  function approvalDialog(): HTMLElement | null {
    return fixture.nativeElement.querySelector('app-approval-dialog');
  }

  function dialogButtons(): HTMLButtonElement[] {
    return Array.from(
      fixture.nativeElement.querySelectorAll('app-approval-dialog .modal-actions button'),
    );
  }

  function textOf(selector: string): string {
    return fixture.nativeElement.querySelector(selector)?.textContent ?? '';
  }

  async function select(...rowIndexes: number[]): Promise<void> {
    for (const index of rowIndexes) {
      rowCheckboxes()[index].click();
    }
    await settle();
  }

  async function fillReason(text: string): Promise<void> {
    const textarea: HTMLTextAreaElement =
      fixture.nativeElement.querySelector('#approval-reason');
    textarea.value = text;
    textarea.dispatchEvent(new Event('input'));
    await settle();
  }

  beforeEach(async () => {
    TestBed.configureTestingModule({
      imports: [It03Page],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    http = TestBed.inject(HttpTestingController);
    fixture = TestBed.createComponent(It03Page);
    http.expectOne('/api/it03/documents').flush(DOCUMENTS);
    await settle();
  });

  afterEach(() => http.verify());

  it('lists every document but locks the checkbox of a decided one', async () => {
    expect(fixture.nativeElement.querySelectorAll('tbody tr').length).toBe(3);
    expect(rowCheckboxes().map((box) => box.disabled)).toEqual([false, true, false]);
  });

  it('ticks only the pending rows when the header box is used', async () => {
    fixture.nativeElement.querySelector('thead input[type="checkbox"]').click();
    await settle();

    expect(rowCheckboxes().map((box) => box.checked)).toEqual([true, false, true]);
    expect(textOf('.toolbar-note')).toContain('2');
  });

  it('keeps both actions disabled until a row is selected', async () => {
    expect(approveButton().disabled).toBe(true);
    expect(rejectButton().disabled).toBe(true);

    await select(0);

    expect(approveButton().disabled).toBe(false);
    expect(rejectButton().disabled).toBe(false);
  });

  it('sends every selected id in one approval and reports what came back', async () => {
    await select(0, 2);

    approveButton().click();
    await settle();
    expect(approvalDialog()).not.toBeNull();

    await fillReason('เอกสารครบถ้วน');
    dialogButtons()[0].click();

    const request = http.expectOne('/api/it03/documents/approve');
    expect(request.request.body).toEqual({ documentIds: [1, 4], reason: 'เอกสารครบถ้วน' });
    request.flush({ affectedCount: 2, documentIds: [1, 4], statusNameTh: 'อนุมัติ' });
    await settle();

    // The page refetches instead of patching rows locally, so the test has to
    // answer a second list request before anything renders.
    http.expectOne('/api/it03/documents').flush(DOCUMENTS);
    await settle();

    expect(approvalDialog()).toBeNull();
    expect(textOf('.alert-success')).toContain('2');
  });

  it('closes the dialog without calling the API when cancelled', async () => {
    await select(0);
    approveButton().click();
    await settle();

    await fillReason('พิมพ์ไว้แล้วเปลี่ยนใจ');
    dialogButtons()[1].click();
    await settle();

    expect(approvalDialog()).toBeNull();
    http.expectNone('/api/it03/documents/approve');
  });

  it('shows the reason the server refused the batch', async () => {
    await select(0);
    approveButton().click();
    await settle();

    await fillReason('อนุมัติซ้ำ');
    dialogButtons()[0].click();

    http
      .expectOne('/api/it03/documents/approve')
      .flush(
        { title: 'Conflict', detail: 'เอกสารนี้ถูกอนุมัติไปแล้ว' },
        { status: 409, statusText: 'Conflict' },
      );
    await settle();

    http.expectOne('/api/it03/documents').flush(DOCUMENTS);
    await settle();

    expect(approvalDialog()).toBeNull();
    expect(textOf('.alert-error')).toContain('เอกสารนี้ถูกอนุมัติไปแล้ว');
  });

  it('opens the history of the document whose name was clicked', async () => {
    fixture.nativeElement.querySelectorAll('.name-link')[1].click();
    await settle();

    http.expectOne('/api/it03/documents/2/logs').flush([
      {
        id: 9,
        documentId: 2,
        fromStatusNameTh: 'รออนุมัติ',
        toStatusNameTh: 'อนุมัติ',
        reason: 'เอกสารครบถ้วน',
        actionBy: 'demo.user',
        actionAt: '2026-08-02T10:00:00Z',
      },
    ]);
    await settle();

    const history = fixture.nativeElement.querySelector('app-history-dialog');
    expect(history).not.toBeNull();
    expect(history.textContent).toContain('ใบเบิกวัสดุสำนักงาน');
    expect(history.textContent).toContain('demo.user');
  });
});
