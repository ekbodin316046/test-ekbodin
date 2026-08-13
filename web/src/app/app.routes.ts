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
      { path: '**', redirectTo: 'it03' },
    ],
  },
];
