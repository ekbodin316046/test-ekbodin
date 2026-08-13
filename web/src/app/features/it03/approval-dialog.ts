import { Component, computed, input, output, signal } from '@angular/core';

import { DecisionMode } from '../../core/models/it03.models';

@Component({
  selector: 'app-approval-dialog',
  templateUrl: './approval-dialog.html',
  styleUrl: './approval-dialog.css',
})
export class ApprovalDialog {
  readonly mode = input.required<DecisionMode>();
  readonly count = input.required<number>();
  readonly saving = input(false);

  readonly confirmed = output<string>();
  readonly cancelled = output<void>();

  protected readonly reason = signal('');

  protected readonly isApprove = computed(() => this.mode() === 'approve');
  protected readonly title = computed(() =>
    this.isApprove() ? 'ยืนยันการอนุมัติ' : 'ยืนยันการไม่อนุมัติ',
  );
  protected readonly confirmLabel = computed(() => (this.isApprove() ? 'อนุมัติ' : 'ไม่อนุมัติ'));
  protected readonly canConfirm = computed(() => this.reason().trim().length > 0 && !this.saving());

  protected onReasonInput(event: Event): void {
    this.reason.set((event.target as HTMLTextAreaElement).value);
  }

  protected submit(): void {
    if (this.canConfirm()) {
      this.confirmed.emit(this.reason().trim());
    }
  }
}
