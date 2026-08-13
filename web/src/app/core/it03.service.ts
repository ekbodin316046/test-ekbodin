import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ApprovalLogEntry,
  DecisionMode,
  DecisionResult,
  DocumentListItem,
  DocumentStatusItem,
} from './models/it03.models';

@Injectable({ providedIn: 'root' })
export class It03Service {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/it03';

  getDocuments(): Observable<DocumentListItem[]> {
    return this.http.get<DocumentListItem[]>(`${this.baseUrl}/documents`);
  }

  decide(mode: DecisionMode, documentIds: number[], reason: string): Observable<DecisionResult> {
    return this.http.post<DecisionResult>(`${this.baseUrl}/documents/${mode}`, {
      documentIds,
      reason,
    });
  }

  getHistory(documentId: number): Observable<ApprovalLogEntry[]> {
    return this.http.get<ApprovalLogEntry[]>(`${this.baseUrl}/documents/${documentId}/logs`);
  }

  getStatuses(): Observable<DocumentStatusItem[]> {
    return this.http.get<DocumentStatusItem[]>(`${this.baseUrl}/statuses`);
  }
}
