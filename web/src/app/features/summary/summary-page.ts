import { Component, computed, inject, signal } from '@angular/core';

import { It03Service } from '../../core/it03.service';
import { DocumentListItem } from '../../core/models/it03.models';
import { describeError } from '../../core/problem-details';
import { Spinner } from '../../shared/spinner';

interface StatusTile {
  code: string;
  nameTh: string;
  count: number;
  percent: number;
}

@Component({
  selector: 'app-summary-page',
  imports: [Spinner],
  templateUrl: './summary-page.html',
  styleUrl: './summary-page.css',
})
export class SummaryPage {
  private readonly service = inject(It03Service);

  protected readonly documents = signal<DocumentListItem[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal<string | null>(null);

  // Derived from the list endpoint rather than a dedicated one — the page needs
  // no data the table does not already fetch.
  protected readonly tiles = computed<StatusTile[]>(() => {
    const all = this.documents();
    const order = [
      { code: 'PENDING', nameTh: 'รออนุมัติ' },
      { code: 'APPROVED', nameTh: 'อนุมัติ' },
      { code: 'REJECTED', nameTh: 'ไม่อนุมัติ' },
    ];

    return order.map(({ code, nameTh }) => {
      const count = all.filter((document) => document.statusCode === code).length;

      return {
        code,
        nameTh,
        count,
        percent: all.length === 0 ? 0 : Math.round((count / all.length) * 100),
      };
    });
  });

  protected readonly total = computed(() => this.documents().length);

  constructor() {
    this.service.getDocuments().subscribe({
      next: (documents) => {
        this.documents.set(documents);
        this.isLoading.set(false);
      },
      error: (error: unknown) => {
        this.errorMessage.set(describeError(error));
        this.isLoading.set(false);
      },
    });
  }

  protected statusClass(code: string): string {
    return `status-${code.toLowerCase()}`;
  }
}
