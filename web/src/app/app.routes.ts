import { Routes } from '@angular/router';

import { Shell } from './layout/shell';

export const routes: Routes = [
  {
    path: '',
    component: Shell,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'it03' },
      {
        path: 'it03',
        title: 'อนุมัติเอกสาร (IT 03)',
        loadComponent: () => import('./features/it03/it03-page').then((m) => m.It03Page),
      },
      {
        path: 'summary',
        title: 'สรุปสถานะเอกสาร',
        loadComponent: () => import('./features/summary/summary-page').then((m) => m.SummaryPage),
      },
      {
        path: 'master/status',
        title: 'ข้อมูลสถานะเอกสาร',
        loadComponent: () => import('./features/master/status-page').then((m) => m.StatusPage),
      },
      { path: '**', redirectTo: 'it03' },
    ],
  },
];
