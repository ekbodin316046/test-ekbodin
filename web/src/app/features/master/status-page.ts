import { Component, inject, signal } from '@angular/core';

import { It03Service } from '../../core/it03.service';
import { DocumentStatusItem } from '../../core/models/it03.models';
import { describeError } from '../../core/problem-details';

@Component({
  selector: 'app-status-page',
  templateUrl: './status-page.html',
  styleUrl: './status-page.css',
})
export class StatusPage {
  private readonly service = inject(It03Service);

  protected readonly statuses = signal<DocumentStatusItem[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal<string | null>(null);

  constructor() {
    this.service.getStatuses().subscribe({
      next: (statuses) => {
        this.statuses.set(statuses);
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
