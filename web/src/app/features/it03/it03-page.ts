import { Component, computed, inject, signal } from '@angular/core';

import { It03Service } from '../../core/it03.service';
import { DecisionMode, DocumentListItem } from '../../core/models/it03.models';
import { describeError } from '../../core/problem-details';
import { ApprovalDialog } from './approval-dialog';
import { HistoryDialog } from './history-dialog';

@Component({
  selector: 'app-it03-page',
  imports: [ApprovalDialog, HistoryDialog],
  templateUrl: './it03-page.html',
  styleUrl: './it03-page.css',
})
export class It03Page {
  private readonly service = inject(It03Service);

  protected readonly documents = signal<DocumentListItem[]>([]);
  protected readonly selectedIds = signal<ReadonlySet<number>>(new Set<number>());
  protected readonly isLoading = signal(false);
  protected readonly isSaving = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly notice = signal<string | null>(null);

  protected readonly dialogMode = signal<DecisionMode | null>(null);
  protected readonly historyDocument = signal<DocumentListItem | null>(null);

  protected readonly pendingCount = computed(
    () => this.documents().filter((document) => document.isPending).length,
  );
  protected readonly selectedCount = computed(() => this.selectedIds().size);

  // Header checkbox reflects the pending rows only, since decided rows can
  // never be selected.
  protected readonly allPendingSelected = computed(() => {
    const selected = this.selectedIds();
    const pending = this.documents().filter((document) => document.isPending);

    return pending.length > 0 && pending.every((document) => selected.has(document.id));
  });

  constructor() {
    this.load();
  }

  protected load(options: { keepError?: boolean } = {}): void {
    this.isLoading.set(true);
    if (!options.keepError) {
      this.errorMessage.set(null);
    }

    this.service.getDocuments().subscribe({
      next: (documents) => {
        this.documents.set(documents);
        this.selectedIds.set(new Set<number>());
        this.isLoading.set(false);
      },
      error: (error: unknown) => {
        this.errorMessage.set(describeError(error));
        this.isLoading.set(false);
      },
    });
  }

  protected isSelected(id: number): boolean {
    return this.selectedIds().has(id);
  }

  protected toggleRow(document: DocumentListItem): void {
    if (!document.isPending) {
      return;
    }

    const next = new Set(this.selectedIds());
    if (!next.delete(document.id)) {
      next.add(document.id);
    }
    this.selectedIds.set(next);
  }

  protected toggleAll(): void {
    if (this.allPendingSelected()) {
      this.selectedIds.set(new Set<number>());
      return;
    }

    this.selectedIds.set(
      new Set(this.documents().filter((document) => document.isPending).map((document) => document.id)),
    );
  }

  protected openDialog(mode: DecisionMode): void {
    if (this.selectedCount() === 0) {
      return;
    }

    this.notice.set(null);
    this.errorMessage.set(null);
    this.dialogMode.set(mode);
  }

  protected closeDialog(): void {
    this.dialogMode.set(null);
  }

  protected confirmDecision(reason: string): void {
    const mode = this.dialogMode();
    if (mode === null) {
      return;
    }

    const ids = [...this.selectedIds()];
    this.isSaving.set(true);

    this.service.decide(mode, ids, reason).subscribe({
      next: (result) => {
        this.isSaving.set(false);
        this.dialogMode.set(null);
        this.notice.set(`อัปเดตสถานะเป็น "${result.statusNameTh}" จำนวน ${result.affectedCount} รายการ`);
        this.load();
      },
      error: (error: unknown) => {
        this.isSaving.set(false);
        this.dialogMode.set(null);
        this.errorMessage.set(describeError(error));
        // The batch was rejected server-side, so pull fresh state rather than
        // trusting what the table shows. The refresh must not wipe the message
        // explaining why it was rejected.
        this.load({ keepError: true });
      },
    });
  }

  protected openHistory(document: DocumentListItem): void {
    this.historyDocument.set(document);
  }

  protected closeHistory(): void {
    this.historyDocument.set(null);
  }

  protected statusClass(document: DocumentListItem): string {
    return `status-${document.statusCode.toLowerCase()}`;
  }
}
