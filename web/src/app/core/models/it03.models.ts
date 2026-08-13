export type DecisionMode = 'approve' | 'reject';

export interface DocumentListItem {
  id: number;
  documentName: string;
  reason: string | null;
  statusId: number;
  statusCode: string;
  statusNameTh: string;
  isPending: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface ApprovalLogEntry {
  id: number;
  documentId: number;
  fromStatusNameTh: string;
  toStatusNameTh: string;
  reason: string;
  actionBy: string;
  actionAt: string;
}

export interface DecisionResult {
  affectedCount: number;
  documentIds: number[];
  statusNameTh: string;
}

export interface DocumentStatusItem {
  id: number;
  code: string;
  nameTh: string;
  documentCount: number;
}
