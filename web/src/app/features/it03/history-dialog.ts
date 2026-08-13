import { Component, OnInit, inject, input, output, signal } from '@angular/core';

import { It03Service } from '../../core/it03.service';
import { ApprovalLogEntry, DocumentListItem } from '../../core/models/it03.models';
import { describeError } from '../../core/problem-details';

@Component({
  selector: 'app-history-dialog',
  templateUrl: './history-dialog.html',
  styleUrl: './history-dialog.css',
})
export class HistoryDialog implements OnInit {
  readonly document = input.required<DocumentListItem>();
  readonly closed = output<void>();

  private readonly service = inject(It03Service);

  protected readonly entries = signal<ApprovalLogEntry[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.service.getHistory(this.document().id).subscribe({
      next: (entries) => {
        this.entries.set(entries);
        this.isLoading.set(false);
      },
      error: (error: unknown) => {
        this.errorMessage.set(describeError(error));
        this.isLoading.set(false);
      },
    });
  }

  protected formatDate(value: string): string {
    return new Date(value).toLocaleString('th-TH', {
      dateStyle: 'medium',
      timeStyle: 'short',
    });
  }
}
