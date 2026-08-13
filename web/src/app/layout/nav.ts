export type NavIcon = 'approve' | 'chart' | 'settings';

export interface NavItem {
  label: string;
  route: string;
  icon: NavIcon;
}

export interface NavSection {
  title: string;
  items: NavItem[];
}

export const NAV_SECTIONS: readonly NavSection[] = [
  {
    title: 'งานเอกสาร',
    items: [
      { label: 'อนุมัติเอกสาร (IT 03)', route: '/it03', icon: 'approve' },
      { label: 'สรุปสถานะเอกสาร', route: '/summary', icon: 'chart' },
    ],
  },
  {
    title: 'ตั้งค่าระบบ',
    items: [{ label: 'ข้อมูลสถานะเอกสาร', route: '/master/status', icon: 'settings' }],
  },
];
