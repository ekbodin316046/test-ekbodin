import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DecisionMode } from '../../core/models/it03.models';
import { ApprovalDialog } from './approval-dialog';

describe('ApprovalDialog', () => {
  let fixture: ComponentFixture<ApprovalDialog>;

  async function open(mode: DecisionMode = 'approve', count = 2, saving = false): Promise<void> {
    fixture = TestBed.createComponent(ApprovalDialog);
    fixture.componentRef.setInput('mode', mode);
    fixture.componentRef.setInput('count', count);
    fixture.componentRef.setInput('saving', saving);
    fixture.detectChanges();
    await fixture.whenStable();
  }

  function actions(): HTMLButtonElement[] {
    return Array.from(fixture.nativeElement.querySelectorAll('.modal-actions button'));
  }

  function confirmButton(): HTMLButtonElement {
    return actions()[0];
  }

  function cancelButton(): HTMLButtonElement {
    return actions()[1];
  }

  async function type(text: string): Promise<void> {
    const textarea: HTMLTextAreaElement = fixture.nativeElement.querySelector('#approval-reason');
    textarea.value = text;
    textarea.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    await fixture.whenStable();
  }

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [ApprovalDialog] });
  });

  it('refuses to confirm until the reason holds real text', async () => {
    await open();
    expect(confirmButton().disabled).toBe(true);

    await type('   ');
    expect(confirmButton().disabled).toBe(true);

    await type('เอกสารครบถ้วน');
    expect(confirmButton().disabled).toBe(false);
  });

  it('emits the reason without its surrounding spaces', async () => {
    await open();
    const emitted: string[] = [];
    fixture.componentInstance.confirmed.subscribe((reason) => emitted.push(reason));

    await type('  เอกสารครบถ้วน  ');
    confirmButton().click();

    expect(emitted).toEqual(['เอกสารครบถ้วน']);
  });

  it('cancels without emitting a decision even after a reason is typed', async () => {
    await open();
    const confirmed: string[] = [];
    let cancelledCount = 0;
    fixture.componentInstance.confirmed.subscribe((reason) => confirmed.push(reason));
    fixture.componentInstance.cancelled.subscribe(() => (cancelledCount += 1));

    await type('พิมพ์ไว้แล้วเปลี่ยนใจ');
    cancelButton().click();

    expect(cancelledCount).toBe(1);
    expect(confirmed).toEqual([]);
  });

  it('carries the rejection wording when opened to reject', async () => {
    await open('reject', 3);

    expect(fixture.nativeElement.querySelector('.modal-head').textContent).toContain(
      'ยืนยันการไม่อนุมัติ',
    );
    expect(confirmButton().textContent).toContain('ไม่อนุมัติ');
    expect(fixture.nativeElement.querySelector('.count-note').textContent).toContain('3');
  });

  it('locks both buttons while the decision is in flight', async () => {
    await open('approve', 1, true);
    await type('เอกสารครบถ้วน');

    expect(confirmButton().disabled).toBe(true);
    expect(cancelButton().disabled).toBe(true);
  });
});
