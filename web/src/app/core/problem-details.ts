import { HttpErrorResponse } from '@angular/common/http';

interface ProblemDetails {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
}

// Turns the API's RFC 7807 payload into one line for the user. Validation
// failures carry their messages under errors rather than detail.
export function describeError(error: unknown): string {
  if (!(error instanceof HttpErrorResponse)) {
    return 'เกิดข้อผิดพลาดที่ไม่คาดคิด';
  }

  if (error.status === 0) {
    return 'ไม่สามารถเชื่อมต่อ API ได้ ตรวจสอบว่าเรียกใช้ dotnet run แล้วหรือไม่';
  }

  const problem = error.error as ProblemDetails | null;

  const validationMessages = Object.values(problem?.errors ?? {}).flat();
  if (validationMessages.length > 0) {
    return validationMessages.join(' ');
  }

  return problem?.detail ?? problem?.title ?? `เกิดข้อผิดพลาด (${error.status})`;
}
