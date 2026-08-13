import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { It03Service } from './it03.service';

describe('It03Service', () => {
  let service: It03Service;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(It03Service);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('reads the document list', () => {
    service.getDocuments().subscribe();

    const request = http.expectOne('/api/it03/documents');
    expect(request.request.method).toBe('GET');
    request.flush([]);
  });

  it('sends every selected id in a single approval request', () => {
    service.decide('approve', [1, 4], 'เอกสารครบถ้วน').subscribe();

    const request = http.expectOne('/api/it03/documents/approve');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ documentIds: [1, 4], reason: 'เอกสารครบถ้วน' });
    request.flush({ affectedCount: 2, documentIds: [1, 4], statusNameTh: 'อนุมัติ' });
  });

  it('routes a rejection to its own endpoint', () => {
    service.decide('reject', [2], 'เอกสารไม่ครบ').subscribe();

    const request = http.expectOne('/api/it03/documents/reject');
    expect(request.request.body).toEqual({ documentIds: [2], reason: 'เอกสารไม่ครบ' });
    request.flush({ affectedCount: 1, documentIds: [2], statusNameTh: 'ไม่อนุมัติ' });
  });

  it('reads the approval history of one document', () => {
    service.getHistory(7).subscribe();

    http.expectOne('/api/it03/documents/7/logs').flush([]);
  });

  it('reads the status master list', () => {
    service.getStatuses().subscribe();

    http.expectOne('/api/it03/statuses').flush([]);
  });
});
