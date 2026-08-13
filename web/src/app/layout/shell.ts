import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { AppHeader } from './header';
import { Sidebar } from './sidebar';

@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, AppHeader, Sidebar],
  templateUrl: './shell.html',
  styleUrl: './shell.css',
})
export class Shell {
  protected readonly sidebarOpen = signal(true);

  protected toggleSidebar(): void {
    this.sidebarOpen.update((open) => !open);
  }
}
