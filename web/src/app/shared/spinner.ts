import { Component, input } from '@angular/core';

@Component({
  selector: 'app-spinner',
  template: `
    <div class="wrap" role="status">
      <span class="spinner" aria-hidden="true"></span>
      <span class="label">{{ label() }}</span>
    </div>
  `,
  styles: `
    .wrap {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 10px;
      padding: 26px 16px;
      color: var(--color-muted);
      font-size: 13px;
    }
  `,
})
export class Spinner {
  readonly label = input('กำลังโหลดข้อมูล');
}
