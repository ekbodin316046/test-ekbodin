import { HttpErrorResponse } from '@angular/common/http';

import { describeError } from './problem-details';

describe('describeError', () => {
  it('prefers validation messages over the generic problem title', () => {
    const message = describeError(
      new HttpErrorResponse({
        status: 400,
        error: {
          title: 'One or more validation errors occurred.',
          errors: { Reason: ['กรุณาระบุเหตุผล'] },
        },
      }),
    );

    expect(message).toBe('กรุณาระบุเหตุผล');
  });

  it('shows the business rule detail behind a 409', () => {
    const message = describeError(
      new HttpErrorResponse({
        status: 409,
        error: { title: 'Conflict', detail: 'เอกสารนี้ถูกอนุมัติไปแล้ว' },
      }),
    );

    expect(message).toBe('เอกสารนี้ถูกอนุมัติไปแล้ว');
  });

  it('points at the API when the request never reached it', () => {
    expect(describeError(new HttpErrorResponse({ status: 0 }))).toContain('dotnet run');
  });

  it('falls back to the status code when the body carries no problem details', () => {
    expect(describeError(new HttpErrorResponse({ status: 500, error: null }))).toBe(
      'เกิดข้อผิดพลาด (500)',
    );
  });

  it('handles a thrown value that is not an http response at all', () => {
    expect(describeError(new Error('boom'))).toBe('เกิดข้อผิดพลาดที่ไม่คาดคิด');
  });
});
